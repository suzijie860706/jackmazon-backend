using NSubstitute;
using Jacmazon_ECommerce.Controllers;
using Jacmazon_ECommerce.Services;
using Jacmazon_ECommerce.ViewModels;
using Jacmazon_ECommerce.Extensions;
using Jacmazon_ECommerce.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Net;
using Microsoft.IdentityModel.Protocols.WsTrust;

namespace Jacmazon_ECommerce.Tests.Controllers
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class UsersControllerTests : PageTest
    {
        private IAntiforgery _antiforgery;
        private IUserService _userService;
        private IValidationService _validationService;
        private ITokenService _tokenService;
        private IMapper _mapper;


        private UserController _controller;
        private MapperConfiguration config;

        [SetUp]
        public void SetUp()
        {
            _antiforgery = Substitute.For<IAntiforgery>();
            _validationService = Substitute.For<IValidationService>();
            _userService = Substitute.For<IUserService>();
            _tokenService = Substitute.For<ITokenService>();

            config = new MapperConfiguration(cfg =>
            {
                // 手動添加所有的 AutoMapper Profile
                cfg.AddProfile(new UserViewModelProfile());
            });

            _mapper = config.CreateMapper();

            _controller = new UserController(_antiforgery, _userService, _validationService, _tokenService, _mapper);
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
            UserParameter userViewModel = new()
            {
                Email = "email",
                Password = "password"
            };
            _userService.UserVerify(userViewModel).Returns(new OkResponse());

            TokenViewModel tokenViewModel = new()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken"
            };
            _tokenService.CreateTokenAsync(userViewModel.Email).Returns(Task.FromResult(tokenViewModel));

            //Act
            var okObjectResult = await _controller.Login(userViewModel) as OkObjectResult;

            //Assert
            OkResponse<TokenViewModel>? responseData = okObjectResult?.Value as OkResponse<TokenViewModel>;

            Assert.IsNotNull(responseData);
            Assert.IsTrue(responseData.Success);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(responseData.Data, Is.EqualTo(tokenViewModel));
        }

        [Test]
        public async Task Login_WhenCredentialsAreInvalid_ReturnsUnauthorized()
        {
            //Arrange
            UserParameter userViewModel = new()
            {
                Email = "email",
                Password = "password"
            };
            _userService.UserVerify(userViewModel).Returns(new FailResponse401("帳號或密碼錯誤"));

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
            UserParameter userViewModel = new()
            {
                Email = "email",
                Password = "password"
            };

            _userService.IsEmailNotRegisteredAsync(userViewModel.Email).Returns(true);

            //Act
            var okObjectResult = await _controller.CreateAccount(userViewModel) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.IsNotNull(responseData);
            Assert.IsTrue(responseData.Success);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task CreateAccount_WhenAccountExist_ReturnsUnauthorized()
        {
            //Arrange
            UserParameter userViewModel = new()
            {
                Email = "email",
                Password = "password"
            };

            _userService.IsEmailNotRegisteredAsync(userViewModel.Email).Returns(false);

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
        public async Task Logout_WhenCalled_ReturnsOk()
        {
            //Arrange
            LogoutParameter logoutParameter = new LogoutParameter()
            {
                RefreshToken = "refreshToken"
            };

            _tokenService.DeleteRefreshTokenAsync(logoutParameter.RefreshToken).Returns(true);

            //Act
            var okObjectResult = await _controller.Logout(logoutParameter) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Success, Is.True);
            Assert.That(responseData.Status, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task RefreshToken_WhenCalled_ReturnsToken()
        {
            //Arrange
            string refreshToken = "validToken";
            var response = new OkResponse<string>("newAccessToken");
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
        [TestCase(true, true, true, HttpStatusCode.OK, "")]
        [TestCase(false, false, true, HttpStatusCode.BadRequest, "格式錯誤")]
        [TestCase(false, true, false, HttpStatusCode.Unauthorized, "電子信箱已註冊")]
        public async Task Email_WhenCalled_ReturnsExpectedResult(bool isSuccess, bool isValidEmail, bool isEmailRegistered,
            int httpstatusCode, string errorMessage)
        {
            //Arrange
            string email = "emai";

            _validationService.IsValidEmail(email).Returns(isValidEmail);
            _userService.IsEmailNotRegisteredAsync(email).Returns(isEmailRegistered);

            //Act
            var okObjectResult = await _controller.VerifyEmail(email) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Success, Is.EqualTo(isSuccess));
            Assert.That(responseData.Status, Is.EqualTo(httpstatusCode));
            Assert.That(responseData.Message, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(true, true, true, HttpStatusCode.OK, "")]
        [TestCase(false, false, true, HttpStatusCode.BadRequest, "格式錯誤")]
        [TestCase(false, true, false, HttpStatusCode.Unauthorized, "手機號碼已註冊")]
        public async Task Phone_WhenCalled_ReturnsExpectedResult(bool isSuccess, bool isValidPhone, bool isPhoneRegistered,
            int httpstatusCode, string errorMessage)
        {
            //Arrange
            string phone = "phone";

            _validationService.IsValidPhone(phone).Returns(isValidPhone);
            _userService.IsPhoneNotRegisteredAsync(phone).Returns(isPhoneRegistered);

            //Act
            var okObjectResult = await _controller.VerifyPhone(phone) as OkObjectResult;

            //Assert
            Response<string>? responseData = okObjectResult?.Value as Response<string>;

            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Success, Is.EqualTo(isSuccess));
            Assert.That(responseData.Status, Is.EqualTo(httpstatusCode));
            Assert.That(responseData.Message, Is.EqualTo(errorMessage));
        }
    }
}
