using TNArch.Microservices.Core.Common.Command;

namespace TNArch.Microservices.Inventory.Core.Item
{
    public class ItemCommandHandlers :
        ICommandHandler<CreateItemCommand, CreateItemCommandResponse>,
        ICommandHandler<UpdateItemCommand>,
        ICommandHandler<DeleteItemCommand>,
        IQueryHandler<GetItemsByNameQuery, List<Item>>
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

        public Task<List<Item>> Handle(GetItemsByNameQuery query)
        {
            return Task.FromResult(new List<Item> { new Item { Id = Guid.NewGuid(), Name = "N1" }, new Item { Id = Guid.NewGuid(), Name = "N2" } });
        }
    }
}