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

namespace TNArch.Microservices.Inventory.Functions
{
    public class InventoryFunctions
    {
        private readonly ICommandDispatcherInvoker _invoker;

        public InventoryFunctions(ICommandDispatcherInvoker invoker)
        {
            _invoker = invoker;
        }

        [FunctionName(nameof(OpenApi))]
        public static HttpResponseMessage OpenApi([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger.{extension}")] HttpRequestMessage req,
           string extension, [SwashBuckleClient] ISwashBuckleClient swasBuckleClient)
        {
            return swasBuckleClient.CreateSwaggerResponse(req, extension);
        }

        [FunctionName(nameof(RequestReceived))]
        public async Task<IActionResult> RequestReceived([HttpTrigger(AuthorizationLevel.Function, "get", "post", "put", "delete", Route = "{*commandName}")] HttpRequest req, string commandName, ILogger log)
        {
            return new JsonResult(await _invoker.DispatchRequest(commandName, req));
        }
    }
}
