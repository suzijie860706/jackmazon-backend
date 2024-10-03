using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Jacmazon_ECommerce.ActionFilter
{
    public class SwagParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (parameter.Name == "Email")
            {
                parameter.Description += " (The local part of the email must be between 3 and 30 characters)";
            }
            if (parameter.Name == "password")
            {
                parameter.Description += " (123123123)";
            }
            
        }

    }
}
