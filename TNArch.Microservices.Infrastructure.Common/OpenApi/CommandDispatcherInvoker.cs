using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TNArch.Microservices.Core.Common.Command;
using TNArch.Microservices.Core.Common.DependencyInjection;
using TNArch.Microservices.Core.Common.Extensions;
using Microsoft.Extensions.Options;

namespace TNArch.Microservices.Infrastructure.Common.OpenApi
{
    public interface ICommandDispatcherInvoker
    {
        Task<IActionResult> DispatchRequest(string commandName, HttpRequest req);
    }

    [Dependency(typeof(ICommandDispatcherInvoker))]
    public class CommandDispatcherInvoker : ICommandDispatcherInvoker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOperationToApiMapper _operationToApiMapper;
        private readonly JsonSerializerOptions _options;

        public CommandDispatcherInvoker(IServiceProvider serviceProvider, IOperationToApiMapper operationToApiMapper, IOptions<JsonSerializerOptions> options)
        {
            _serviceProvider = serviceProvider;
            _operationToApiMapper = operationToApiMapper;
            _options = options.Value;
        }

        public async Task<IActionResult> DispatchRequest(string commandName, HttpRequest req)
        {
            var handlerMap = _operationToApiMapper.GetHandlerMap(commandName, req.Method);

            object requestObject;

            if (req.QueryString.HasValue)
                requestObject = JsonSerializer.Deserialize(req.Query.QueryStringToJson(), handlerMap.RequestType, _options);
            else
                requestObject = await JsonSerializer.DeserializeAsync(req.Body, handlerMap.RequestType, _options);

            using var scope = _serviceProvider.CreateScope();

            var commandDispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();

            var invocationResult = await handlerMap.DispatcherInvoker.InvokeAsync(commandDispatcher, requestObject);

            return new OkObjectResult(invocationResult);
        }        
    }
}
