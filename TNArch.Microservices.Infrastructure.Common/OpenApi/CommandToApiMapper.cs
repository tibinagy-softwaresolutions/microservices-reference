using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Data;
using TNArch.Microservices.Core.Common.Command;

namespace TNArch.Microservices.Infrastructure.Common.OpenApi
{
    public class CommandToApiMapper : ICommandToApiMapper
    {
        private readonly Type[] _handlerTypes = new[] { typeof(ICommandHandler<>), typeof(ICommandHandler<,>), typeof(IQueryHandler<,>) };

        private readonly Dictionary<string, OperationType> _prefixToOperationMap = new Dictionary<string, OperationType>()
        {
            { "Create", OperationType.Post },
            { "Add", OperationType.Post },
            { "Update", OperationType.Put },
            { "Change", OperationType.Put },
            { "Edit", OperationType.Put },
            { "Delete", OperationType.Delete },
            { "Remove", OperationType.Delete },
            { "Deactivate", OperationType.Delete },
        };

        private readonly Lazy<HandlerRequestMap[]> _requestMap;

        public CommandToApiMapper(IServiceCollection services)
        {
            _requestMap = new Lazy<HandlerRequestMap[]>(() => InitializeHandlerRequestMaps(services));
        }

        private HandlerRequestMap[] InitializeHandlerRequestMaps(IServiceCollection services)
        {
            var handlerTypes = services.Where(s => s.ServiceType.IsGenericType && _handlerTypes.Contains(s.ServiceType.GetGenericTypeDefinition()));

            return handlerTypes.Select(s => GetHandlerRequestMap(s.ServiceType)).ToArray();
        }

        private HandlerRequestMap GetHandlerRequestMap(Type serviceType)
        {
            var genericArguments = serviceType.GetGenericArguments();

            var requestType = genericArguments[0];

            var isCommand = typeof(ICommand).IsAssignableFrom(requestType);
            var hasReturnType = genericArguments.Length > 1;

            var responseType = typeof(CommandResponse);

            if (!isCommand)
                responseType = typeof(IQueryResult<>).MakeGenericType(genericArguments.Last());

            else if (hasReturnType)
                responseType = typeof(CommandResponse<>).MakeGenericType(genericArguments.Last());

            return new HandlerRequestMap
            {
                RequestName = GetRequestName(requestType),
                OperationType = GetOperationType(requestType),
                HandlerType = serviceType,
                RequestType = requestType,
                ResponseType = responseType
            };
        }

        private string GetRequestName(Type requestType)
        {
            var requestName = requestType.Name;

            if (requestName.EndsWith("Query"))
                requestName = requestName.Replace("Query", string.Empty);

            if (requestName.EndsWith("Command"))
                requestName = requestName.Replace("Command", string.Empty);

            return char.ToLowerInvariant(requestName[0]) + requestName[1..];
        }

        private OperationType GetOperationType(Type requestType)
        {
            if (requestType.IsAssignableFrom(typeof(IQuery)))
                return OperationType.Get;

            return _prefixToOperationMap.First(m => requestType.Name.StartsWith(m.Key, StringComparison.OrdinalIgnoreCase)).Value;
        }

        public IEnumerable<IRequestMap> GetRequestMap()
        {
            return _requestMap.Value.Cast<IRequestMap>();
        }

        public IHandlerMap GetHandlerMap(string commandName, string httpMethod)
        {
            return _requestMap.Value.Single(r => r.RequestName == commandName && r.OperationType.ToString().Equals(httpMethod, StringComparison.OrdinalIgnoreCase));
        }

        private sealed class HandlerRequestMap : IRequestMap, IHandlerMap
        {
            public string RequestName { get; set; }
            public OperationType OperationType { get; set; }
            public Type HandlerType { get; set; }
            public Type RequestType { get; set; }
            public Type ResponseType { get; set; }
        }

    }
}
