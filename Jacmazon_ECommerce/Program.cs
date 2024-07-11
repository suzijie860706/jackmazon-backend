using Jacmazon_ECommerce.Interfaces;
using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Models.AdventureWorksLT2016Context;
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
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

//資料庫
builder.Services.AddDbContext<AdventureWorksLt2016Context>(option =>
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
});
#endregion

builder.Services.AddScoped<ICRUDService<ProductCategory>, CRUDService<ProductCategory>>();
builder.Services.AddScoped<ICRUDService<Product>, CRUDService<Product>>();
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

#region swagger
builder.Services.AddSwaggerGen(options =>
{
    //表頭描述
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jacmazon API",
        Description = "An ASP.NET Core Web API for managing Jacmazon",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    //xml文件增加註解
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    //增加Token驗證欄位
    options.AddSecurityDefinition("Bearer",
    new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization"
    });

    options.AddSecurityRequirement(
       new OpenApiSecurityRequirement
       {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
       });

});
#endregion


//builder.Services.Configure
var app = builder.Build();

//antiforgeryToken
//將其寫在api上取得即可
//app.UseRequestLogging();

// 自定義JTW [Authorize] 未通過時Response
app.UseCustomAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
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
    pattern: "{controller=home}/{action=index}/{id?}"
);

app.Run();
