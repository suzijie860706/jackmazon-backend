using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Jacmazon_ECommerce.Extensions
{
    public class PagerViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(PagedResultBase result)
        {
            return Task.FromResult((IViewComponentResult)View("Default", result));
        }
    }
}
