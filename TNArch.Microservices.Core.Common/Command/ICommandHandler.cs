namespace TNArch.Microservices.Core.Common.Command
{
    public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand
    {
        Task<TResponse> Handle(TCommand command);
    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }
}