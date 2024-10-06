using Jacmazon_ECommerce.Repositories;
using Jacmazon_ECommerce.ViewModels;
using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Models.LoginContext;


namespace Jacmazon_ECommerce.Services
{
    public class UserService : IUserService
    {
        private readonly ICRUDRepository<User> _repository;
        private readonly IHashingPassword _hashingPassword;

        public UserService(ICRUDRepository<User> userRepository,IHashingPassword hashingPassword)
        {
            _repository = userRepository;
            _hashingPassword = hashingPassword;
        }

        public async Task<bool> CreateUserAsync(string email, string password)
        {
            byte[] bytes = _hashingPassword.GenerateSalt();
            string passwordConfirm = _hashingPassword.HashPassword(password, bytes);

            User newUser = new()
            {
                Email = email,
                Password = passwordConfirm,
                Name = "",
                Rank = 0,
                Approved = true,
                Phone = "",
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Salt = bytes
            };

            return await _repository.CreateAsync(newUser);
        }

        public async Task<bool> IsEmailNotRegisteredAsync(string email)
        {
            IEnumerable<User> users = await _repository.FindAsync(u => u.Email == email);
            return !users.Any();
        }

        public async Task<bool> IsPhoneNotRegisteredAsync(string phone)
        {
            IEnumerable<User> users = await _repository.FindAsync(u => u.Phone == phone);
            return !users.Any();
        }

        public async Task<Response<string>> UserVerify(UserParameter userViewModel)
        {
            //取得資料庫資料
            var data = (await _repository.FindAsync(u => u.Email ==  userViewModel.Email)).FirstOrDefault();
            if (data == null)
            {
                return new Response<string>()
                {
                    Message = "查無此帳號"
                };
            }

            //密碼加密
            string hashedPassword = _hashingPassword.HashPassword(userViewModel.Password, data.Salt);

            //比較並回傳
            if (data.Password == hashedPassword)
            {
                return new OkResponse();
            }
            else
            {
                return new FailResponse401("密碼錯誤");
            }

        }
    }
}
