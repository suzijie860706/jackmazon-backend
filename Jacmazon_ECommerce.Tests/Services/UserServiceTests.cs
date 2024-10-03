using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.Services;
using Jacmazon_ECommerce.Repositories;
using Jacmazon_ECommerce.JWT;
using Jacmazon_ECommerce.ViewModels;
using Jacmazon_ECommerce.Models;
using NSubstitute;


namespace Jacmazon_ECommerce.Tests.Services
{
    [TestFixture]
    public class UserServiceTests : PageTest
    {
        private ICRUDRepository<User> _repository;
        private IHashingPassword _hashingPassword;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<ICRUDRepository<User>>();
            _hashingPassword = Substitute.For<IHashingPassword>();
            _userService = new UserService(_repository, _hashingPassword);
        }

        [Test]
        public async Task CreateUserAsync_WhenCalled_ReturnsOk()
        {
            //Arrange
            string email = "email@gmail.com";
            string password = "password";

            byte[] bytes = new byte[16];
            _hashingPassword.GenerateSalt().Returns(new byte[16]);
            _hashingPassword.HashPassword(password, bytes).Returns("");

            _repository.CreateAsync(Arg.Any<User>()).Returns(true);

            //Act
            bool result = await _userService.CreateUserAsync(email, password);

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsEmailNotRegisteredAsync_EmailNotRegistered_ReturnsTrue()
        {
            //Arrange
            string email = "email";
            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(Enumerable.Empty<User>());

            //Act
            bool isValid = await _userService.IsEmailNotRegisteredAsync(email);

            //Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public async Task IsEmailNotRegisteredAsync_EmailRegistered_ReturnsFalse()
        {
            //Arrange
            string email = "email";
            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(new List<User>() { new User { Id = 1 } });

            //Act
            bool isValid = await _userService.IsEmailNotRegisteredAsync(email);

            //Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public async Task IsPhoneNotRegisteredAsync_PhoneNotRegistered_ReturnsTrue()
        {
            //Arrange
            string phone = "phone";
            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(Enumerable.Empty<User>());

            //Act
            bool isValid = await _userService.IsPhoneNotRegisteredAsync(phone);

            //Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public async Task IsPhoneNotRegisteredAsync_PhoneRegistered_ReturnsFalse()
        {
            //Arrange
            string phone = "phone";
            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(new List<User>() { new User { Id = 1 } });

            //Act
            bool isValid = await _userService.IsPhoneNotRegisteredAsync(phone);

            //Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public async Task UserVerify_WhenCalled_ReturnsOk()
        {
            //Arrange
            UserParameter userViewModel = new UserParameter() { Email = "email", Password = "password" };
            List<User> users = new()
            {
                new User
                {
                    Email = userViewModel.Email,
                    Password = userViewModel.Password,
                    Salt = new byte[16]
                }
            };

            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(users);

            _hashingPassword.HashPassword(userViewModel.Password, users[0].Salt).Returns(userViewModel.Password);

            //Act
            Response<string> response = await _userService.UserVerify(userViewModel);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
        }

        [Test]
        public async Task UserVerify_UserNotFound_ReturnsErrorMsg()
        {
            //Arrange
            UserParameter userViewModel = new UserParameter() { Email = "email", Password = "password" };

            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(new List<User>());

            //Act
            Response<string> response = await _userService.UserVerify(userViewModel);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("查無此帳號"));
        }

        [Test]
        public async Task UserVerify_PasswordError_ReturnsErrorMsg()
        {
            //Arrange
            UserParameter userViewModel = new UserParameter() { Email = "email", Password = "errorPassword" };
            List<User> users = new()
            {
                new User
                {
                    Email = userViewModel.Email,
                    Password = "password",
                    Salt = new byte[16]
                }
            };

            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(users);

            _hashingPassword.HashPassword(userViewModel.Password, users[0].Salt).Returns(userViewModel.Password);

            //Act
            Response<string> response = await _userService.UserVerify(userViewModel);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("密碼錯誤"));
        }
    }
}
