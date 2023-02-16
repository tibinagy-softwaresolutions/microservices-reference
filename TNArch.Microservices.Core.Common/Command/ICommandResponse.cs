namespace TNArch.Microservices.Core.Common.Command
{
    public class CommandResponse
    {
        public bool IsAuthorized { get; set; }
        public bool IsValid { get; }
        public ValidationError[] ValidationErrors { get; set; }
    }

    public class CommandResponse<TResponse>: CommandResponse
    {
        public TResponse Response { get; }
    }
}