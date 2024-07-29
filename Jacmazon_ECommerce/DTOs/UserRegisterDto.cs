using Jacmazon_ECommerce.Models.LoginContext;
using Microsoft.AspNetCore.Mvc;

namespace Jacmazon_ECommerce.DTOs
{
    [ModelMetadataType(typeof(UserMetaData))]
    public class UserRegisterDto
    {
        /// <summary>
        /// 電子信箱
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; } = null!;
    }
}
