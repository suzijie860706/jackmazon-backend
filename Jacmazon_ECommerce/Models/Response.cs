using System.Net;
using System.Net.Mail;

namespace Jacmazon_ECommerce.Models
{
    /// <summary>
    /// 回傳格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T>
    {
        public bool Success { get; set; } = false;

        public int? Status { get; set; } = null;

        public string? Message { get; set; } = "";

        public T? Data { get; set; } = default;
    }

    public class OkResponse : Response<string>
    {
        public OkResponse()
        {
            Success = true;
            Status = (int)HttpStatusCode.OK;
        }
    }

    public class OkResponse<T> : Response<T>
    {
        public OkResponse()
        {
            Success = true;
            Status = (int)HttpStatusCode.OK;
        }

        public OkResponse(T data)
        {
            Success = true;
            Status = (int)HttpStatusCode.OK;
            Data = data;
        }
    }

    public class FailResponse401 : Response<string>
    {
        public FailResponse401(string errorMessage)
        {
            Success = false;
            Status = (int)HttpStatusCode.Unauthorized;
            Message = errorMessage;
        }
    }

    public class FailResponse400 : Response<string>
    {
        public FailResponse400(string errorMessage)
        {
            Success = false;
            Status = (int)HttpStatusCode.BadRequest;
            Message = errorMessage;
        }
    }

    public class FailResponse500 : Response<string>
    {
        public FailResponse500()
        {
            Success = false;
            Status = (int)HttpStatusCode.InternalServerError;
        }
    }
}
