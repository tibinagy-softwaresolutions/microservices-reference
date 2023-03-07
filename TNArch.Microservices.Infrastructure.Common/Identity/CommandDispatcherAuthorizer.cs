using TNArch.Microservices.Core.Common.Command;
using TNArch.Microservices.Core.Common.DependencyInjection;

namespace TNArch.Microservices.Infrastructure.Common.Identity
{
    [DecorateDependency(typeof(ICommandDispatcher), typeof(CommandDispatcher))]
    public class CommandDispatcherAuthorizer : ICommandDispatcher
    {
        private readonly ICommandDispatcher _inner;
        private readonly IIdentityService _identityService;

        public CommandDispatcherAuthorizer(ICommandDispatcher inner, IIdentityService identityService)
        {
            _inner = inner;
            _identityService = identityService;
        }

        public async Task<CommandResponse> DispatchCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (!await _identityService.HasPermission(command.Permission))
                return new CommandResponse { IsAuthorized = false };

            return await _inner.DispatchCommand(command);
        }

        public async Task<CommandResponse> DispatchCommand<TCommand, TResponse>(TCommand command) where TCommand : ICommand
        {
            if (!await _identityService.HasPermission(command.Permission))
                return new CommandResponse { IsAuthorized = false };

            return await _inner.DispatchCommand<TCommand, TResponse>(command);

        }

        public async Task<QueryResult<TResponse>> DispatchQuery<TQuery, TResponse>(TQuery query) where TQuery : IQuery
        {
            if (!await _identityService.HasPermission(query.Permission))
                return new QueryResult<TResponse> { IsAuthorized = false };

            return await _inner.DispatchQuery<TQuery, TResponse>(query);
        }
    }
}