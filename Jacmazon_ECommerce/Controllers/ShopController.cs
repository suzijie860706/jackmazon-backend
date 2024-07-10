using Jacmazon_ECommerce.Extensions;
using Jacmazon_ECommerce.Interfaces;
using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace Jacmazon_ECommerce.Controllers
{
    public class ShopController : Controller
    {
        private readonly AdventureWorksLt2019Context _context;
        private readonly IConfiguration _configuration;
        private readonly ICRUDService<ProductCategory> _productcategory;
        private readonly ICRUDService<Product> _product;

        public ShopController(AdventureWorksLt2019Context context,
            IConfiguration configuration,
            ICRUDService<ProductCategory> productcategory,
            ICRUDService<Product> product)
        {
            _context = context;
            _configuration = configuration;
            _productcategory = productcategory;
            _product = product;
        }

        [HttpGet]
        public ActionResult ProductCategoryList(int currentPage = 1)
        {
            return View(_productcategory.GetAll(currentPage));
        }

        [HttpGet]
        public ActionResult ProductCategoryDetail(int? id)
        {
            var data = _productcategory.Detail(id);
            if (data == null) return NotFound();

            return View(data);
        }

        //[HttpGet]
        //public ActionResult ProductList(int? id, int page = 1)
        //{
        //    PagedResult<Product> data = new PagedResult<Product>(_context, _configuration)
        //    {
        //        Results = _product.GetAll(page),
        //        CurrentPage = page
        //    };

        //    if (data == null) return NotFound();

        //    return View(data);
        //}

        [HttpGet]
        [OutputCache(Duration = 30)]
        public ActionResult ProductList(int? id, int page = 1)
        {
            PagedResult<Product> data = new PagedResult<Product>(_context, _configuration)
            {
                Results = id.HasValue ? _product.GetCategoryList(id) : _product.GetAll(page),
                CurrentPage = page
            };
            return View(nameof(ProductList), data);
        }

        [HttpGet]
        [OutputCache(Duration = 30)]
        public ActionResult ProductList_EndlessScroll(int? id)
        {
            int page = 1;
            PagedResult<Product> data = new PagedResult<Product>(_context, _configuration)
            {
                Results = id.HasValue ? _product.GetCategoryList(id) : _product.GetAll(page),
                CurrentPage = page
            };
            return View(nameof(ProductList_EndlessScroll), data);
        }

        [HttpPost]
        public IEnumerable<Product> GetProductData(int? pageIndex)
        {
            IEnumerable<Product> data = _product.GetAll(Convert.ToInt32(pageIndex));
            return data;
        }


        //改由透過WebApi呼叫
        //public FileContentResult? GetImage(int? id)
        //{
        //    Product? product = _context.Products.Find(id);
        //    if (product != null && product.ThumbNailPhoto != null)
        //    {
        //        return File(product.ThumbNailPhoto, "image/" + Path.GetExtension(product.ThumbnailPhotoFileName));
        //    }
        //    else return null;
        //}


        // GET: ShopController1
        [HttpGet]
        [OutputCache(Duration = 30)]
        public ActionResult Index()
        {
            //練習Json格式
            //string jsonData = JsonConvert.SerializeObject(_context.Products.ToList());
            //return Content(jsonData);

            //測試WebApi

            return View();
        }

        // GET: ShopController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// 登入頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 拒絕頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult AccessDeny()
        {
            return View();
        }

        // GET: ShopController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShopController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ShopController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ShopController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ShopController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ShopController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
