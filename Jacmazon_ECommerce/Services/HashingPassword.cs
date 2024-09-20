using Microsoft.AspNetCore.Identity;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Jacmazon_ECommerce.Services
{
    public class HashingPassword : IHashingPassword
    {
        public string HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                //將密碼轉成byte數組
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                //初始化數組長度
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                //將密碼覆蓋到數組起始
                Buffer.BlockCopy(passwordBytes, 0, salt, 0, passwordBytes.Length);
                //將salt覆蓋到數組的密碼結尾處
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                //SHA256加密
                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                //
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public byte[] GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[16];
                rng.GetBytes(bytes);
                return bytes;
            }
        }
    }
}
