using AutoMapper;
using Jacmazon_ECommerce.Services;
using Jacmazon_ECommerce.ViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.Extensions;

namespace Jacmazon_ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// 取得產品資料
        /// </summary>
        /// <returns></returns>
        [HttpGet("ProductList")]
        [Authorize]
        public async Task<IActionResult> ProductList()
        {
            IEnumerable<ProductViewModel> productResponseDtos = await _productService.GetAllProducts();

            return Ok(new Response<IEnumerable<ProductViewModel>>
            {
                Success = true,
                Status = StatusCodes.Status200OK,
                Data = productResponseDtos
            });

        }
    }
}
