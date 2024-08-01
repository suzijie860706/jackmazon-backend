using Microsoft.AspNetCore.Antiforgery;
using System.Diagnostics;

namespace Jacmazon_ECommerce.Ｍiddlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAntiforgery antiforgery)
        {
            //// 在這裡處理請求前的邏輯
            //Debug.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");

            await _next(context);

            //string path = context.Request.Path.Value ?? "";

            ////進入SingIn時回傳Token
            //if (string.Equals(path, "/api/WebAPI", StringComparison.OrdinalIgnoreCase))
            //{
            //    var tokens = antiforgery.GetAndStoreTokens(context);
            //    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
            //        new CookieOptions() {
            //            HttpOnly = false, // 让前端 JavaScript 可以读取
            //            Secure = false, // 仅在 HTTPS 下发送
            //            //SameSite = SameSiteMode.Strict // 防止跨站请求伪造
            //        });
            //}


            //// 在這裡處理請求後的邏輯
            //Debug.WriteLine($"Response: {context.Response.StatusCode}");
        }
    }
}
