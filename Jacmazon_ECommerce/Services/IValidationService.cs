namespace Jacmazon_ECommerce.Services
{
    public interface IValidationService
    {
        /// <summary>
        /// Email驗證格式
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsValidEmail(string email);

        /// <summary>
        /// Phone驗證格式
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public bool IsValidPhone(string phone);
    }
}
