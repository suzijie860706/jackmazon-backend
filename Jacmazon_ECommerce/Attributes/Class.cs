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

        public int minimumLength;
        public int maximumLength;
        /// <summary>錯誤訊息</summary>
        public string _errorMessage = string.Empty;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string email = value?.ToString() ?? "";
            string[] emailPart = email.Split('@');
            if (emailPart.Length == 2)
            {
                if (emailPart[0].Length < minimumLength || emailPart[0].Length > maximumLength)
                {
                    return new ValidationResult($"很抱歉，使用者名稱長度必須介於 {minimumLength} 到 {maximumLength} 個半形字元之間");
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
