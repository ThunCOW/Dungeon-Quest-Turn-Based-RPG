
public interface IItemContainer
{
    bool AddItem(Item item);
    bool CanAddItem(Item item, int amount = 1);
    
    bool RemoveItem(Item item);
    Item RemoveItem(string itemID);
    
    //bool ContainsItem(Item item);
    int ItemCount(string itemID);
}
