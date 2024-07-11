using Azure.Core;
using Jacmazon_ECommerce.JWTServices;
using Jacmazon_ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Jacmazon_ECommerce.Models;

namespace Jacmazon_ECommerce.Ｍiddlewares
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;



        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;

        }

        public async Task InvokeAsync(HttpContext context, LoginContext _loginContext)
        {
            // Call the next middleware in the pipeline
            await _next(context);

            string WWW_Authenticate = context.Response.Headers.WWWAuthenticate.ToString();
            
            //使用Authorize
            if (WWW_Authenticate.Contains("Bearer"))
            {
                //驗證Authorize失敗
                //if ()
                // {
                ////取得完整token
                //string Authorization = context.Request.Headers.Authorization.ToString();
                //string refreshToken = Authorization.Split(" ")[1];

                //DbToken? dbToken = await _loginContext.DbTokens.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
                //var tokenHandler = new JwtSecurityTokenHandler();
                //JwtSecurityToken? jwtSecurityToken = tokenHandler.ReadToken(refreshToken) as JwtSecurityToken;

                ////驗證Refresh Token尚未過期
                //if (dbToken != null && jwtSecurityToken != null && DateTime.Now < jwtSecurityToken.ValidTo)
                //{
                //    //回傳AccessToken
                //    string nameClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
                //    string roleClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";

                //    UserTable userTable = new UserTable { Email = nameClaim, Password = roleClaim };
                //    string token = TokenServices.CreateAccessToken(userTable);
                //    dbToken.AccessToken = token;

                //    //更新資料庫AccessToken
                //    _loginContext.DbTokens.Update(dbToken);
                //    await _loginContext.SaveChangesAsync();

                //回傳token
                var response = new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Authorization failed. Access denied.",
                    Data = ""
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));

                //}

                //Refresh Token過期
                //else
                //{
                //    context.Response.StatusCode = StatusCodes.Status200OK;
                //    context.Response.ContentType = "application/json";
                //    var response = new Response<string>
                //    {
                //        Success = false,
                //        Status = StatusCodes.Status401Unauthorized,
                //        Message = "Token timeout",
                //        Data = ""
                //    };

                //    //await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                //}
                //  }
                //驗證Authorize成功
                //else
                //{
                //    //更新Refresh Token
                //    //取得完整token
                //    string Authorization = context.Request.Headers.Authorization.ToString();
                //    string accessToken = Authorization.Split(" ")[1];

                //    //解析Token
                //    var tokenHandler = new JwtSecurityTokenHandler();
                //    JwtSecurityToken? jwtSecurityToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;

                //    //更新資料庫Refresh Token
                //    DbToken? dbToken = await _loginContext.DbTokens.FirstOrDefaultAsync(u => u.AccessToken == accessToken);
                //    if (dbToken != null && jwtSecurityToken != null)
                //    {
                //        string nameClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
                //        string roleClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";

                //        UserTable userTable = new UserTable { Email = nameClaim, Password = roleClaim };
                //        string token = TokenServices.CreateRefreshToken(userTable);
                //        dbToken.RefreshToken = token;

                //        _loginContext.DbTokens.Update(dbToken);
                //        await _loginContext.SaveChangesAsync();
                //    }
                //}
            }
        }
    }
}
