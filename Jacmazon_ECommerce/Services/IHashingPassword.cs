namespace Jacmazon_ECommerce.Services
{
    public interface IHashingPassword
    {
        /// <summary>
        /// 加密密碼
        /// </summary>
        /// <param name="password">密碼</param>
        /// <param name="salt">鹽</param>
        /// <returns></returns>
        public string HashPassword(string password, byte[] salt);

        /// <summary>
        /// 隨機加鹽
        /// </summary>
        /// <returns></returns>
        public byte[] GenerateSalt();
    }
}
