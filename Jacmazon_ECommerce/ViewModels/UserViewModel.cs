using AutoMapper;
using Jacmazon_ECommerce.Extensions;
using Jacmazon_ECommerce.Models.LoginContext;
using Microsoft.AspNetCore.Mvc;

namespace Jacmazon_ECommerce.ViewModels
{
    [ModelMetadataType(typeof(UserMetaData))]
    public class UserViewModel
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

    /// <summary>
    /// Mapper使用
    /// 務必設定屬性對映，避免SQL欄位名稱改變無法對應
    /// </summary>
    public class UserViewModelProfile : Profile
    {
        public UserViewModelProfile()
        {
            CreateMap<UserViewModel, User>()
                .IgnoreAllUnmapped()
                .ForMember(u => u.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(u => u.Password, opt => opt.MapFrom(src => src.Password));
        }
    }
}
