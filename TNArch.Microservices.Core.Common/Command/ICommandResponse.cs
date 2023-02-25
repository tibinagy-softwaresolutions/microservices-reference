namespace TNArch.Microservices.Core.Common.Command
{
    public class CommandResponse
    {
        public bool IsAuthorized { get; set; } = true;
        public bool IsValid { get; set; } = true;
        public ValidationError[] ValidationErrors { get; set; }
    }

    public class CommandResponse<TResponse> : CommandResponse
    {
        public TResponse Response { get; set; }
    }

    public class QueryResult<TResponse>
    {
        public bool IsAuthorized { get; set; } = true;
        public TResponse Response { get; set; }
    }
}