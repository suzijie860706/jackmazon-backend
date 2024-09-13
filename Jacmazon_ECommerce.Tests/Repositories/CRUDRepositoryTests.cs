using Jacmazon_ECommerce.Data;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.Repositories;
using Jacmazon_ECommerce.Services;
using Microsoft.EntityFrameworkCore;


namespace Jacmazon_ECommerce.Tests.Repositories
{
    [TestFixture]
    public class CRUDRepositoryTests : PageTest
    {
        private LoginContext _context;
        private DbSet<Token> _dbset;
        private List<Token> tokens = new List<Token>();
        private CRUDRepository<Token, DbContext> _repository;

        public CRUDRepositoryTests()
        {
            tokens = new()
            {
                new Token
                {
                    Id = 1,
                    RefreshToken = "refreshToken",
                    ExpiredDate = DateTime.Now.AddMinutes(1),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                }
            };
        }

        [SetUp]
        public void SetUp()
        {
            //使用 InMemory 資料庫
            var options = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new LoginContext(options);
            _dbset = _context.Set<Token>();
            _context.Tokens.AddRange(tokens);
            _context.SaveChanges();

            _repository = new CRUDRepository<Token, DbContext>(_context);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }


        [Test]
        public async Task CreateAsync_WhenCalled_AddsEntityToDbSetAndSaveChanges()
        {
            //Arrange
            await _context.Database.EnsureDeletedAsync();

            //Act
            await _repository.CreateAsync(tokens[0]);

            //Assert
            var addedToken = await _context.Tokens.FindAsync(tokens[0].Id);
            Assert.That(addedToken, Is.Not.Null);
            Assert.That(addedToken.Id, Is.EqualTo(tokens[0].Id));
            Assert.That(addedToken.RefreshToken, Is.EqualTo(tokens[0].RefreshToken));
            Assert.That(addedToken.ExpiredDate, Is.EqualTo(tokens[0].ExpiredDate));
        }

        [Test]
        public async Task UpdateAsync_WhenCalled_UpdateToDbSetAndSaveChanges()
        {
            //Arrange
            tokens[0].RefreshToken = "newRefreshToken";

            //Act
            int count = await _repository.UpdateAsync(tokens[0]);

            //Assert
            var updatedToken = await _context.Tokens.FindAsync(tokens[0].Id);
            Assert.That(updatedToken, Is.Not.Null);
            Assert.That(updatedToken.Id, Is.EqualTo(tokens[0].Id));
            Assert.That(updatedToken.RefreshToken, Is.EqualTo("newRefreshToken"));
        }

        [Test]
        public async Task DeleteAsync_WhenCalled_DeleteFromDbSetAndSaveChanges()
        {
            //Arrange

            //Act
            int count = await _repository.DeleteAsync(tokens[0]);

            //Assert
            var deletedToken = await _context.Tokens.FindAsync(tokens[0].Id);
            Assert.That(deletedToken, Is.Null);
            Assert.That(_context.Tokens.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task FindByIdAsync_WhenCalled_RetrunsEntity()
        {
            //Arrange

            //Act
            var entity = await _repository.FindByIdAsync(1);

            //Assert
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task FindByIdAsync_WhenCalled_RetrunsNull()
        {
            //Arrange
            await _context.Database.EnsureDeletedAsync();
            _context.ChangeTracker.Clear();

            _repository = new CRUDRepository<Token, DbContext>(_context);

            //Act
            var entity = await _repository.FindByIdAsync(1);

            //Assert
            Assert.That(entity, Is.Null);
        }

        [Test]
        public async Task FindAsync_WhenCalled_RetrunsEntity()
        {
            //Arrange

            //Act
            var data = await _repository.FindAsync(u => u.RefreshToken == tokens[0].RefreshToken);

            //Assert
            Assert.That(data.ToList()[0].RefreshToken, Is.EqualTo(tokens[0].RefreshToken));
        }
    }
}
