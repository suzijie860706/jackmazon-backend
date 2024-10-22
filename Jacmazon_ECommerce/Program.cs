using System.Text;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Jacmazon_ECommerce.JWT;
using Jacmazon_ECommerce.Data;
using Jacmazon_ECommerce.Services;
using Jacmazon_ECommerce.��iddlewares;
using Jacmazon_ECommerce.Repositories;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.Models.AdventureWorksLT2016Context;
using Serilog;
using Jacmazon_ECommerce.ViewModels;
using AutoMapper;
using Jacmazon_ECommerce.ActionFilter;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.Build.Framework;
using Microsoft.OpenApi.Any;

var builder = WebApplication.CreateBuilder(args);


#region Log
//�q appsettings.json Ū���]�w���
var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
        .Build();
//�ϥαq appsettings.json Ū���쪺���e�ӳ]�w logger
Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration).CreateLogger();

// �N Serilog ���U����x�O�����ѵ{��
builder.Host.UseSerilog();
#endregion

// Add services to the container.
builder.Services.AddControllers();

//����ModelState�۰ʦ^��400�L�o��
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddMvc(options =>
{
    //�۩w�q������Ҿ�
    options.Filters.Add(typeof(ValidatorActionFilter));
    //Log���������m
    options.Filters.Add(typeof(LoggingActionFilter));
});


//��Ʈw
builder.Services.AddDbContext<AdventureWorksLt2016Context>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("AdventureWorksLT2016")));
builder.Services.AddDbContext<LoginContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("Login")));

//Business Layer
builder.Services.AddScoped<ICRUDRepository<User>, CRUDRepository<User, LoginContext>>();
builder.Services.AddScoped<ICRUDRepository<Token>, CRUDRepository<Token, LoginContext>>();
builder.Services.AddScoped<ICRUDRepository<Product>, CRUDRepository<Product, AdventureWorksLt2016Context>>();

//Presentation Layer
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IHashingPassword, HashingPassword>();

//Common
builder.Services.AddTransient<IJWTSettings, JWTSettings>();
builder.Services.AddTransient<IValidationService, ValidationService>();

//AutoMapper DI Profile Class
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#region JWT�]�w
var key = Encoding.UTF8.GetBytes(IJWTSettings.Secret);
//�[�J����
builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //�w�]���Ҥ�� JWT ���
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //��Y�ӽШD���q�L���ҮɡA�t�η|�ͦ��@��Challenge�A�o�Ӥ�״N�O�ΥH�B�z�o��Challenge
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    // �����ҥ��ѮɡA�^�����Y�|�]�t WWW-Authenticate ���Y�A�o�̷|��ܥ��Ѫ��Բӿ��~��]
    x.IncludeErrorDetails = true; // �w�]�Ȭ� true�A���ɷ|�S�O����

    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,

        //��٦w�����_
        IssuerSigningKey = new SymmetricSecurityKey(key),
        //�ݭn�קﰵ���ҡA�קK����H�i�Ӵ���Token
        ValidateIssuer = false, //�O�_���ҽֵ֮o��?
        ValidateAudience = false, //���ǫȤ�i�H�ϥ�? 
        ValidIssuer = IJWTSettings.Issuer
    };
});
#endregion

//�^���֨�
builder.Services.AddResponseCaching();

//�s�WCORS
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

// �K�[ Antiforgery �A��
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

#region swagger
builder.Services.AddSwaggerGen(options =>
{
    //���Y�y�z
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jacmazon API",
        Description = $@"
An ASP.NET Core Web API for JacmazonAPI.  
Some useful links:
- [The JacmazonAPI repository](https://github.com/suzijie860706/Jackmazon_backend)
- [The source static API definition for the JacmazonAPI](https://suzijie860706.github.io/)",

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

    //�W�[Token�������
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

    //xml���W�[����
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    //�۩w�qoperationID
    options.CustomOperationIds(e =>
    {
        string methodName = e.HttpMethod?.ToLower() ?? "" + " ";
        if (methodName == "post") methodName = "";
        return $"{methodName}{e.ActionDescriptor.RouteValues["action"]}";
    });
    //�W�[���۩w�q���Ҥ��e
    options.SchemaFilter<SwagSchemaFilter>();

    //�Ұ�Filters
    options.ExampleFilters();

    
});
// �ҥ� Filters �A��
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
#endregion

//builder.Services.Configure
var app = builder.Build();

// ���� AutoMapper �t�m
var mapper = app.Services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();

#region �۩w�qMiddleware
app.UseCustomAuthorization(); //Customer JWT Validation Response
app.UseLogger(); //Logger
#endregion

// Configure the HTTP request pipeline.
//TODO:IIS�o��A�Ȯ�����
//if (app.Environment.IsDevelopment()) 
//{
app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        //c.InjectStylesheet("/swagger-ui/custom.css");
    });
//}

app.UseStaticFiles();

app.UseRouting();

//�[�JCORS
app.UseCors("AllowAllOrigins");

//�^���֨�
app.UseResponseCaching();
//�[�J����
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=index}/{id?}"
);

app.Run();
