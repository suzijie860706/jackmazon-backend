using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Services;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.ViewModels;
using Jacmazon_ECommerce.Extensions;
using AutoMapper;

namespace Jacmazon_ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAPIController : ControllerBase
    {
        private readonly ILogger<WebAPIController> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public WebAPIController(IAntiforgery antiforgery, ILogger<WebAPIController> logger, IUserService userService,
            ITokenService tokenService, IProductService productService, IMapper mapper)
        {
            _antiforgery = antiforgery;
            _logger = logger;
            _userService = userService;
            _tokenService = tokenService;
            _productService = productService;
            _mapper = mapper;
        }

        /// <summary>
        /// 取得Lgg-----temperary
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLogs")]
        public IActionResult GetLogs()
        {
            var d = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(d, $"Serilogs/log-{DateTime.Now.ToString("yyyyMMdd")}.txt");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Log file not found.");
            }

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fileStream))
                {
                    var logs = reader.ReadToEnd().Split(Environment.NewLine);
                    return Ok(logs);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Controller}/{Action} Fail",
                ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                throw;
            }
        }

        /// <summary>
        /// 取得防偽表單Token
        /// </summary>
        /// <returns></returns>
        [HttpGet("antiForgery")]
        public IActionResult GetAntiforgeryToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
                new CookieOptions { HttpOnly = false });

            return Ok(new { token = tokens.RequestToken });
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] UserViewModel user)
        {
            try
            {
                // 驗證帳密
                bool isValidUser = await _userService.VerifyUserLogin(user.Email, user.Password);
                if (!isValidUser)
                {
                    return Ok(new Response<string>
                    {
                        Success = false,
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "帳號或密碼錯誤",
                    });
                }

                //建立Token
                TokenViewModel token1 = await _tokenService.CreateTokenAsync(user.Email);

                return Ok(new Response<TokenViewModel>
                {
                    Success = true,
                    Status = StatusCodes.Status200OK,
                    Data = token1
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 註冊帳號
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("CreateAccount")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount([FromBody] UserViewModel user)
        {
            try
            {
                //驗證Email
                bool isEmailRegistered = await _userService.IsEmailRegisteredAsync(user.Email);
                if (isEmailRegistered)
                {
                    return Ok(new Response<string>
                    {
                        Success = false,
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "帳號已建立",
                    });
                }
                
                User user1 = _mapper.Map<User>(user);
                await _userService.CreateUserAsync(user1);

                return Ok(new Response<string>
                {
                    Success = true,
                    Status = StatusCodes.Status200OK,
                });
            }
            catch (Exception)
            {
                throw;
            }


        }

        /// <summary>
        /// 取得產品資料
        /// </summary>
        /// <returns></returns>
        [HttpGet("ProductList")]
        [Authorize]
        public async Task<IActionResult> ProductList()
        {
            try
            {
                IEnumerable<ProductViewModel> productResponseDtos = await _productService.GetAllProducts();

                return Ok(new Response<IEnumerable<ProductViewModel>>
                {
                    Success = true,
                    Status = StatusCodes.Status200OK,
                    Data = productResponseDtos
                });

            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 取得新的Access Token
        /// </summary>
        /// <returns></returns>
        [HttpPost("Refresh_Token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                Response<string> response = await _tokenService.UpdateRefreshTokenAsync(refreshToken);

                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //[HttpGet("LoginIndex2")]
        //[Authorize(Roles = "Test")]
        //public IActionResult LoginIndex2()
        //{
        //    return Content("");
        //}

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            try
            {
                Response<string> response = await _tokenService.DeleteRefreshTokenAsync(refreshToken);

                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 驗證Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Email")]
        public async Task<IActionResult> Email([FromBody] string email)
        {
            EmailValidateAttribute emailValidateAttribute = new();
            bool isEmailValid = emailValidateAttribute.IsValid(email);

            if (isEmailValid)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "格式錯誤",
                });
            }

            try
            {
                bool isRegistered = await _userService.IsEmailRegisteredAsync(email);
                if (isRegistered)
                {
                    return Ok(new Response<string>
                    {
                        Success = false,
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "電子信箱已註冊",
                    });
                }

                //TODO:驗證碼待實作
                string verifyCode = "";

                return Ok(new Response<string>
                {
                    Success = true,
                    Status = StatusCodes.Status200OK,
                    Data = verifyCode
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 驗證手機號碼
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost("phone")]
        public async Task<IActionResult> Phone([FromBody] string phone)
        {
            PhoneValidateAttribute phoneValidateAttribute = new();
            bool isPhoneValid = phoneValidateAttribute.IsValid(phone);
            if (isPhoneValid)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "格式錯誤",
                });
            }

            try
            {
                bool isRegistered = await _userService.IsPhoneRegisteredAsync(phone);
                if (isRegistered)
                {
                    return Ok(new Response<string>
                    {
                        Success = false,
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "手機號碼已註冊",
                    });
                }

                //TODO:驗證碼待實作
                string verifyCode = "";

                return Ok(new Response<string>
                {
                    Success = true,
                    Status = StatusCodes.Status200OK,
                    Data = verifyCode
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
