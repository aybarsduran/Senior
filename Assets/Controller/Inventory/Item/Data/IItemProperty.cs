namespace IdenticalStudios.InventorySystem
{
    public interface IItemProperty
    {
        int Id { get; }
        string Name { get; }

        ItemPropertyType Type { get; }
        bool Boolean { get; set; }
        int Integer { get; set; }
        float Float { get; set; }
        int ItemId { get; set; }

        IItemProperty GetClone();

        event PropertyChangedCallback Changed;
    }

    public delegate void PropertyChangedCallback(IItemProperty property);
}