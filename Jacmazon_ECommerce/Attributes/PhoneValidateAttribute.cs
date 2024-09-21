using Jacmazon_ECommerce.Services;
using System.ComponentModel.DataAnnotations;

namespace Jacmazon_ECommerce.Attributes
{
    /// <summary>
    /// Phone驗證格式
    /// </summary>
    public class PhoneValidateAttribute : ValidationAttribute
    {
        /// <summary>錯誤訊息</summary>
        public string _errorMessage = string.Empty;
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var service = (IValidationService?)validationContext.GetService(typeof(IValidationService));
            if (service == null) throw new NullReferenceException();

            string phone = value?.ToString() ?? "";

            bool isValid = service.IsValidPhone(phone);
            if (!isValid)
            {
                return new ValidationResult(_errorMessage);
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
