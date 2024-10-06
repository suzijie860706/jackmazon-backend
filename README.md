
# JacmazonAPI 商務網站API
專案以常見的商業購物網站為目標，實作後端API功能。

靜態API文件參考：https://suzijie860706.github.io/

![App Screenshot](https://imgur.com/a/HjU2Roz)

專案目標：  
- 使用三層式分層設計分離關注點與職責，便於日後維後與管理
- 實作會員登入JWT驗證
- 使用NUNIT單元測試
- 運用Swagger產生文件

## 使用技術  
ActionFilter：  
自定義Log與ModelStateValid，將Action中用於驗證與Log的部分抽離出來，並自定義回傳前端訊息
```
public class LoggingActionFilter : IActionFilter
{
    private readonly ILogger<LoggingActionFilter> _logger;

    public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
    {
        _logger = logger;
    }
    public void OnActionExecuting(ActionExecutingContext filterContext)
    {
        string? controllerName = filterContext.ActionDescriptor.RouteValues["controller"];
        string? actionName = filterContext.ActionDescriptor.RouteValues["action"];

        _logger.LogInformation("{Controller}/{Action} Start",
            controllerName, actionName);
    }

    public void OnActionExecuted(ActionExecutedContext filterContext)
    {
        string? controllerName = filterContext.ActionDescriptor.RouteValues["controller"];
        string? actionName = filterContext.ActionDescriptor.RouteValues["action"];
        if (filterContext.Exception == null)
        {
            _logger.LogInformation("{Controller}/{Action} End",
            controllerName, actionName);
        }
        else
        {
            _logger.LogError(filterContext.Exception, "{Controller}/{Action} Fail",
            controllerName, actionName);
        }
    }
}
```
DI：  
將類別之間的耦合分離，方便抽換功能與測試實作
```
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
```

Middleware：  
前端驗證失敗的訊息可以在這裡快速處理並回傳，降低伺服器負擔，並且在偵測到伺服器因各種原因報錯時，統一回傳Internal Error到前端。
```
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
```