using Jacmazon_ECommerce.Data;
using Jacmazon_ECommerce.Models;
using Microsoft.AspNetCore.Antiforgery;
using System.Diagnostics;
using System.Text.Json;

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
            }
            catch (Exception ex)
            {
                // 紀錄logger
                Type exceptionType = ex.GetType();
                logger.LogError(ex, $"應用程式內部錯誤。錯誤類型：{exceptionType.Name} \n" +
                    $"錯誤訊息：{ex}");
                
                var response = new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = ""
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
