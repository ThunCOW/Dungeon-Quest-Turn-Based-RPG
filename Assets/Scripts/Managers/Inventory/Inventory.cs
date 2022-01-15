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

        for(i = items.Count; i < itemSlots.Count; i++)
        {
            itemSlots[i].item = null;
            itemSlots[i].Amount = 0;
        }
        
        for(i = startingItems.Count - 1; i >= 0; i--)
        {
            startingItems.RemoveAt(i);
        }
    }

    public void SwapEquipmentItem(Item firstItem, Item secondItem)
    {
        items[items.FindIndex(ind=>ind.Equals(firstItem))] = secondItem;
        RefreshUI();
    }

    public void SwapInventoryItem(ItemSlot firstItemSlot, ItemSlot secondItemSlot)
    {
        if(firstItemSlot.item != null && secondItemSlot.item != null)
        {
            int secondIndex = items.FindIndex(ind=>ind.Equals(secondItemSlot.item));
            items[items.FindIndex(ind=>ind.Equals(firstItemSlot.item))] = secondItemSlot.item;
            items[secondIndex] = firstItemSlot.item;

            //if(firstItemSlot.item.ID == secondItemSlot.item.ID)         // Stacking will happen, might implement later
            int tempAmount = firstItemSlot.Amount;
            firstItemSlot.Amount = secondItemSlot.Amount;
            secondItemSlot.Amount = tempAmount;
            RefreshUI();
        }
    }
}