using System.Transactions;

namespace Jacmazon_ECommerce.Models
{
    public class UserTable
    {
        public int UserId { get; set; }

        public string? UserName { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string? Password { get; set;}

        /// <summary>
        /// 電子信箱
        /// </summary>
        public string? Email{ get; set; }
    }
}
