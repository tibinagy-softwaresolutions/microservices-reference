using TNArch.Microservices.Core.Common.Command;

namespace TNArch.Microservices.Inventory.Core.Item
{
    public class CreateItemCommand : ICommand
    {
        public string Permission => "Demo.Items.Create";
        public string Name { get; set; }
        public ItemType Type { get; set; }
    }

    public class GetItemsByNameQuery : IQuery
    {
        public string Permission => "Demo.Items.Read";
        public string Name { get; set; }
        public Item Item { get; set; }
    }

    public enum ItemType
    {
        Type1,
        Type2,
        Type3
    }

    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string[] Descriptions { get; set; }
        public ItemType Type { get; set; }
    }
}