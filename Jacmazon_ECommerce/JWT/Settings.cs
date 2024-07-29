using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jacmazon_ECommerce.JWT
{
    public static class Settings
    {
        public static readonly string Secret = "Jacmazon_ECommerce20240321JackSu";

        public static readonly string Issuer = "http://localhost:5092/";

        /// <summary>
        /// 取得Access Token
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string CreateAccessToken(string email)
        {
            var key = Encoding.UTF8.GetBytes(Settings.Secret);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = Settings.Issuer,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, email),
                    //new(ClaimTypes.Role,  ?? "")
                }),
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 取得Refresh Token
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string CreateRefreshToken(string email)
        {
            var key = Encoding.UTF8.GetBytes(Settings.Secret);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = Settings.Issuer,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, email),
                    //new Claim(ClaimTypes.Role, )
                }),
                Expires = Settings.Refresh_Expired_Date(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 取得Refresh Token Expired Date
        /// </summary>
        public static DateTime Refresh_Expired_Date()
        {
            return DateTime.Now.AddSeconds(40);
        }
    }
}
