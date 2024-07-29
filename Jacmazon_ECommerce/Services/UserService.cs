using Jacmazon_ECommerce.Repositories;
using Jacmazon_ECommerce.Models.LoginContext;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Jacmazon_ECommerce.Services
{
    public class UserService : IUserService
    {
        private readonly ICRUDRepository<User> _repository;
        public UserService(ICRUDRepository<User> userRepository)
        {
            _repository = userRepository;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            User newUser = new()
            {
                Email = user.Email,
                Password = user.Password,
                Name = "",
                Rank = 0,
                Approved = true,
                Phone = "",
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

            return await _repository.CreateAsync(newUser);
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            IEnumerable<User> users = await _repository.FindAsync(u => u.Email == email);
            return users.Any();
        }

        public async Task<bool> IsPhoneRegisteredAsync(string phone)
        {
            IEnumerable<User> users = await _repository.FindAsync(u => u.Phone == phone);
            return users.Any();
        }

        public async Task<bool> VerifyUserLogin(string email, string password)
        {
            IEnumerable<User>? userLogin = await _repository.FindAsync(u => u.Email == email && u.Password == password);
            return userLogin.Any();
        }
    }
}
