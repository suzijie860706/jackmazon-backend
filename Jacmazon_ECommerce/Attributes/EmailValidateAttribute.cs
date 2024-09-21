using Jacmazon_ECommerce.Data;
using Jacmazon_ECommerce.Services;
using Jacmazon_ECommerce.Models.LoginContext;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Jacmazon_ECommerce.Attributes
{
    /// <summary>
    /// Email驗證格式
    /// </summary>
    public class EmailValidateAttribute : ValidationAttribute
    {
        /// <summary>錯誤訊息</summary>
        public string _errorMessage = string.Empty;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var service = (IValidationService?)validationContext.GetService(typeof(IValidationService));
            if (service == null) throw new NullReferenceException();

            string email = value?.ToString() ?? "";

            bool isValid = service.IsValidEmail(email);
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
