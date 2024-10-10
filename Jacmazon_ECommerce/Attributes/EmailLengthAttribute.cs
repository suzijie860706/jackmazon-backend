using Jacmazon_ECommerce.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace Jacmazon_ECommerce.Attributes
{
    /// <summary>
    /// Email驗證格式
    /// </summary>
    public class EmailLengthAttribute : ValidationAttribute
    {

        private int _minimumLength;
        private int _maximumLength;
        /// <summary>錯誤訊息</summary>
        public string _errorMessage = string.Empty;

        public EmailLengthAttribute(int minimumLength, int maximumLength)
        {
            _minimumLength = minimumLength;
            _maximumLength = maximumLength;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string email = value?.ToString() ?? "";
            string[] emailPart = email.Split('@');
            if (emailPart.Length == 2)
            {
                if (emailPart[0].Length < _minimumLength || emailPart[0].Length > _maximumLength)
                {
                    return new ValidationResult($"很抱歉，使用者名稱長度必須介於 {_minimumLength} 到 {_maximumLength} 個半形字元之間");
                }
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Email格式錯誤");
            }
        }
    }
}
