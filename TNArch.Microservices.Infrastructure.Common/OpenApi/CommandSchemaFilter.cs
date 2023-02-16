using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TNArch.Microservices.Core.Common.Command;

namespace TNArch.Microservices.Infrastructure.Common.OpenApi
{
    public class CommandSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties.Count == 0)
                return;

            if (context.Type.GetInterfaces().Contains(typeof(ICommand)))
                schema.Properties.Remove("permission");
        }
    }
}
