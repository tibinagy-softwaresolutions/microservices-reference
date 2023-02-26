using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Reflection;
using System.Text.Json.Nodes;
using TNArch.Microservices.Infrastructure.Common.DependencyInjection;

namespace TNArch.Microservices.Infrastructure.Common.OpenApi
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
                            Name = $"{serviceName} Open Api document",
                            Title = $"{serviceName} Open Api document",
                            Description = $"Service enpoints of the {serviceName}",
                            Version = "v3"
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

        public static JsonObject QueryStringToJson(this IEnumerable<KeyValuePair<string, StringValues>> queryParameters)
        {
            var json = new JsonObject();

            var queryParametersByKey = queryParameters.GroupBy(k => k.Key[0..Math.Max(k.Key.IndexOf('.'), 0)]);

            foreach (var parameterGroup in queryParametersByKey)
            {
                if (parameterGroup.Key != string.Empty)
                {
                    json.Add(parameterGroup.Key, QueryStringToJson(parameterGroup.Select(p => new KeyValuePair<string, StringValues>(p.Key[(parameterGroup.Key.Length + 1)..], p.Value))));
                    continue;
                }

                parameterGroup.Where(p => p.Value.Count == 1).ForEach(p => json.Add(p.Key, JsonValue.Create(p.Value[0])));

                parameterGroup.Where(p => p.Value.Count > 1).ForEach(p => json.Add(p.Key, new JsonArray(p.Value.Select(v => JsonValue.Create(v)).ToArray())));
            }

            return json;

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
