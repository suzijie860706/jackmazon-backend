using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Jacmazon_ECommerce.Models;

namespace Jacmazon_ECommerce.ActionFilter
{
    /// <summary>
    /// ModelState驗證器
    /// </summary>
    public class ValidatorActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public ValidatorActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string? controllerName = filterContext.ActionDescriptor.RouteValues["controller"];
            string? actionName = filterContext.ActionDescriptor.RouteValues["action"];
            _logger.LogInformation("{Controller}/{Action} 參數驗證 Start",
                controllerName, actionName);


            var modelState = filterContext.ModelState;
            if (!modelState.IsValid)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var item in modelState)
                {
                    string key = item.Key;
                    string value = item.Value.Errors.FirstOrDefault()?.ErrorMessage ?? "";
                    dic.Add(key, value);
                }

                var response = new FailResponse400<Dictionary<string, string>>
                {
                    Success = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "驗證錯誤",
                    Data = dic
                };

                filterContext.Result = new OkObjectResult(response);
                _logger.LogInformation("{Controller}/{Action} 參數驗證 Error",
                controllerName, actionName);
            }
            else
            {
                _logger.LogInformation("{Controller}/{Action} 參數驗證 End",
                controllerName, actionName);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}
