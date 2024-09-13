using Jacmazon_ECommerce.JWT;
using Jacmazon_ECommerce.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Jacmazon_ECommerce.Models.LoginContext;
using NSubstitute;
using Jacmazon_ECommerce.Services;
using Jacmazon_ECommerce.ViewModels;
using Jacmazon_ECommerce.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace Jacmazon_ECommerce.Tests.Services
{
    [TestFixture]
    public class TokenServiceTests : PageTest
    {
        private ICRUDRepository<Token> _repository;
        private IJWTSettings _jwtSettings;
        private TokenService tokenService;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<ICRUDRepository<Token>>();
            _jwtSettings = Substitute.For<IJWTSettings>();
            tokenService = new TokenService(_repository, _jwtSettings);
        }

        [Test]
        public async Task CreateTokenAsync_WhenCalled_ReturnsToken()
        {
            //Arrange
            string email = "email@gmail.com";
            string refreshToken = "refreshToken";

            _jwtSettings.CreateAccessToken(email).Returns("accessToken");
            _jwtSettings.CreateRefreshToken(email).Returns(refreshToken);
            Token token = new Token() { RefreshToken = refreshToken };
            _repository.CreateAsync(token).Returns(true);

            //Act
            var result = await tokenService.CreateTokenAsync(email);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RefreshToken, Is.EqualTo("refreshToken"));
            Assert.That(result.AccessToken, Is.EqualTo("accessToken"));
        }

        [Test]
        public async Task UpdateRefreshTokenAsync_ValidToken_ReturnsNewAccessToken()
        {

            //Arrange
            List<Token> tokens = new()
            {
                new Token
                {
                    RefreshToken = "refreshToken",
                    ExpiredDate = DateTime.Now.AddMinutes(1),
                }
            };
            //拆解Token取得Email
            _jwtSettings.ReadToken(tokens[0].RefreshToken).Returns(new JwtSecurityToken());
            //確認Token存在
            _repository.FindAsync(Arg.Is<Expression<Func<Token, bool>>>(expr => expr.Compile()(new Token { RefreshToken = tokens[0].RefreshToken })))
                .Returns(Task.FromResult((IEnumerable<Token>)tokens));
            //利用Email取得新Token
            _jwtSettings.CreateRefreshToken(Arg.Any<string>()).Returns("newAccessToken");

            //Act
            var response = await tokenService.UpdateRefreshTokenAsync(tokens[0].RefreshToken);


            //Assert
            Assert.That(response.Data, Is.EqualTo("newAccessToken"));
            Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(response.Success, Is.EqualTo(true));
        }

        [Test]
        public async Task UpdateRefreshTokenAsync_TokenNotFound_ReturnsUnauthorized()
        {
            //Arrange
            //拆解Token取得Email
            _jwtSettings.ReadToken(Arg.Any<string>()).Returns(new JwtSecurityToken());
            //確認Token存在
            _repository.FindAsync(Arg.Is<Expression<Func<Token, bool>>>(expr => expr.Compile()(new Token { RefreshToken = "InvalidRefreshToken" })))
                .Returns(Task.FromResult((Enumerable.Empty<Token>())));

            //Act
            var response = await tokenService.UpdateRefreshTokenAsync("InvalidRefreshToken");


            //Assert
            Assert.That(response.Data, Is.Null);
            Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("查無此Token"));
        }

        [Test]
        public async Task UpdateRefreshTokenAsync_TokenExpired_ReturnsUnauthorized()
        {
            //Arrange
            List<Token> tokens = new()
            {
                new Token
                {
                    RefreshToken = "refreshToken",
                    ExpiredDate = DateTime.Now.AddMinutes(-1),
                }
            };

            //拆解Token取得Email
            _jwtSettings.ReadToken(Arg.Any<string>()).Returns(new JwtSecurityToken());
            //確認Token存在
            _repository.FindAsync(Arg.Is<Expression<Func<Token, bool>>>(expr => expr.Compile()(new Token { RefreshToken = tokens[0].RefreshToken })))
                .Returns(Task.FromResult((IEnumerable<Token>)tokens));

            //Act
            var response = await tokenService.UpdateRefreshTokenAsync(tokens[0].RefreshToken);


            //Assert
            Assert.That(response.Data, Is.Null);
            Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("RefreshToken已過期"));
        }

        [Test]
        public async Task UpdateRefreshTokenAsync_JwtReadTokenReturnsNull_ReturnsUnauthorized()
        {
            //Arrange
            //拆解Token取得Email
            _jwtSettings.ReadToken(Arg.Any<string>()).Returns((JwtSecurityToken?)null);

            //Act
            var response = await tokenService.UpdateRefreshTokenAsync("");

            //Assert
            Assert.That(response.Data, Is.Null);
            Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("查無此Token"));
        }

        [Test]
        public async Task DeleteRefreshTokenAsync_ValidToken_ReturnsOk()
        {
            //Arrange
            List<Token> tokens = new()
            {
                new Token
                {
                    RefreshToken = "refreshToken",
                    ExpiredDate = DateTime.Now.AddMinutes(1),
                }
            };
            //確認Token存在
            _repository.FindAsync(Arg.Is<Expression<Func<Token, bool>>>(expr => expr.Compile()(new Token
                { RefreshToken = tokens[0].RefreshToken }))).Returns(Task.FromResult((IEnumerable<Token>)tokens));
            //Act
            var response = await tokenService.DeleteRefreshTokenAsync(tokens[0].RefreshToken);

            //Assert
            Assert.That(response, Is.True);
        }

        [Test]
        public async Task DeleteRefreshTokenAsync_InValidToken_ReturnsUnauthorized()
        {
            //Arrange
            //確認Token存在
            _repository.FindAsync(Arg.Any<Expression<Func<Token, bool>>>()).Returns(Task.FromResult((Enumerable.Empty<Token>())));
            //Act
            var response = await tokenService.DeleteRefreshTokenAsync("InvalidToken");

            //Assert
            Assert.That(response, Is.False);
        }
    }
}
