namespace TNArch.Microservices.Core.Common.Command
{
    public interface ICommand
    {
        string Permission { get; }
    }

    public interface IQuery: ICommand
    {        
    }
}