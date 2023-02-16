using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace TNArch.Microservices.Infrastructure.Common.OpenApi
{
    public interface ICommandToApiMapper
    {
        IEnumerable<IRequestMap> GetRequestMap();
        IHandlerMap GetHandlerMap(string commandName, string httpMethod);
    }

    public interface IRequestMap
    {
        string RequestName { get; set; }
        OperationType OperationType { get; set; }
        Type RequestType { get; set; }
        Type ResponseType { get; set; }
    }

    public interface IHandlerMap
    {
        Type HandlerType { get; set; }
        Type RequestType { get; set; }
    }
}
