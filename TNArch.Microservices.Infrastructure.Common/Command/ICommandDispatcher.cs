namespace TNArch.Microservices.Core.Common.Command
{
    public interface ICommandDispatcher
    {
        Task<CommandResponse> DispatchCommand<TCommand>(TCommand command) where TCommand : ICommand;
        Task<CommandResponse> DispatchCommand<TCommand, TResponse>(TCommand command) where TCommand : ICommand;
        Task<QueryResult<TResponse>> DispatchQuery<TQuery, TResponse>(TQuery query) where TQuery : IQuery;
    }
}