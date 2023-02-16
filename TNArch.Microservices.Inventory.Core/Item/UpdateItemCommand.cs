using TNArch.Microservices.Core.Common.Command;

namespace TNArch.Microservices.Inventory.Core.Item
{
    public class UpdateItemCommand : ICommand
    {
        public string Permission => "Demo.Items.Update";
        public string Name { get; set; }
    }
}