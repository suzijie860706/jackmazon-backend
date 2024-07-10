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

        public async Task<IActionResult> Index()
        {
            //ASMX Test
            //CalculatorSoapClient calculatorSoapClient = new CalculatorSoapClient(CalculatorSoapClient.EndpointConfiguration.CalculatorSoap);

            //var add = await calculatorSoapClient.AddAsync(1, 3);


            //var total = new WebService3SoapClient(WebService3SoapClient.EndpointConfiguration.WebService3Soap);
            //var aa = await total.HelloWorldAsync();
            //TempData["add"] = aa.Body.HelloWorldResult;

            //WCF TEst
            CalculatorClient calculatorClient = new CalculatorClient();
            try
            {
                var total = calculatorClient.AddAsync(1, 5);

                TempData["add"] = total.Result;

                await calculatorClient.CloseAsync();
            }
            catch (Exception)
            {
                calculatorClient.Abort();
                throw;
            }
            

            return View();
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
