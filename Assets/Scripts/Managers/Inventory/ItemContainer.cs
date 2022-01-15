using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemContainer : MonoBehaviour//, IItemContainer
{
    [SerializeField] protected ItemSlot[] itemSlots;
    [SerializeField] List<Item> items = null;

    public virtual bool AddItem(Item item)
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].item == null)                                       // Found an empty(null) slot
            {
                items.Add(item);
                itemSlots[i].Amount++;
                //RefreshUI();
                return true;
            }
            else if(itemSlots[i].item.ID == item.ID && itemSlots[i].Amount < item.MaximumStacks)
            {
                itemSlots[i].Amount++;
                return true;
            }
        }
        return false;
    }
}
