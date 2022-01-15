using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemContainer : MonoBehaviour//, IItemContainer
{
    public event Action<BaseItemSlot> OnPointerEnterEvent;
	public event Action<BaseItemSlot> OnPointerExitEvent;
	public event Action<BaseItemSlot> OnRightClickEvent;
	public event Action<BaseItemSlot> OnBeginDragEvent;
	public event Action<BaseItemSlot> OnEndDragEvent;
	public event Action<BaseItemSlot> OnDragEvent;
	public event Action<BaseItemSlot> OnDropEvent;

    [SerializeField] protected List<ItemSlot> itemSlots;
    [SerializeField] protected List<Item> items = null;

    protected virtual void OnValidate()
	{
		GetComponentsInChildren<ItemSlot>(includeInactive: true, result: itemSlots);
	}

	protected virtual void Awake()
	{
		for (int i = 0; i < itemSlots.Count; i++)
		{
			itemSlots[i].OnPointerEnterEvent += slot => EventHelper(slot, OnPointerEnterEvent);
			itemSlots[i].OnPointerExitEvent += slot => EventHelper(slot, OnPointerExitEvent);
			itemSlots[i].OnRightClickEvent += slot => EventHelper(slot, OnRightClickEvent);
			itemSlots[i].OnBeginDragEvent += slot => EventHelper(slot, OnBeginDragEvent);
			itemSlots[i].OnEndDragEvent += slot => EventHelper(slot, OnEndDragEvent);
			itemSlots[i].OnDragEvent += slot => EventHelper(slot, OnDragEvent);
			itemSlots[i].OnDropEvent += slot => EventHelper(slot, OnDropEvent);
		}
	}

    protected virtual void Start(){}
    protected virtual void OnEnable(){}
    protected virtual void OnDisable(){}

	private void EventHelper(BaseItemSlot itemSlot, Action<BaseItemSlot> action)
	{
		if (action != null)
			action(itemSlot);
	}

    public virtual bool CanAddItem(Item item, int amount = 1)
	{
		int freeSpaces = 0;

		foreach (ItemSlot itemSlot in itemSlots)
		{
			if (itemSlot.item == null || itemSlot.item.ID == item.ID)
			{
				freeSpaces += item.MaximumStacks - itemSlot.Amount;
			}
		}
		return freeSpaces >= amount;
	}

    public virtual bool AddItem(Item item)
    {
        // If this item already exists, we will attempt to add it to stack
        for (int i = 0; i < itemSlots.Count; i++)
		{
			if (itemSlots[i].CanAddStack(item))
			{
                // Since this new item is a clone(assuming it is just created) it fails to match object comparison later
				//itemSlots[i].item = item;
				itemSlots[i].Amount++;
                //RefreshUI();
				return true;
			}
		}

        // If its a new item, item is not stackable or stack is full
		for (int i = 0; i < itemSlots.Count; i++)
		{
			if (itemSlots[i].item == null)
			{
                items.Add(item);
				itemSlots[i].item = item;
				itemSlots[i].Amount++;
                //RefreshUI();
				return true;
			}
		}
        return false;
    }

    public bool RemoveItem(Item item)
    {
        for(int i = 0; i < itemSlots.Count; i++)
        {
            if(itemSlots[i].item == item)
            {
                itemSlots[i].Amount--;
                if(itemSlots[i].Amount == 0)
                {
                    int y = items.IndexOf(item);                                // Store index of item in items list to know from what point Amount variable will be changed
                    if(items.Remove(item))                                      // Remove current item from items list
                    {
                        for(; y < items.Count && y < itemSlots.Count; y++)     // Get Amount information of next item slot
                        {
                            //itemSlots[y].item = items[i];
                            itemSlots[y].Amount = itemSlots[y + 1].Amount;
                        }
                        RefreshUI();                                            // Item is removed, refresh UI to see changes
                    }else
                        return false;
                }
                return true;
            }
        }
        return false;
    }

    public Item RemoveItem(string itemID)
    {
        for(int i = 0; i < itemSlots.Count; i++)
        {
            Item item = itemSlots[i].item;
            if(item != null && item.ID == itemID)
            {
                itemSlots[i].Amount--;
                if(itemSlots[i].Amount == 0)
                {
                    int y = items.IndexOf(item);                                // Store index of item in items list to know from what point Amount variable will be changed
                    if(items.Remove(item))                                      // Remove current item from items list
                    {
                        for(; y < items.Count && y < itemSlots.Count; y++)      // Get Amount information of next item slot
                        {
                            itemSlots[y].Amount = itemSlots[y + 1].Amount;
                        }
                        RefreshUI();                                          // Item is removed, refresh UI to see changes
                    }else
                        return null;
                }
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// This function is to make sure that there is no space between slots by being called everytime when one item is changed.
    /// </summary>
    protected virtual void RefreshUI()
    {
        int i = 0;
        for(; i < items.Count && i < itemSlots.Count; i++)
        {
            itemSlots[i].item = items[i];
        }

        for(; i < itemSlots.Count; i++)
        {
            itemSlots[i].item = null;
        }
    }

    /*public virtual bool RemoveItem(Item item)
	{
		for (int i = 0; i < itemSlots.Count; i++)
		{
			if (itemSlots[i].item == item)
			{
				itemSlots[i].Amount--;
				return true;
			}
		}
		return false;
	}*/

    public int ItemCount(string itemID)
    {
        int number = 0;
        for(int i = 0; i < itemSlots.Count; i++)
        {
            if(itemSlots[i].item.ID == itemID)
                number += itemSlots[i].Amount;
        }
        return number;
    }
}
