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
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
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

                var response = new Response<Dictionary<string, string>>
                {
                    Success = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "驗證錯誤",
                    Data = dic
                };

                filterContext.Result = new OkObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}
