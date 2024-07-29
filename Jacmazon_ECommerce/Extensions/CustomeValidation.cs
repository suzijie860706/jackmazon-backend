using Jacmazon_ECommerce.Data;
using Jacmazon_ECommerce.Models.LoginContext;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Jacmazon_ECommerce.Extensions
{
    /// <summary>
    /// Email驗證格式
    /// </summary>
    public class EmailValidateAttribute : ValidationAttribute
    {
        /// <summary>錯誤訊息</summary>
        public string _errorMessage = string.Empty;
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            string? email = value.ToString() ?? "";
            //驗證格式
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ValidationResult(_errorMessage);
            }

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }

                if (Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(_errorMessage);
                }
            }
            catch (Exception)
            {
                throw;
            }
            //return base.IsValid(value, validationContext);
        }
    }

    /// <summary>
    /// Phone驗證格式
    /// </summary>
    public class PhoneValidateAttribute : ValidationAttribute
    {
        /// <summary>錯誤訊息</summary>
        public string _errorMessage = string.Empty;
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            string? phone = value.ToString() ?? "";
            //驗證格式
            if (string.IsNullOrWhiteSpace(phone))
            {
                return new ValidationResult(_errorMessage);
            }

            try
            {
                //格式驗證
                if (!Regex.IsMatch(phone, @"^09[0-9]{8}$"))
                {
                    return new ValidationResult(_errorMessage);
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            catch (Exception)
            {
                throw;
            }
            //return base.IsValid(value, validationContext);
        }
    }
}
