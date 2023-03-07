using Microsoft.Extensions.Logging;
using TNArch.Microservices.Core.Common.DependencyInjection;
using TNArch.Microservices.Infrastructure.Common.Identity;

namespace TNArch.Microservices.Core.Common.Command
{
    [DecorateDependency(typeof(ICommandDispatcher), typeof(CommandDispatcherAuthorizer))]
    public class CommandDispatcherLogger : ICommandDispatcher
    {
        private readonly ICommandDispatcher _inner;
        private readonly ILogger _logger;

        public CommandDispatcherLogger(ICommandDispatcher inner, ILogger<CommandDispatcher> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<CommandResponse> DispatchCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            try
            {
                _logger.LogInformation($"Handling {typeof(TCommand).Name}");
                return await _inner.DispatchCommand(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured when handling {typeof(TCommand).Name}");
                throw;
            }
        }

        public async Task<CommandResponse> DispatchCommand<TCommand, TResponse>(TCommand command) where TCommand : ICommand
        {
            try
            {
                _logger.LogInformation($"Handling {typeof(TCommand).Name}");
                return await _inner.DispatchCommand<TCommand,TResponse>(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured when handling {typeof(TCommand).Name}");
                throw;
            }
        }

        public async Task<QueryResult<TResponse>> DispatchQuery<TQuery, TResponse>(TQuery query) where TQuery : IQuery
        {
            try
            {
                _logger.LogInformation($"Handling {typeof(TQuery).Name}");
                return await _inner.DispatchQuery<TQuery, TResponse>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured when handling {typeof(TQuery).Name}");
                throw;
            }
        }
    }
}