namespace Jacmazon_ECommerce.JWTServices
{
    public static class Settings
    {
        public static readonly string Secret = "Jacmazon_ECommerce20240321JackSu";

        public static readonly string Issuer = "http://localhost:5092/";

        /// <summary>
        /// 取得Refresh Token Expired Date
        /// </summary>
        public static DateTime Refresh_Expired_Date()
        {
            return DateTime.Now.AddSeconds(40);
        }
    }
}
