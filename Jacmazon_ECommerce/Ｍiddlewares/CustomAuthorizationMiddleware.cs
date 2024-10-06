using Azure.Core;
using Jacmazon_ECommerce.JWT;
using Jacmazon_ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Jacmazon_ECommerce.Models;

namespace Jacmazon_ECommerce.Ｍiddlewares
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;



        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;

        }

        public async Task InvokeAsync(HttpContext context, LoginContext _loginContext)
        {
            

            string WWW_Authenticate = context.Response.Headers.WWWAuthenticate.ToString();

            //使用Authorize
            if (WWW_Authenticate.Contains("Bearer"))
            {
                //回傳token
                var response = new FailResponse401("Authorization failed. Access denied.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }

            await _next(context);
        }

        // Call the next middleware in the pipeline
    }
}

