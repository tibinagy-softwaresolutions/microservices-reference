using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using System.Net;
using System.Reflection;
using TNArch.Microservices.Infrastructure.Common.DependencyInjection;

namespace TNArch.Microservices.Infrastructure.Common.OpenApi
{
    public static class OpenApiExtensions
    {
        public static DependencyDescriptorBuilder UseCommandAsApiEndpoint(this DependencyDescriptorBuilder builder, string serviceName)
        {
            var mapper = new CommandToApiMapper(builder.ServiceCollection);
            builder.ServiceCollection.AddSingleton<ICommandToApiMapper>(mapper);

            builder.ServiceCollection.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
                {
                    opts.AddCodeParameter = true;
                    opts.Documents = new[]
                    {
                        new SwaggerDocument
                        {
                            Name = $"{serviceName} Open Api document",
                            Title = $"{serviceName} Open Api document",
                            Description = $"Service enpoints of the {serviceName}",
                            Version = "v2"
                        }
                    };
                    opts.ConfigureSwaggerGen = x =>
                    {
                        x.CustomSchemaIds(SchemaIdSelector);
                        x.DocumentFilter<CommandToApiEndpointFilter>(mapper);
                        x.SchemaFilter<CommandSchemaFilter>();
                        x.SchemaFilter<EnumSchemaFilter>();
                    };
                });

            return builder;
        }

        private static string SchemaIdSelector(Type modelType)
        {
            if (!modelType.IsConstructedGenericType)
            {
                return modelType.Name;
            }

            var prefix = modelType.GetGenericArguments()
                .Select(SchemaIdSelector)
                .Aggregate<string>((previous, current) => previous + current);

            return modelType.Name.Split('`').First() + "Of" + prefix;
        }

        public static HttpResponseMessage CreateSwaggerResponse(this ISwashBuckleClient swasBuckleClient, HttpRequestMessage req, string extension, string serviceName)
        {
            if (extension == "json")
                return swasBuckleClient.CreateSwaggerJsonDocumentResponse(req, $"{serviceName} Open Api document");

            if (extension == "yaml")
                return swasBuckleClient.CreateSwaggerYamlDocumentResponse(req, $"{serviceName} Open Api document");

            if (extension == "html")
                return swasBuckleClient.CreateSwaggerUIResponse(req, "swagger.json");

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
