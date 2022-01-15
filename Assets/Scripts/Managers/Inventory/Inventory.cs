using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.Serialization;

public class Inventory : MonoBehaviour, IItemContainer
{
    //[FormerlySerializedAs("items")]
    [SerializeField] List<Item> startingItems = null;
    [SerializeField] List<Item> items = null;
    [SerializeField] Transform itemsParent = null;
    [SerializeField] ItemSlot[] itemSlots = null;

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<Item> OnPointerExitEvent;
    public event Action<Item> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<Item> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    private void Awake()
    {

    }

    private void Start()
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            itemSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            itemSlots[i].OnRightClickEvent += OnRightClickEvent;
            itemSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            itemSlots[i].OnEndDragEvent += OnEndDragEvent;
            itemSlots[i].OnDragEvent += OnDragEvent;
            itemSlots[i].OnDropEvent += OnDropEvent;
        }
        SetStartingItems();
    }

    private void OnValidate()
    {
        if(itemsParent != null)
        {
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
        }
    }

    private void SetStartingItems()
    {
        int i = 0;
        for(; i < startingItems.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].item = startingItems[i].GetCopy();
            itemSlots[i].Amount = 1;
            items.Add(itemSlots[i].item);
        }

        for(; i < itemSlots.Length; i++)
        {
            itemSlots[i].item = null;
            itemSlots[i].Amount = 0;
        }
        
        for(i = startingItems.Count - 1; i >= 0; i--)
        {
            startingItems.RemoveAt(i);
        }
    }

    private void RefreshUI()
    {
        int i = 0;
        for(; i < items.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].item = items[i];
        }

        for(; i < itemSlots.Length; i++)
        {
            itemSlots[i].item = null;
        }
    }

    public bool AddItem(Item item)
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].item == null)                                       // Found an empty(null) slot
            {
                items.Add(item);
                itemSlots[i].Amount++;
                RefreshUI();
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

    public bool RemoveItem(Item item)
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].item == item)
            {
                itemSlots[i].Amount--;
                if(itemSlots[i].Amount == 0)
                {
                    int y = items.IndexOf(item);                                // Store index of item in items list to know from what point Amount variable will be changed
                    if(items.Remove(item)){                                     // Remove current item from items list
                        for(; y < items.Count && y < itemSlots.Length; y++)     // Get Amount information of next item slot
                        {
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

    public Item RemoveItem(string itemID){
        for(int i = 0; i < itemSlots.Length; i++)
        {
            Item item = itemSlots[i].item;
            if(item != null && item.ID == itemID)
            {
                itemSlots[i].Amount--;
                if(itemSlots[i].Amount == 0)
                {
                    int y = items.IndexOf(item);                                // Store index of item in items list to know from what point Amount variable will be changed
                    if(items.Remove(item)){                                     // Remove current item from items list
                        for(; y < items.Count && y < itemSlots.Length; y++)     // Get Amount information of next item slot
                        {
                            itemSlots[y].Amount = itemSlots[y + 1].Amount;
                        }
                        RefreshUI();                                            // Item is removed, refresh UI to see changes
                    }else
                        return null;
                }
                return item;
            }
        }
        return null;
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
    
    public bool CanAddItem(Item item, int amount = 1)
    {
        int freeSpaces = 0;

        foreach (ItemSlot itemSlot in itemSlots)
        {
            if(itemSlot.item == null || itemSlot.item.ID == item.ID)
            {
                freeSpaces += item.MaximumStacks - itemSlot.Amount;
            }
        }

        return freeSpaces >= amount;
    }

    public bool ContainsItem(Item item)
    {
        return items.Contains(item);
    }

    public int ItemCount(string itemID)
    {
        int number = 0;
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].item.ID == itemID)
                number += itemSlots[i].Amount;
        }
        return number;
    }
}

/*
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    //[FormerlySerializedAs("items")]
    [SerializeField] List<Item> items;
    [SerializeField] Transform itemsParent;
    [SerializeField] ItemSlot[] itemSlots;

    public event Action<Item> OnItemRightClickedEvent;

    private void Awake() {
        for(int i = 0; i < itemSlots.Length; i++){
            itemSlots[i].OnRightClickEvent += OnItemRightClickedEvent;  // adding listeners
        }
    }

    private void Start() {
        StartingItems();
    }

    private void OnValidate() {
        if(itemsParent != null){
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
        }
        StartingItems();
    }

    private void StartingItems(){
        int i = 0;
        for(; i < items.Count && i < itemSlots.Length; i++){
            itemSlots[i].item = Instantiate(items[i]);
        }

        for(; i < itemSlots.Length; i++){
            itemSlots[i].item = null;
        }
    }

    public bool AddItem(Item item){
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].item == null)       // if found an empty slot, add item and return true
            {
                itemSlots[i].item = item;
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(Item item, out int position){
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].item == item)       // if found the item in inventory, remove it and return true
            {
                itemSlots[i].item = null;
                position = i;
                return true;
            }
        }
        position = -1;
        return false;
    }

    public void SwapItem(Item equippedItem, Item unequippedItem){
        items[items.FindIndex(ind=>ind.Equals(equippedItem))] = unequippedItem;
    }
    
    public bool IsFull(){
        
    }
}
*/
