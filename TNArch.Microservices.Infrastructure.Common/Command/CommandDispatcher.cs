using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using TNArch.Microservices.Core.Common.DependencyInjection;
using TNArch.Microservices.Core.Common.Extensions;

namespace TNArch.Microservices.Core.Common.Command
{
    [Dependency(typeof(ICommandDispatcher))]
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPermissionService _permissionService;

        public CommandDispatcher(IServiceProvider serviceProvider, IPermissionService permissionService)
        {
            _serviceProvider = serviceProvider;
            _permissionService = permissionService;
        }

        public async Task<CommandResponse> DispatchCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (!await _permissionService.HasPermission(command.Permission))
                return new CommandResponse { IsAuthorized = false };

            var validationErrors = await ValidateCommand(command);

            if (validationErrors.Any())
                return new CommandResponse { IsValid = false, ValidationErrors = validationErrors };

            var commandHandler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

            await commandHandler.Handle(command);

            return new CommandResponse();
        }

        public async Task<CommandResponse> DispatchCommand<TCommand, TResponse>(TCommand command) where TCommand : ICommand
        {
            if (!await _permissionService.HasPermission(command.Permission))
                return new CommandResponse { IsAuthorized = false };

            var validationErrors = await ValidateCommand(command);

            if (validationErrors.Any())
                return new CommandResponse { IsValid = false, ValidationErrors = validationErrors };

            var commandHandler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand,TResponse>>();

            return new CommandResponse<TResponse> { Response = await commandHandler.Handle(command) };
        }

        public async Task<QueryResult<TResponse>> DispatchQuery<TQuery, TResponse>(TQuery query) where TQuery : IQuery
        {
            if (!await _permissionService.HasPermission(query.Permission))
                return new QueryResult<TResponse> { IsAuthorized = false };
            
            var queryHandler = _serviceProvider.GetRequiredService<ICommandHandler<TQuery, TResponse>>();

            return new QueryResult<TResponse> { Response = await queryHandler.Handle(query) };
        }

        public virtual async Task<ValidationError[]> ValidateCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            var context = new ValidationContext(command, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(command, context, results, true);

            var validationErrors = results.Select(r => new ValidationError { Field = r.MemberNames.FirstOrDefault(), ErrorMessage = r.ErrorMessage }).ToArray();

            if (!validationErrors.Any())
                return validationErrors;

            var commandValidators = _serviceProvider.GetServices<ICommandValidator<TCommand>>().ToList();

            validationErrors = (await commandValidators.SelectManyAsync(v => v.Validate(command))).ToArray();

            return validationErrors;
        }
    }
}