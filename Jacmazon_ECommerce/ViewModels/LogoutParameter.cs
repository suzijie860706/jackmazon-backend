using Jacmazon_ECommerce.Models.LoginContext;
using Microsoft.AspNetCore.Mvc;

namespace Jacmazon_ECommerce.ViewModels
{
    [ModelMetadataType(typeof(TokenMetaData))]
    public class LogoutParameter
    {
        /// <summary>
        /// 長期權杖
        /// </summary>
        public string RefreshToken { get; set; } = null!;
    }
}
