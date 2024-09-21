using Jacmazon_ECommerce.Models.LoginContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacmazon_ECommerce.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests : CRUDRepositoryTestsBase<User>
    {
        protected override void SeedData()
        {
            _dbset.Add(new User
            {
                Id = 1,
                Approved = true,
                Email = "email",
                Name = "name",
                Password = "password",
                Phone = "Phone",
                Rank = 1,
                Salt = new byte[16],
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            });
        }

    }
}
