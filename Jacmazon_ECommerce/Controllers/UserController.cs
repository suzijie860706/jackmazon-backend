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
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IValidationService _validationService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        
        private readonly IMapper _mapper;

        public UserController(IAntiforgery antiforgery, IUserService userService, IValidationService validationService,
            ITokenService tokenService, IMapper mapper)
        {
            _antiforgery = antiforgery;
            _userService = userService;
            _validationService = validationService;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        /// <summary>
        /// 取得Lgg-----temperary
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logs")]
        public IActionResult Logs()
        {
            var d = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(d, $"Serilogs/log-{DateTime.Now.ToString("yyyyMMdd")}.txt");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Log file not found.");
            }

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream))
            {
                var logs = reader.ReadToEnd().Split(Environment.NewLine);
                return Ok(logs);
            }

        }

        /// <summary>
        /// 取得防偽表單Token
        /// </summary>
        /// <returns></returns>
        [HttpGet("AntiforgeryToken")]
        public IActionResult AntiforgeryToken()
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
        [ProducesResponseType(typeof(Response<TokenViewModel>), 200)]
        [ProducesResponseType(typeof(Response<string>), 401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Login([FromBody] UserParameter user)
        {
            // 驗證帳密
            Response<string> response = await _userService.UserVerify(user);
            if (!response.Success)
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

        /// <summary>
        /// 註冊帳號
        /// </summary>
        /// <param name="user">使用者資訊</param>
        /// <returns></returns>
        [HttpPost("CreateAccount")]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<string>), 401)]
        [ProducesResponseType(500)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount([FromBody] UserParameter user)
        {
            //驗證Email
            bool isValid = await _userService.IsEmailNotRegisteredAsync(user.Email);
            if (!isValid)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "帳號已建立",
                });
            }

            await _userService.CreateUserAsync(user.Email, user.Password);

            return Ok(new Response<string>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
            });
        }

        /// <summary>
        /// 取得新的Access Token
        /// </summary>
        /// <param name="refreshToken">長期權杖</param>
        /// <returns></returns>
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            Response<string> response = await _tokenService.UpdateRefreshTokenAsync(refreshToken);

            return Ok(response);
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
        /// <param name="logoutParameter">使用者登出權杖</param>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutParameter logoutParameter)
        {
            await _tokenService.DeleteRefreshTokenAsync(logoutParameter.RefreshToken);

            return Ok(new Response<string>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
            });
        }

        /// <summary>
        /// 驗證Email
        /// </summary>
        /// <param name="email">電子信箱</param>
        /// <returns></returns>
        [HttpGet("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email)
        {
            if (!_validationService.IsValidEmail(email))
            {
                return Ok(new FailResponse400("格式錯誤"));
            }

            if (!await _userService.IsEmailNotRegisteredAsync(email))
            {
                return Ok(new FailResponse401("電子信箱已註冊"));
            }

            return Ok(new OkResponse());
            
        }

        /// <summary>
        /// 驗證手機號碼
        /// </summary>
        /// <param name="phone">電話</param>
        /// <returns></returns>
        [HttpGet("VerifyPhone")]
        public async Task<IActionResult> VerifyPhone([FromQuery] string phone)
        {
            if (!_validationService.IsValidPhone(phone))
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "格式錯誤",
                });
            }

            if (!await _userService.IsPhoneNotRegisteredAsync(phone))
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "手機號碼已註冊",
                });
            }

            return Ok(new Response<string>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
            });
        }

        [HttpGet]
        public ActionResult Test()
        {
            return Ok();
        }
    }
}
