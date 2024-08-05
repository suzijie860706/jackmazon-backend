using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Jacmazon_ECommerce.ActionFilter
{
    /// <summary>
    /// 紀錄Log發生位置
    /// </summary>
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
}
