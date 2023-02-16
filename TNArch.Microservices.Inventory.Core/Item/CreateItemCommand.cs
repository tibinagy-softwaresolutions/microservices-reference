using TNArch.Microservices.Core.Common.Command;

namespace TNArch.Microservices.Inventory.Core.Item
{
    public class CreateItemCommand : ICommand
    {
        public string Permission => "Demo.Items.Create";
        public string Name { get; set; }
        public ItemType Type { get; set; }
    }

    public enum ItemType
    {
        Type1,
        Type2,
        Type3
    }
}