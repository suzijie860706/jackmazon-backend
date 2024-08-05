using NSubstitute;
using System.Linq.Expressions;
using Jacmazon_ECommerce;
using Jacmazon_ECommerce.Controllers;
using Jacmazon_ECommerce.Repositories;
using Jacmazon_ECommerce.Data;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using AutoMapper;
using Jacmazon_ECommerce.ViewModels;
using Microsoft.AspNetCore.Http;
using Jacmazon_ECommerce.Models;
using NSubstitute.ExceptionExtensions;
using Elfie.Serialization;
using NSubstitute.ClearExtensions;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Jacmazon_ECommerce.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class Tests : PageTest
    {
        private IAntiforgery _antiforgery;
        private ILogger<WebAPIController> _logger;
        private IUserService _userService;
        private ITokenService _tokenService;
        private IProductService _productService;
        private IMapper _mapper;


        private WebAPIController _controller;
        private MapperConfiguration config;

        [SetUp]
        public void SetUp()
        {
            _antiforgery = Substitute.For<IAntiforgery>();
            _logger = Substitute.For<ILogger<WebAPIController>>();
            _userService = Substitute.For<IUserService>();
            _tokenService = Substitute.For<ITokenService>();
            _productService = Substitute.For<IProductService>();

            config = new MapperConfiguration(cfg =>
            {
                // 手動添加所有的 AutoMapper Profile
                cfg.AddProfile(new UserViewModelProfile());
            });

            _mapper = config.CreateMapper();

            _controller = new WebAPIController(_antiforgery, _logger, _userService, _tokenService, _productService, _mapper);
        }


        /// <summary>
        /// 驗證Mapper Config
        /// </summary>
        [Test]
        public void MapperConfiguration_ShouldBeValid()
        {
            //Arrange

            //Act
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();

            //Assert
            Assert.Pass();
        }

        ///// <summary>
        ///// RefreshToken Http Response回傳OK
        ///// </summary>
        ///// <returns></returns>
        //[Test]
        //public async Task RefreshToken_WhenCalled_ReturnsOkResult()
        //{
        //    //Arrange
        //    string refreshToken = "validToken";
        //    var response = new Response<string> { Success = true, Data = "newAccessToken", Status = (int)HttpStatusCode.OK };
        //    _tokenService.UpdateRefreshTokenAsync(refreshToken).Returns(response);
            
        //    //Act
        //    var result = await _controller.RefreshToken(refreshToken);

        //    //Assert
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOf<OkObjectResult>(result);
        //}

        /// <summary>
        /// Refresh Token回傳字串
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task RefreshToken_WhenCalled_ReturnsToken()
        {
            //Arrange
            string refreshToken = "validToken";
            var response = new Response<string> { Success = true, Data = "newAccessToken", Status = (int)HttpStatusCode.OK };
            _tokenService.UpdateRefreshTokenAsync(refreshToken).Returns(response);
            //Act
            var okObjectResult = await _controller.RefreshToken(refreshToken) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.IsNotNull(responseData);
            Assert.IsTrue(responseData.Success);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(responseData.Data, Is.EqualTo(response.Data));
        }

        //[Test]
        //public async Task GetLogs_ReturnsLogs_WhenLogFileExists()
        //{
        //    // Arrange
        //    string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Serilogs", $"log-{DateTime.Now.ToString("yyyyMMdd")}.txt");
        //    Directory.CreateDirectory(Path.GetDirectoryName(testFilePath));
        //    await File.WriteAllLinesAsync(testFilePath, new[] { "Log1", "Log2" });

        //    // Act
        //    var result = await _controller.GetLogs() as OkObjectResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(200, result.StatusCode);
        //    var logs = result.Value as string[];
        //    Assert.AreEqual(2, logs.Length);
        //    Assert.AreEqual("Log1", logs[0]);
        //    Assert.AreEqual("Log2", logs[1]);

        //    // Clean up
        //    File.Delete(testFilePath);
        //}

        //[Test]
        //public async Task GetLogs_ReturnsNotFound_WhenLogFileDoesNotExist()
        //{
        //    // Arrange
        //    string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Serilogs", $"log-{DateTime.Now.ToString("yyyyMMdd")}.txt");
        //    if (File.Exists(testFilePath))
        //    {
        //        File.Delete(testFilePath);
        //    }

        //    // Act
        //    var result = await _controller.GetLogs() as NotFoundObjectResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(404, result.StatusCode);
        //    Assert.AreEqual("Log file not found.", result.Value);
        //}

        //[Test]
        //public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
        //{
        //    await Page.GotoAsync("https://playwright.dev");

        //    // Expect a title "to contain" a substring.
        //    await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

        //    // create a locator
        //    var getStarted = Page.Locator("text=Get Started");

        //    // Expect an attribute "to be strictly equal" to the value.
        //    await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        //    // Click the get started link.
        //    await getStarted.ClickAsync();

        //    // Expects the URL to contain intro.
        //    await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
        //}
    }
}
