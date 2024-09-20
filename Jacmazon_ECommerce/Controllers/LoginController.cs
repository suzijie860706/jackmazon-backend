using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Services;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.ViewModels;
using Jacmazon_ECommerce.Extensions;
using AutoMapper;
using System.Runtime.Intrinsics.Arm;

namespace Jacmazon_ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IValidationService _validationService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        
        private readonly IMapper _mapper;

        public LoginController(IAntiforgery antiforgery, IUserService userService, IValidationService validationService,
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
        [HttpGet("GetLogs")]
        public IActionResult GetLogs()
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
            System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
            System.Security.Cryptography.RSA rsa = System.Security.Cryptography.RSA.Create();
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
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("CreateAccount")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount([FromBody] UserViewModel user)
        {
            //驗證Email
            bool isEmailRegistered = await _userService.IsEmailNotRegisteredAsync(user.Email);
            if (isEmailRegistered)
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
        /// <returns></returns>
        [HttpPost("Refresh_Token")]
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
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            await _tokenService.DeleteRefreshTokenAsync(refreshToken);

            return Ok(new Response<string>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
            });
        }

        /// <summary>
        /// 驗證Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Email")]
        public async Task<IActionResult> Email([FromBody] string email)
        {
            if (!_validationService.IsValidEmail(email))
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "格式錯誤",
                });
            }

            if (!await _userService.IsEmailNotRegisteredAsync(email))
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "電子信箱已註冊",
                });
            }

            return Ok(new Response<string>(true));
            
        }

        /// <summary>
        /// 驗證手機號碼
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost("phone")]
        public async Task<IActionResult> Phone([FromBody] string phone)
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
    }
}
