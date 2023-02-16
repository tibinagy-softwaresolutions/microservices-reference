namespace TNArch.Microservices.Core.Common.Command
{
    public interface IQueryResult<TResult>
    {
        bool IsAuthorized { get; set; }
        TResult Result { get; set; }
    }
}