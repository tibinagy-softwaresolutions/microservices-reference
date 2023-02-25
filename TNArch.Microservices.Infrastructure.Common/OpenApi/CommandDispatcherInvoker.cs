using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TNArch.Microservices.Core.Common.Command;
using TNArch.Microservices.Core.Common.DependencyInjection;
using TNArch.Microservices.Core.Common.Extensions;

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

        public CommandDispatcherInvoker(IServiceProvider serviceProvider, IOperationToApiMapper operationToApiMapper)
        {
            _serviceProvider = serviceProvider;
            _operationToApiMapper = operationToApiMapper;
        }

        public async Task<IActionResult> DispatchRequest(string commandName, HttpRequest req)
        {
            //https://stackoverflow.com/questions/9817591/convert-querystring-from-to-object
            var handlerMap =_operationToApiMapper.GetHandlerMap(commandName, req.Method);            
            
            var request = await JsonSerializer.DeserializeAsync(req.Body, handlerMap.RequestType);

            using var scope = _serviceProvider.CreateScope();

            var commandDispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();

            var invocationResult = await handlerMap.DispatcherInvoker.InvokeAsync(commandDispatcher,  request);
            
            return new OkObjectResult(invocationResult);
        }

        
    }
}
