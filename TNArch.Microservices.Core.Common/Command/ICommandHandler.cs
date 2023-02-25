namespace TNArch.Microservices.Core.Common.Command
{
    public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand
    {
        Task<TResponse> Handle(TCommand command);
    }

    public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery
    {
        Task<TResponse> Handle(TQuery query);
    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }
}