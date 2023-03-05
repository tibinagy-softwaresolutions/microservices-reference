using TNArch.Microservices.Core.Common.DependencyInjection;

namespace TNArch.Microservices.Core.Common.Command
{
    [DecorateDependency(typeof(ICommandDispatcher), typeof(CommandDispatcher))]
    public class CommandDispatcherAuthorizer : ICommandDispatcher
    {
        private readonly ICommandDispatcher _inner;
        private readonly IPermissionService _permissionService;

        public CommandDispatcherAuthorizer(ICommandDispatcher inner, IPermissionService permissionService)
        {
            _inner = inner;
            _permissionService = permissionService;
        }

        public async Task<CommandResponse> DispatchCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (!await _permissionService.HasPermission(command.Permission))
                return new CommandResponse { IsAuthorized = false };

            return await _inner.DispatchCommand(command);
        }

        public async Task<CommandResponse> DispatchCommand<TCommand, TResponse>(TCommand command) where TCommand : ICommand
        {
            if (!await _permissionService.HasPermission(command.Permission))
                return new CommandResponse { IsAuthorized = false };

            return await _inner.DispatchCommand<TCommand, TResponse>(command);

        }

        public async Task<QueryResult<TResponse>> DispatchQuery<TQuery, TResponse>(TQuery query) where TQuery : IQuery
        {
            if (!await _permissionService.HasPermission(query.Permission))
                return new QueryResult<TResponse> { IsAuthorized = false };

            return await _inner.DispatchQuery<TQuery, TResponse>(query);
        }
    }
}