using TNArch.Microservices.Core.Common.Command;

namespace TNArch.Microservices.Inventory.Core.Item
{
    public class DeleteItemCommand : ICommand
    {
        public string Permission => "Demo.Items.Delete";
        public string Name { get; set; }
    }
}