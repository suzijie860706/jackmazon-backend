using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Jacmazon_ECommerce.Services
{
    public class ValidationService : IValidationService
    {
        /// <summary>
        /// Email驗證格式
        /// </summary>
        public bool IsValidEmail(string email)
        {
            //驗證格式
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
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
                    return true;
                }
                else
                {
                    return false;
                }
            }

            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Phone驗證格式
        /// </summary>
        public bool IsValidPhone(string phone)
        {
            //驗證格式
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            try
            {
                //格式驗證
                if (!Regex.IsMatch(phone, @"^09[0-9]{8}$"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

