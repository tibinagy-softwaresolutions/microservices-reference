using TNArch.Microservices.Core.Common.Command;

namespace TNArch.Microservices.Inventory.Core.Item
{
    public class ItemCommandHandlers :
        ICommandHandler<CreateItemCommand, CreateItemCommandResponse>,
        ICommandHandler<UpdateItemCommand>,
        ICommandHandler<DeleteItemCommand>
    {
        public Task<CreateItemCommandResponse> Handle(CreateItemCommand command)
        {
            //create item here

            return Task.FromResult(new CreateItemCommandResponse { ItemId = Guid.NewGuid() });
        }

        public Task Handle(UpdateItemCommand command)
        {
            //update item here

            return Task.CompletedTask;
        }

        public Task Handle(DeleteItemCommand command)
        {
            //delete item here

            return Task.CompletedTask;
        }
    }
}