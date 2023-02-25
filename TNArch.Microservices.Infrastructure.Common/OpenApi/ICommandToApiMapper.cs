using Microsoft.OpenApi.Models;
using System.Reflection;

namespace TNArch.Microservices.Infrastructure.Common.OpenApi
{
    public interface IOperationToApiMapper
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
        Type RequestType { get; set; }
        public MethodInfo DispatcherInvoker { get; set; }
    }
}
