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

        public Response()
        {
        }

        //public Response(bool success)
        //{
        //    if (success)
        //    {
        //        Status = (int)HttpStatusCode.OK;
        //    }
        //    Success = success;
        //}

        //public Response<T> OkResponse(T data)
        //{
        //    return new Response<T>
        //    {
        //        Status = (int)HttpStatusCode.OK,
        //        Success = true,
        //        Data = data
        //    };
        //}

        //public Response<T> ErrorResponse(int httpStatusCode, string message)
        //{
        //    return new Response<T>
        //    {
        //        Status = httpStatusCode,
        //        Success = false,
        //        Message = message
        //    };
        //}

        //public Response<T> ErrorResponse(string message)
        //{
        //    return new Response<T>
        //    {
        //        Success = false,
        //        Message = message
        //    };
        //}
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
