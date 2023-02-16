using TNArch.Microservices.Core.Common.DependencyInjection;
using TNArch.Microservices.Infrastructure.Common.OpenApi;

namespace TNArch.Microservices.Core.Common.Command
{
    public interface ICommandDispatcher
    {
        Task<CommandResponse> Dispatch(object command, IHandlerMap handlerMap);        
    }

    [Dependency(typeof(ICommandDispatcher))]
    public class CommandDispatcher : ICommandDispatcher
    {
        public Task<CommandResponse> Dispatch(object command, IHandlerMap handlerMap)
        {
            throw new NotImplementedException();
        }
    }
}