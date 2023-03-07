using TNArch.Microservices.Core.Common.Command;

namespace TNArch.Microservices.Inventory.Core.Item
{
    public class ItemCommandValidators :
        ICommandValidator<CreateItemCommand>,
        ICommandValidator<UpdateItemCommand>
    {
        public Task<List<ValidationError>> Validate(CreateItemCommand command)
        {
            //contextual command validation here
            return Task.FromResult(new List<ValidationError>());
        }

        public Task<List<ValidationError>> Validate(UpdateItemCommand command)
        {
            //contextual command validation here
            return Task.FromResult(new List<ValidationError>());
        }
    }
}