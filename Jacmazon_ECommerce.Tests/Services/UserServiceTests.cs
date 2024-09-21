using Jacmazon_ECommerce.JWT;
using Jacmazon_ECommerce.Repositories;
using System.Net;
using Jacmazon_ECommerce.Models.LoginContext;
using NSubstitute;
using Jacmazon_ECommerce.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;

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
    }
}
