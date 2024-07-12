using Jacmazon_ECommerce.Data;
using Microsoft.AspNetCore.Antiforgery;
using System.Diagnostics;

namespace Jacmazon_ECommerce.Ｍiddlewares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, LoginContext loginContext,ILogger<LoggerMiddleware> logger)
        {
            // 在這裡處理請求前的邏輯
            //Debug.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");

            try
            {
                await _next(context);

                if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
                {
                    //var errorMessage = "An internal server error occurred.";
                    //_logger.LogError($"HTTP STATUS CODE: {context.Response.StatusCode}, Message: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常，记录错误日志
                logger.LogError(ex, $"An unhandled exception occurred: {ex.Message}");
                //await HandleExceptionAsync(context, ex);
            }



            if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
            {
                //logger.LogInformation("This is an information message.");
                //logger.LogWarning("This is a warning message.");
                //logger.LogError("HTTP STATUS CODE:" + context.Response.StatusCode);
            }


            // 在這裡處理請求後的邏輯
            //Debug.WriteLine($"Response: {context.Response.StatusCode}");
        }
    }
}
