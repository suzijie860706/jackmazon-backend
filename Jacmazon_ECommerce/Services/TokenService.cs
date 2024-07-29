using Jacmazon_ECommerce.Repositories;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.JWT;
using Jacmazon_ECommerce.DTOs;
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
        public TokenService(ICRUDRepository<Token> userRepository)
        {
            _repository = userRepository;
        }

        public async Task<TokenResponseDto> CreateTokenAsync(string email)
        {
            TokenResponseDto tokenDto = new()
            {
                AccessToken = Settings.CreateAccessToken(email),
                RefreshToken = Settings.CreateRefreshToken(email),
                RefreshTokenExpiryDate = Settings.Refresh_Expired_Date()
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
            Response<string> response = new();

            //JWT資訊
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtSecurityToken = tokenHandler.ReadToken(refreshToken) as JwtSecurityToken;

            //Token查詢
            Token? token = (await _repository.FindAsync(u => u.RefreshToken == refreshToken)).FirstOrDefault() ?? null;
            if (token == null || jwtSecurityToken == null)
            {
                response.Status = (int)HttpStatusCode.Unauthorized;
                response.Message = "查無此Token";
                return response;
            }

            if (DateTime.Now > token.ExpiredDate)
            {
                response.Status = (int)HttpStatusCode.Unauthorized;
                response.Message = "RefreshToken已過期";
                return response;
            }

            //更新Table
            token.ExpiredDate = Settings.Refresh_Expired_Date();
            token.UpdatedDate = DateTime.Now;
            await _repository.UpdateAsync(token);

            //產生新的Access Token並回傳
            string email = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
            //string roleClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";

            string accessToken = Settings.CreateRefreshToken(email);

            response.SuccessResponse(accessToken);
            return response;
        }

        public async Task<Response<string>> DeleteRefreshTokenAsync(string refreshToken)
        {
            Response<string> response = new();

            //Token查詢
            Token? token = (await _repository.FindAsync(u => u.RefreshToken == refreshToken)).FirstOrDefault() ?? null;
            if (token == null)
            {
                response.Status = (int)HttpStatusCode.Unauthorized;
                response.Message = "查無此Token";
                return response;
            }

            await _repository.DeleteAsync(token.Id);

            response.SuccessResponse("");
            return response;
        }
        
    }
}

