using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Reflection;
using TNArch.Microservices.Infrastructure.Common.DependencyInjection;
using TNArch.Microservices.Infrastructure.Common.OpenApi;

namespace TNArch.Microservices.Inventory
{
    public static class OpenApiExtensions
    {
        public static DependencyDescriptorBuilder UseCommandAsApiEndpoint(this DependencyDescriptorBuilder builder, string serviceName)
        {            
            var mapper = new CommandToApiMapper(builder.ServiceCollection);
            builder.ServiceCollection.AddSingleton<IOperationToApiMapper>(mapper);

            builder.ServiceCollection.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
            {
                opts.AddCodeParameter = true;
                opts.Documents = new[]
                {
                        new SwaggerDocument
                        {
                            Name = "v1",
                            Title = $"{serviceName} Open Api document",
                            Description = $"Service enpoints of the {serviceName}",
                            Version = "v3"
                        }
                    };
                opts.ConfigureSwaggerGen = x =>
                {
                    x.CustomSchemaIds(Infrastructure.Common.OpenApi.OpenApiExtensions.SchemaIdSelector);
                    x.DocumentFilter<CommandToApiEndpointFilter>(mapper);
                    x.SchemaFilter<CommandSchemaFilter>();
                    x.SchemaFilter<EnumSchemaFilter>();
                };
            });

            return builder;
        }


        public static HttpResponseMessage CreateSwaggerResponse(this ISwashBuckleClient swasBuckleClient, HttpRequestMessage req, string extension)
        {
            if (extension == "json")
                return swasBuckleClient.CreateSwaggerJsonDocumentResponse(req, "v1");

            if (extension == "yaml")
                return swasBuckleClient.CreateSwaggerYamlDocumentResponse(req, "v1");

            if (extension == "html")
                return swasBuckleClient.CreateSwaggerUIResponse(req, "swagger.json");

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
