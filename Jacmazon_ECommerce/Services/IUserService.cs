using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.ViewModels;

namespace Jacmazon_ECommerce.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 新增帳號
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<bool> CreateUserAsync(string email, string password);

        /// <summary>
        /// 驗證信箱註冊
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<bool> IsEmailNotRegisteredAsync(string email);

        /// <summary>
        /// 驗證電話註冊
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public Task<bool> IsPhoneNotRegisteredAsync(string phone);

        /// <summary>
        /// 驗證帳號密碼
        /// </summary>
        /// <param name="userViewModel"></param>
        /// <returns></returns>
        public Task<Response<string>> UserVerify(UserViewModel userViewModel);
    }
}
