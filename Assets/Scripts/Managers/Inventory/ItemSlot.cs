using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : BaseItemSlot, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    // Actions will trigger when the item slot is right clicked, inventory and equipment manager
    // both implements them as they use itemSlots, then they are all being handled in Character script
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<BaseItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    private bool isDragging;
	private Color dragColor = new Color(1, 1, 1, 0.5f);

    /// <summary>
    /// If item has same ID with the item in slot, and stack in that slot is NOT full, it returns true.
    /// </summary>
    public override bool CanAddStack(Item item, int amount = 1)
	{
		return base.CanAddStack(item, amount) && Amount + amount <= item.MaximumStacks;
	}

    /// <summary>
    /// By default unlike BaseItemSlot objects, ItemSlot objects can be moved around.
    /// </summary>
    public override bool CanReceiveItem(Item item)
	{
		return true;
	}
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        if (item != null)
			image.color = dragColor;

        if(OnBeginDragEvent != null)
            OnBeginDragEvent(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

		if (item != null)
			image.color = normalColor;

        if(OnEndDragEvent != null)
            OnEndDragEvent(this);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if(OnDragEvent != null)
            OnDragEvent(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(OnDropEvent != null)
            OnDropEvent(this);
    }

    protected override void OnDisable()
	{
		base.OnDisable();

		if (isDragging)
			OnEndDrag(null);
	}
}
