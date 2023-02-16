using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TNArch.Microservices.Infrastructure.Common.OpenApi
{
    public class CommandToApiEndpointFilter : IDocumentFilter
    {
        private readonly ICommandToApiMapper _commandToApiMapper;

        public CommandToApiEndpointFilter(ICommandToApiMapper commandToApiMapper)
        {
            _commandToApiMapper = commandToApiMapper;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths.Clear();

            var commandToApiMaps = _commandToApiMapper.GetRequestMap();

            foreach (var commandToApiMap in commandToApiMaps)
            {
                var requestSchema = context.SchemaGenerator.GenerateSchema(commandToApiMap.RequestType, context.SchemaRepository);
                var responseSchema = context.SchemaGenerator.GenerateSchema(commandToApiMap.ResponseType, context.SchemaRepository);

                var request = new OpenApiRequestBody { Content = new Dictionary<string, OpenApiMediaType> { { "applicaiton/json", new OpenApiMediaType { Schema = requestSchema } } } };

                var response = new OpenApiResponses { { "200", new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "applicaiton/json", new OpenApiMediaType { Schema = responseSchema } } } } } };

                var apiPath = new OpenApiPathItem { Operations = new Dictionary<OperationType, OpenApiOperation> { { commandToApiMap.OperationType, new OpenApiOperation { RequestBody = request, Responses = response } } } };

                swaggerDoc.Paths.Add($"/api/{commandToApiMap.RequestName}", apiPath);
            }
        }
    }    
}
