using Jacmazon_ECommerce.Models.LoginContext;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Jacmazon_ECommerce.JWTServices
{
    public static class TokenServices
    {
        /// <summary>
        /// /// 取得Access Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string CreateAccessToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(Settings.Secret);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = Settings.Issuer,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, user.Email ?? ""),
                    new(ClaimTypes.Role, user.Password ?? "")
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
        /// <param name="user"></param>
        /// <returns></returns>
        public static string CreateRefreshToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(Settings.Secret);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = Settings.Issuer,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, user.Password ?? "")
                }),
                Expires = Settings.Refresh_Expired_Date(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
