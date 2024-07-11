using Microsoft.AspNetCore.Mvc;
using Jacmazon_ECommerce.Data;
using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.Models.AdventureWorksLT2016Context;
using Microsoft.EntityFrameworkCore;
using Jacmazon_ECommerce.JWTServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using System.IdentityModel.Tokens.Jwt;

namespace Jacmazon_ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAPIController : ControllerBase
    {
        private readonly AdventureWorksLt2016Context _context;
        private readonly LoginContext _loginContext;

        private readonly IAntiforgery _antiforgery;
        public WebAPIController(AdventureWorksLt2016Context context, IAntiforgery antiforgery, LoginContext loginContext)
        {
            _context = context;
            _antiforgery = antiforgery;
            _loginContext = loginContext;
        }

        [HttpGet("getImage/{id}")]
        public FileContentResult? GetImage(int? id)
        {
            Product? product = _context.Products.Find(id);
            if (product != null && product.ThumbNailPhoto != null)
            {
                return File(product.ThumbNailPhoto, "image/" + Path.GetExtension(product.ThumbnailPhotoFileName));
            }
            else return null;
        }

        [HttpGet("antiForgery")]
        public IActionResult GetAntiforgeryToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
                new CookieOptions { HttpOnly = false });

            return Ok(new { token = tokens.RequestToken });
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public IActionResult Post([FromBody] User user)
        {
            if (user == null)
            {
                return NotFound(new { message = "查無此人" });
            }

            string token = TokenServices.CreateAccessToken(user);
            return Ok(new { token });
        }



        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Content("匿名登入22", "text/plain");
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (user.Account == null || user.Password == null || !ModelState.IsValid) 
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "驗證錯誤",
                    Data = ""
                });
            }

            User? userLogin = await _loginContext.Users.FirstOrDefaultAsync(u => u.Account == user.Account && u.Password == user.Password);

            //驗證帳密
            if (userLogin == null)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "帳號或密碼錯誤",
                    Data = ""
                });
            }

            string accessToken = TokenServices.CreateAccessToken(user);
            string refreshToken = TokenServices.CreateRefreshToken(user);

            //新增至資料庫
            _loginContext.Tokens.Add(new Token
            {
                RefreshToken = refreshToken,
                ExpiredDate = Settings.Refresh_Expired_Date(),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            });
            _loginContext.SaveChanges();

            var token = new { AccessToken = accessToken, RefreshToken = refreshToken };
            return Ok(new Response<object>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
                Message = "",
                Data = token
            });
        }

        [HttpPost("CreateAccount")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount([FromBody] User user)
        {
            if (user.Account == null || user.Password == null || !ModelState.IsValid) { return Content("驗證錯誤"); }

            List<User> t = _loginContext.Users.ToList();
            User? userLogin = await _loginContext.Users.FirstOrDefaultAsync(u => u.Account == user.Account);
            //驗證帳密
            if (userLogin != null && userLogin.Account == user.Account)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "帳號已建立",
                    Data = ""
                });
            }

            User newUser = new()
            {
                Account = user.Account,
                Password = user.Password,
                Name = "",
                Rank = 0,
                Approved = true
            };

            await _loginContext.Users.AddAsync(newUser);
            await _loginContext.SaveChangesAsync();

            return Ok(new Response<string>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
                Message = "",
                Data = ""
            });

        }

        /// <summary>
        /// 取得產品資料
        /// </summary>
        /// <returns></returns>
        [HttpGet("ProductList")]
        [Authorize]
        public IActionResult ProductList()
        {
            try
            {
                List<Product> products = _context.Products.ToList();
                return Ok(new Response<List<Product>>
                {
                    Success = true,
                    Status = StatusCodes.Status200OK,
                    Message = "",
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }

        }

        /// <summary>
        /// 取得新的Access Token
        /// </summary>
        /// <returns></returns>
        [HttpPost("Refresh_Token")]
        public async Task<IActionResult> Refresh_Token([FromBody] string refreshToken)
        {
            Token? dbToken = await _loginContext.Tokens.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            //Refresh Token不存在
            if (dbToken == null)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Refresh_Token not found",
                    Data = ""
                });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = tokenHandler.ReadToken(refreshToken) as JwtSecurityToken;
            //Refresh Token過期
            if (DateTime.Now > dbToken.ExpiredDate)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Refresh_Token Expired",
                    Data = ""
                });
            }

            //更新資料庫
            dbToken.ExpiredDate = Settings.Refresh_Expired_Date();
            dbToken.UpdatedDate = DateTime.Now;
            _loginContext.Tokens.Update(dbToken);
            _loginContext.SaveChanges();

            //產生新的Access Token並回傳
            string nameClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
            string roleClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";

            User userTable = new User { Account = nameClaim, Password = roleClaim };
            string token = TokenServices.CreateAccessToken(userTable);

            return Ok(new Response<string>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
                Message = "",
                Data = token
            });
        }

        [HttpGet("LoginIndex2")]
        [Authorize(Roles = "Test")]
        public IActionResult LoginIndex2()
        {
            return Content("");
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            Token? dbToken = await _loginContext.Tokens.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (dbToken == null)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Refresh_Token not found",
                    Data = ""
                });
            }

            _loginContext.Tokens.Remove(dbToken);
            _loginContext.SaveChanges();

            return Ok(new Response<string>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
                Message = "",
                Data = ""
            });
        }
        /*
        /// <summary>
        /// 驗證Email，是否已註冊
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("Email")]
        public async Task<IActionResult> Email ([FromBody] string userEmail)
        {
            if (userEmail == null || !ModelState.IsValid)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "驗證錯誤",
                    Data = ""
                });
            }

            DbUser? userLogin = await _loginContext.DbUsers.FirstOrDefaultAsync(u => u.UserEmail == userEmail);
            if (userLogin != null)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "電子信箱已註冊",
                    Data = ""
                });
            }

            #region 發送驗證碼回去，並新增至資料庫等待驗證
            string verifyCode = "";
            #endregion

            return Ok(new Response<string>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
                Message = "",
                Data = verifyCode
            });
        }

        /// <summary>
        /// 驗證手機號碼是否已註冊
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("phone")]
        public async Task<IActionResult> Phone([FromBody] string userPhone)
        {
            if (userPhone == null || !ModelState.IsValid)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "驗證錯誤",
                    Data = ""
                });
            }

            DbUser? userLogin = await _loginContext.DbUsers.FirstOrDefaultAsync(u => u.UserEmail == userPhone);
            if (userLogin != null)
            {
                return Ok(new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "手機已註冊",
                    Data = ""
                });
            }

            #region 發送驗證碼回去，並新增至資料庫等待驗證
            string verifyCode = "";
            #endregion

            return Ok(new Response<string>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
                Message = "",
                Data = verifyCode
            });
        }
        */
    }
}
