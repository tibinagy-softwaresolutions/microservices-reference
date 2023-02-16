namespace TNArch.Microservices.Core.Common.Command
{
    public class ValidationError
    {
        public string Field { get; set; }
        public string ErrorMessage { get; set; }
    }
}