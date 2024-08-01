using AutoMapper;
using Jacmazon_ECommerce.Models.LoginContext;
using Microsoft.AspNetCore.Mvc;

namespace Jacmazon_ECommerce.ViewModels
{
    [ModelMetadataType(typeof(TokenMetaData))]
    public class TokenViewModel
    {
        /// <summary>
        /// 訪問權杖
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// 長期權杖
        /// </summary>
        public string RefreshToken { get; set; } = null!;

        /// <summary>
        /// 長期權杖到期時間
        /// </summary>
        public DateTime RefreshTokenExpiryDate { get; set; }
    }
}
