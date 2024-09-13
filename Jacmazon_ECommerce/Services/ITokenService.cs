using Jacmazon_ECommerce.ViewModels;
using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Models.LoginContext;
using NuGet.Protocol.Core.Types;
using System.Linq.Expressions;
using System.Reflection.Metadata;

namespace Jacmazon_ECommerce.Services
{
    public interface ITokenService
    {
        /// <summary>
        /// 新增JWT Token
        /// </summary>
        /// <param name="email"></param>
        /// <returns>TokenResponseDto Entity</returns>
        public Task<TokenViewModel> CreateTokenAsync(string email);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public Task<Response<string>> UpdateRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteRefreshTokenAsync(string refreshToken);
    }
}
