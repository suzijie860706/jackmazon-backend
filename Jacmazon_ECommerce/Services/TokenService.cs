﻿using Jacmazon_ECommerce.Repositories;
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
            Response<string> response = new();

            //JWT資訊
            JwtSecurityToken? jwtSecurityToken = _jwtSettings.ReadToken(refreshToken);

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
            token.ExpiredDate = _jwtSettings.Refresh_Expired_Date();
            token.UpdatedDate = DateTime.Now;
            await _repository.UpdateAsync(token);

            //產生新的Access Token並回傳
            string email = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
            //string roleClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";

            string accessToken = _jwtSettings.CreateRefreshToken(email);

            response.SuccessResponse(accessToken);
            return response;
        }

        public async Task<bool> DeleteRefreshTokenAsync(string refreshToken)
        {
            //Token查詢
            Token? token = (await _repository.FindAsync(u => u.RefreshToken == refreshToken)).FirstOrDefault() ?? null;

            if (token == null) return false;
            else await _repository.DeleteAsync(token);

            return true;
        }
        
    }
}

