using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.Serialization;

public class Inventory : ItemContainer
{
    [SerializeField] List<Item> startingItems = null;
    [SerializeField] Transform itemsParent = null;

    protected override void OnValidate()
    {
        if(itemsParent != null)
			itemsParent.GetComponentsInChildren(includeInactive: true, result: itemSlots);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        SetStartingItems();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetStartingItems();
    }

    private void SetStartingItems()
    {
        int i = 0;
        for(; i < startingItems.Count && i < itemSlots.Count; i++)
        {
            startingItems[i] = startingItems[i].GetCopy();
        }

        for(i = 0; i < startingItems.Count && i < itemSlots.Count; i++)
        {
            AddItem(startingItems[i]);
        }

        /*for(i = items.Count; i < itemSlots.Count; i++)
        {
            itemSlots[i].item = null;
            itemSlots[i].Amount = 0;
        }*/
        
        for(i = startingItems.Count - 1; i >= 0; i--)
        {
            startingItems.RemoveAt(i);
        }
    }

    /// <summary>
    /// Finds equippedItem's index and replaces item on that index with unequipped item.
    /// </summary>
    public void SwapEquipmentItem(BaseItemSlot equippedItemPreviousSlot, Item unequippedItem)
    {
        items[items.FindIndex(ind=>ind.Equals(equippedItemPreviousSlot.item))] = unequippedItem;
        equippedItemPreviousSlot.item = unequippedItem;
        //ReOrderItemContainer();
    }

    public void SwapInventoryItem(ItemSlot dragItemSlot, ItemSlot dropItemSlot)
    {
        /*if(dragItemSlot.item != null && dropItemSlot.item != null)
        {
            int dropItemIndex = items.FindIndex(ind=>ind.Equals(dropItemSlot.item));
            items[items.FindIndex(ind=>ind.Equals(dragItemSlot.item))] = dropItemSlot.item;
            items[dropItemIndex] = dragItemSlot.item;

            //if(dragItemSlot.item.ID == dropItemSlot.item.ID)         // Stacking will happen, might implement later
            int dragItemAmount = dragItemSlot.Amount;
            dragItemSlot.Amount = dropItemSlot.Amount;
            dropItemSlot.Amount = dragItemAmount;
            ReOrderItemContainer();
        }*/

        if(dropItemSlot.item != null)
        {
            int dropItemIndex = items.FindIndex(ind=>ind.Equals(dropItemSlot.item));                    // Find dropItem
            items[items.FindIndex(ind=>ind.Equals(dragItemSlot.item))] = dropItemSlot.item;
            items[dropItemIndex] = dragItemSlot.item;
        }

        //if(dragItemSlot.item.ID == dropItemSlot.item.ID)         // Stacking will happen, might implement later
        int dropItemAmount = dropItemSlot.Amount;
        dropItemSlot.Amount = dragItemSlot.Amount;

        Item dragItem = dragItemSlot.item;
        dragItemSlot.item = dropItemSlot.item;
        dropItemSlot.item = dragItem;
        
        dragItemSlot.Amount = dropItemAmount;
    }
}