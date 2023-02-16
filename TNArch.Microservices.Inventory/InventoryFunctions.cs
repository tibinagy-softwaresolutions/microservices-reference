using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using AzureFunctions.Extensions.Swashbuckle;
using System.Net.Http;
using TNArch.Microservices.Infrastructure.Common.OpenApi;
using TNArch.Microservices.Core.Common.Command;
using System.Text.Json;

namespace TNArch.Microservices.Inventory
{
    public class InventoryFunctions
    {
        private readonly ICommandToApiMapper _commandToApiMapper;
        private readonly ICommandDispatcher _commandDispatcher;

        public InventoryFunctions(ICommandToApiMapper commandToApiMapper, ICommandDispatcher commandDispatcher)
        {
            _commandToApiMapper = commandToApiMapper;
            _commandDispatcher = commandDispatcher;
        }

        [FunctionName(nameof(OpenApi))]
        public static HttpResponseMessage OpenApi([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger.{extension}")] HttpRequestMessage req,
           string extension, [SwashBuckleClient] ISwashBuckleClient swasBuckleClient)
        {
            return swasBuckleClient.CreateSwaggerResponse(req, extension, "Inventory microservice");
        }

        [FunctionName(nameof(RequestReceived))]
        public async Task<IActionResult> RequestReceived([HttpTrigger(AuthorizationLevel.Function, "get", "post", "put", "delete", Route = "{*commandName}")] HttpRequest req, string commandName, ILogger log)
        {
            var handlerMap = _commandToApiMapper.GetHandlerMap(commandName, req.Method);

            var command = JsonSerializer.Deserialize(req.Body, handlerMap.RequestType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
           
            return new OkObjectResult(await _commandDispatcher.Dispatch(command, handlerMap));
        }
    }
}
