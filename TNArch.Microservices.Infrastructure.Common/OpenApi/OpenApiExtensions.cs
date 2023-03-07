using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
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

            builder.ServiceCollection.AddSwaggerGen(opts =>
                {
                    opts.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = $"{serviceName} Open Api document",
                        Description = $"Service enpoints of the {serviceName}",
                        Version = "v1"
                    });

                    opts.CustomSchemaIds(SchemaIdSelector);
                    opts.DocumentFilter<CommandToApiEndpointFilter>(mapper);
                    opts.SchemaFilter<CommandSchemaFilter>();
                    opts.SchemaFilter<EnumSchemaFilter>();
                });

            return builder;
        }

        public static string SchemaIdSelector(Type modelType)
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
    }
}
