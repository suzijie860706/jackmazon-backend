using System.Net;

namespace Jacmazon_ECommerce.Models
{
    /// <summary>
    /// 回傳格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T>
    {
        public bool Success { get; set; } = false;

        public int Status { get; set; }

        public string? Message { get; set; } = "";

        public T? Data { get; set; } = default;

        /// <summary>
        /// 回傳成功
        /// </summary>
        /// <param name="data"></param>
        public void SuccessResponse(T data)
        {
            Success = true;
            Status = (int)HttpStatusCode.OK;
            Message = "";
            Data = data;
        }
    }
}
