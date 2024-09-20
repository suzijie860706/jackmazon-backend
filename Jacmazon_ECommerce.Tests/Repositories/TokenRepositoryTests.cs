using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.Data;
using Jacmazon_ECommerce.Repositories;
using Jacmazon_ECommerce.Services;
using Microsoft.EntityFrameworkCore;

namespace Jacmazon_ECommerce.Tests.Repositories
{
    [TestFixture]
    public class TokenRepositoryTests : CRUDRepositoryTestsBase<Token>
    {
        public TokenRepositoryTests() : base(){ }

        protected override void SeedData()
        {
            _dbset.Add(new Token
            {
                Id = 1,
                RefreshToken = "refreshToken",
                ExpiredDate = DateTime.Now.AddMinutes(1),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            });
        }
    }
}