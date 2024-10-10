using Jacmazon_ECommerce.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jacmazon_ECommerce.Controllers;
using Jacmazon_ECommerce.ViewModels;
using Jacmazon_ECommerce.Extensions;
using Jacmazon_ECommerce.Models;
using AutoMapper;
using Microsoft.AspNetCore.Antiforgery;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Jacmazon_ECommerce.Tests.Controllers
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class ProductControllerTests
    {
        private IProductService _productService;
        private ProductController _controller;

        [SetUp]
        public void SetUp()
        {
            _productService = Substitute.For<IProductService>();

            _controller = new ProductController(_productService);
        }

        [Test]
        public async Task List_WhenCalled_ReturnsOk()
        {
            //Arrange
            List<ProductViewModel> products = new List<ProductViewModel>()
            {
                new ProductViewModel {ProductId = 1},
            };

            _productService.GetAllProducts().Returns(products);

            //Act
            var okObjectResult = await _controller.List() as OkObjectResult;

            //Assert
            OkResponse<IEnumerable<ProductViewModel>>? responseData = okObjectResult?.Value as OkResponse<IEnumerable<ProductViewModel>>;

            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Success, Is.True);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(responseData.Data, Is.EqualTo(products));
        }
    }
}
