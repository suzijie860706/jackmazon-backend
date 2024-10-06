using AutoMapper;
using Jacmazon_ECommerce.Models.LoginContext;
using Microsoft.AspNetCore.Mvc;

namespace Jacmazon_ECommerce.ViewModels
{
    [ModelMetadataType(typeof(TokenMetaData))]
    public class TokenViewModel
    {
        /// <summary>
        /// 存取令牌
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; } = null!;

        /// <summary>
        /// 刷新令牌到期時間
        /// </summary>
        public DateTime RefreshTokenExpiryDate { get; set; }
    }
}
