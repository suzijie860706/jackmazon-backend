using Jacmazon_ECommerce.Repositories;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.JWT;
using Jacmazon_ECommerce.ViewModels;
using NuGet.Protocol.Core.Types;
using System.IdentityModel.Tokens;
using Jacmazon_ECommerce.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net;

namespace Jacmazon_ECommerce.Services
{
    public class TokenService : ITokenService
    {
        private readonly ICRUDRepository<Token> _repository;
        private readonly IJWTSettings _jwtSettings;

        public TokenService(ICRUDRepository<Token> userRepository, IJWTSettings jwtSettings)
        {
            _repository = userRepository;
            _jwtSettings = jwtSettings;
        }

        public async Task<TokenViewModel> CreateTokenAsync(string email)
        {
            TokenViewModel tokenDto = new()
            {
                AccessToken = _jwtSettings.CreateAccessToken(email),
                RefreshToken = _jwtSettings.CreateRefreshToken(email),
                RefreshTokenExpiryDate = _jwtSettings.Refresh_Expired_Date()
            };

            Token token = new()
            {
                RefreshToken = tokenDto.RefreshToken,
                ExpiredDate = tokenDto.RefreshTokenExpiryDate,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            await _repository.CreateAsync(token);

            return tokenDto;
        }



        public async Task<Response<string>> UpdateRefreshTokenAsync(string refreshToken)
        {
            //JWT資訊
            JwtSecurityToken? jwtSecurityToken = _jwtSettings.ReadToken(refreshToken);

            //Token查詢
            List<Token> tokens = (await _repository.FindAsync(u => u.RefreshToken == refreshToken)).ToList();
            if (tokens.Count == 0 || jwtSecurityToken == null)
            {
                return new FailResponse404("查無此Token");
            }
            Token token = tokens[0];

            if (DateTime.Now > token.ExpiredDate)
            {
                return new FailResponse401("RefreshToken已過期");
            }

            //更新Table
            token.ExpiredDate = _jwtSettings.Refresh_Expired_Date();
            token.UpdatedDate = DateTime.Now;
            await _repository.UpdateAsync(token);

            //產生新的Access Token並回傳
            string email = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
            //string roleClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";

            string accessToken = _jwtSettings.CreateRefreshToken(email);

            return new OkResponse<string>(accessToken);
        }

        public async Task<bool> DeleteRefreshTokenAsync(string refreshToken)
        {
            //Token查詢
            Token? token = (await _repository.FindAsync(u => u.RefreshToken == refreshToken)).FirstOrDefault();

            if (token == null) return true; //裡面沒有token相當於已刪除
            else await _repository.DeleteAsync(token);

            return true;
        }
        
    }
}

