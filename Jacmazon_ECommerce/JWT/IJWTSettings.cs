using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jacmazon_ECommerce.JWT
{
    public interface IJWTSettings
    {
        public static readonly string Secret = "Jacmazon_ECommerce20240321JackSu";

        public static readonly string Issuer = "http://localhost:5092/";

        /// <summary>
        /// 取得Access Token
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string CreateAccessToken(string email);

        /// <summary>
        /// 取得Refresh Token
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string CreateRefreshToken(string email);

        /// <summary>
        /// 取得Refresh Token Expired Date
        /// </summary>
        public DateTime Refresh_Expired_Date();

        /// <summary>
        /// 讀取Token資料
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        JwtSecurityToken? ReadToken(string token);
    }
}
