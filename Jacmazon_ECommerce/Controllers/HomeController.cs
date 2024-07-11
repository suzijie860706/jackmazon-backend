using Jacmazon_ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Jacmazon_ECommerce.Models;
using Microsoft.AspNetCore.OutputCaching;
using CalculatorService;
using ReverseService;
using WCFService;

namespace Jacmazon_ECommerce.Controllers
{
    [ResponseCache(CacheProfileName = "Default30")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            
        }

        public IActionResult Index()
        {
            return Redirect("/swagger/index.html");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
