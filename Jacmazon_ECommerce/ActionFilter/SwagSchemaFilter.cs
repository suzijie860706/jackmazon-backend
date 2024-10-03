using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Jacmazon_ECommerce.ViewModels;
using Microsoft.OpenApi.Any;

namespace Jacmazon_ECommerce.ActionFilter
{
    public class SwagSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(UserParameter))
            {
                var emailProperty = schema.Properties.FirstOrDefault(p => p.Key == "email").Value;
                if (emailProperty != null)
                {
                    emailProperty.MinLength = 6;
                    emailProperty.MaxLength = 30;
                    
                    //emailProperty.Description += " (The local part of the email must be between 3 and 30 characters)";
                }
            }
        }
    }
}
