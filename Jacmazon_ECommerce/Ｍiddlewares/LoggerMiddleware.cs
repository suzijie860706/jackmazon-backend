using Jacmazon_ECommerce.Data;
using Jacmazon_ECommerce.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Net;
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

        public async Task InvokeAsync(HttpContext context)
        {
            // 在這裡處理請求前的邏輯
            try
            {
                await _next(context);
            }
            catch (Exception)
            {
                var response = new Response<string>
                {
                    Success = false,
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "伺服器發生錯誤，請稍後再試",
                };

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";   //add this line.....
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }
        }
    }
}
