using Jacmazon_ECommerce.Interfaces;
using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Jacmazon_ECommerce.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Jacmazon_ECommerce.JWTServices;
using Microsoft.IdentityModel.Tokens;
using Jacmazon_ECommerce.Ｍiddlewares;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

//資料庫
builder.Services.AddDbContext<AdventureWorksLt2019Context>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("AdventureWorksLT2016")));
builder.Services.AddDbContext<LoginContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("Login")));

builder.Services.AddScoped<ICRUDRepository<ProductCategory>, ProductCategoryRepository>();
builder.Services.AddScoped<ICRUDRepository<Product>, ProductRepository>();

#region JWT設定
var key = Encoding.UTF8.GetBytes(Settings.Secret);
//加入驗證
builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //預設驗證方案 JWT 方案
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //當某個請求未通過驗證時，系統會生成一個Challenge，這個方案就是用以處理這個Challenge
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
    x.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,

        //對稱安全金鑰
        IssuerSigningKey = new SymmetricSecurityKey(key),
        //需要修改做驗證，避免任何人進來測試Token
        ValidateIssuer = false, //是否驗證誰核發的?
        ValidateAudience = false, //哪些客戶可以使用? 
        ValidIssuer = Settings.Issuer
    };
}).
AddCookie(option =>
{
    option.AccessDeniedPath = "/Shop/AccessDeny"; //拒絕
    option.LoginPath = "/Shop/Login"; //登入頁
}); ;
#endregion

builder.Services.AddScoped<ICRUDService<ProductCategory>,CRUDService<ProductCategory>>();
builder.Services.AddScoped<ICRUDService<Product>,CRUDService<Product>>();
//回應快取
builder.Services.AddResponseCaching();

//新增CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default30",
        new CacheProfile()
        {
            Duration = 30
        });
});

// 添加 Antiforgery 服務
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

//builder.Services.Configure
var app = builder.Build();

//antiforgeryToken
//將其寫在api上取得即可
//app.UseRequestLogging();

// 自定義JTW [Authorize] 未通過時Response
app.UseCustomAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

//加入CORS
app.UseCors("AllowAllOrigins");

//回應快取
app.UseResponseCaching();
//加入驗證
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Shop}/{action=login}/{id?}");

app.Run();
