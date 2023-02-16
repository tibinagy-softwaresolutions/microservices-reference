namespace TNArch.Microservices.Core.Common.Command
{
    public interface ICommandValidator
    {
        Task<IEnumerable<ValidationError>> Validate<TCommand>(TCommand command) where TCommand : ICommand;
    }

    public interface ICommandValidator<TCommand> where TCommand : ICommand
    {
        Task<List<ValidationError>> Validate(TCommand command);
    }
}