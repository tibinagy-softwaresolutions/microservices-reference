using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TNArch.Microservices.Infrastructure.Common.OpenApi
{
    public class CommandToApiEndpointFilter : IDocumentFilter
    {
        private readonly IOperationToApiMapper _commandToApiMapper;

        public CommandToApiEndpointFilter(IOperationToApiMapper commandToApiMapper)
        {
            _commandToApiMapper = commandToApiMapper;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths.Clear();

            var commandToApiMaps = _commandToApiMapper.GetRequestMap();

            foreach (var commandToApiMap in commandToApiMaps)
            {
                var responseSchema = context.SchemaGenerator.GenerateSchema(commandToApiMap.ResponseType, context.SchemaRepository);

                var response = new OpenApiResponses { { "200", new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "applicaiton/json", new OpenApiMediaType { Schema = responseSchema } } } } } };

                var apiPath = new OpenApiPathItem { Operations = new Dictionary<OperationType, OpenApiOperation> { { commandToApiMap.OperationType, new OpenApiOperation { Responses = response } } } };

                if (commandToApiMap.OperationType == OperationType.Get)
                {
                    var schemaRepository = new SchemaRepository();

                    var requestSchema = context.SchemaGenerator.GenerateSchema(commandToApiMap.RequestType, schemaRepository);

                    var parameters = GetQueryParameters(requestSchema, schemaRepository);

                    apiPath.Operations[commandToApiMap.OperationType].Parameters = parameters;
                }
                else
                {
                    var requestSchema = context.SchemaGenerator.GenerateSchema(commandToApiMap.RequestType, context.SchemaRepository);
                    apiPath.Operations[commandToApiMap.OperationType].RequestBody = new OpenApiRequestBody { Content = new Dictionary<string, OpenApiMediaType> { { "applicaiton/json", new OpenApiMediaType { Schema = requestSchema } } } };
                }

                swaggerDoc.Paths.Add($"/api/{commandToApiMap.RequestName}", apiPath);
            }
        }

        private static List<OpenApiParameter> GetQueryParameters(OpenApiSchema requestSchema, SchemaRepository schemaRepository, string prefix = null)
        {
            var schema = schemaRepository.Schemas[requestSchema.Reference.Id];

            if (schema.Enum.Any())
                return new List<OpenApiParameter> { new OpenApiParameter { In = ParameterLocation.Query, Name = prefix, Schema = schema } };

            var primitiveTypes = schema.Properties.Where(p => p.Value.Type != null);

            if (prefix != null)
                prefix = $"{prefix}.";

            var nestedTypes = schema.Properties
                .Except(primitiveTypes)
                .SelectMany(p => GetQueryParameters(p.Value, schemaRepository, $"{prefix}{p.Key}"));

            return primitiveTypes
                .Select(p => new OpenApiParameter { In = ParameterLocation.Query, Name = $"{prefix}{p.Key}", Schema = p.Value })
                .Union(nestedTypes)
                .ToList();
        }
    }
}
