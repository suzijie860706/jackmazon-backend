using Jacmazon_ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        //[HttpGet("getImage/{id}")]
        //public FileContentResult? GetImage(int? id)
        //{
        //    Product? product = _context.Products.Find(id);
        //    if (product != null && product.ThumbNailPhoto != null)
        //    {
        //        return File(product.ThumbNailPhoto, "image/" + Path.GetExtension(product.ThumbnailPhotoFileName));
        //    }
        //    else return null;
        //}
    }
}
