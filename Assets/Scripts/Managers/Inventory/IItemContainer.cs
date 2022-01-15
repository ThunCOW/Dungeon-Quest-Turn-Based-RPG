
public interface IItemContainer
{
    int ItemCount(string itemID);
    Item RemoveItem(string itemID);
    //bool ContainsItem(Item item);
    bool CanAddItem(Item item, int amount = 1);
    bool RemoveItem(Item item);
    bool AddItem(Item item);
}
