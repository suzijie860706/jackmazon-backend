using Jacmazon_ECommerce.Models;
using Jacmazon_ECommerce.Models.LoginContext;
using Jacmazon_ECommerce.ViewModels;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Jacmazon_ECommerce.ActionFilter
{
    public class UserParameterExample : IExamplesProvider<UserParameter>
    {
        public UserParameter GetExamples()
        {
            return new UserParameter
            {
                Email = "asdasd@gmail.com",
                Password = "asdasdasd",
            };
        }
    }
}
