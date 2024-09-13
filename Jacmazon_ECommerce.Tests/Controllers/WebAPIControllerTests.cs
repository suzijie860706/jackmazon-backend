using NSubstitute;
using Jacmazon_ECommerce.Controllers;
using Jacmazon_ECommerce.Services;
using Jacmazon_ECommerce.ViewModels;
using Jacmazon_ECommerce.Extensions;
using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Models.AdventureWorksLT2016Context;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Net;

namespace Jacmazon_ECommerce.Tests.Controllers
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class Tests : PageTest
    {
        private IAntiforgery _antiforgery;
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
            _userService = Substitute.For<IUserService>();
            _tokenService = Substitute.For<ITokenService>();
            _productService = Substitute.For<IProductService>();

            config = new MapperConfiguration(cfg =>
            {
                // 手動添加所有的 AutoMapper Profile
                cfg.AddProfile(new UserViewModelProfile());
            });

            _mapper = config.CreateMapper();

            _controller = new WebAPIController(_antiforgery, _userService, _tokenService, _productService, _mapper);
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

        }

        [Test]
        public async Task Login_WhenCalled_ReturnsOk()
        {
            //Arrange
            UserViewModel userViewModel = new()
            {
                Email = "email",
                Password = "password"
            };
            _userService.VerifyUserLogin(userViewModel.Email, userViewModel.Password).Returns(true);

            TokenViewModel tokenViewModel = new()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken"
            };
            _tokenService.CreateTokenAsync(userViewModel.Email).Returns(Task.FromResult(tokenViewModel));

            //Act
            var okObjectResult = await _controller.Login(userViewModel) as OkObjectResult;

            //Assert
            Response<TokenViewModel>? responseData = okObjectResult?.Value as Response<TokenViewModel>;

            Assert.IsNotNull(responseData);
            Assert.IsTrue(responseData.Success);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(responseData.Data, Is.EqualTo(tokenViewModel));
        }

        [Test]
        public async Task Login_WhenCalled_ReturnsFail()
        {
            //Arrange
            UserViewModel userViewModel = new()
            {
                Email = "email",
                Password = "password"
            };
            _userService.VerifyUserLogin(userViewModel.Email, userViewModel.Password).Returns(false);

            //Act
            var okObjectResult = await _controller.Login(userViewModel) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.IsNotNull(responseData);
            Assert.IsFalse(responseData.Success);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(responseData.Message, Is.EqualTo("帳號或密碼錯誤"));
        }


        [Test]
        public async Task CreateAccount_WhenCalled_ReturnsOk()
        {
            //Arrange
            UserViewModel userViewModel = new()
            {
                Email = "email",
                Password = "password"
            };

            _userService.IsEmailRegisteredAsync(userViewModel.Email).Returns(false);

            //Act
            var okObjectResult = await _controller.CreateAccount(userViewModel) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.IsNotNull(responseData);
            Assert.IsTrue(responseData.Success);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task CreateAccount_WhenCalled_ReturnsFail()
        {
            //Arrange
            UserViewModel userViewModel = new()
            {
                Email = "email",
                Password = "password"
            };

            _userService.IsEmailRegisteredAsync(userViewModel.Email).Returns(Task.FromResult(true));

            //Act
            var okObjectResult = await _controller.CreateAccount(userViewModel) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Success, Is.False);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(responseData.Message, Is.EqualTo("帳號已建立"));
        }

        [Test]
        public async Task ProductList_WhenCalled_ReturnsOk()
        {
            //Arrange
            List<ProductViewModel> products = new List<ProductViewModel>()
            {
                new ProductViewModel {ProductId = 1},
            };

            _productService.GetAllProducts().Returns(products);

            //Act
            var okObjectResult = await _controller.ProductList() as OkObjectResult;

            //Assert
            Response<IEnumerable<ProductViewModel>>? responseData = okObjectResult?.Value as Response<IEnumerable<ProductViewModel>>;

            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Success, Is.True);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(responseData.Data, Is.EqualTo(products));
        }

        [Test]
        public async Task Logout_WhenCalled_ReturnsOk()
        {
            //Arrange
            string refreshToken = "refreshToken";

            _tokenService.DeleteRefreshTokenAsync(refreshToken).Returns(true);

            //Act
            var okObjectResult = await _controller.Logout(refreshToken) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Success, Is.True);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Logout_WhenCalled_ReturnsFail()
        {
            //Arrange
            string refreshToken = "refreshToken";

            _tokenService.DeleteRefreshTokenAsync(refreshToken).Returns(false);

            //Act
            var okObjectResult = await _controller.Logout(refreshToken) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Success, Is.False);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(responseData.Message, Is.EqualTo("查無此Token"));
        }

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

        [Test]
        public async Task Email_WhenCalled_ReturnsOk()
        {
            //Arrange
            string email = "email@gmail.com";

            _userService.IsEmailRegisteredAsync(email).Returns(false);

            //Act
            var okObjectResult = await _controller.Email(email) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Success, Is.True);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Email_WhenCalled_ReturnsOk()
        {
            //Arrange
            string email = "email@gmail.com";

            _userService.IsEmailRegisteredAsync(email).Returns(true);

            //Act
            var okObjectResult = await _controller.Email(email) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Success, Is.False);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(responseData.Message, Is.EqualTo("電子信箱已註冊"));
        }
    }
}
