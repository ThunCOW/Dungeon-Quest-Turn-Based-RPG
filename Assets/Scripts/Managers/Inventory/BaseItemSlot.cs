using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaseItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image image = null;
	[SerializeField] protected Text amountText = null;

    // Actions will trigger when the item slot is right clicked, inventory and equipment manager
    // both implements them as they use itemSlots, then they are all being handled in Character script
	public event Action<BaseItemSlot> OnPointerEnterEvent;
	public event Action<BaseItemSlot> OnPointerExitEvent;
	public event Action<BaseItemSlot> OnRightClickEvent;

	protected bool isPointerOver;

	protected Color normalColor = Color.white;
	protected Color disabledColor = Color.clear;

	protected Item _item;
	public virtual Item item{
		get{return _item;}
		set
        {
            _item = value;

            if (_item == null && Amount != 0) Amount = 0;
            if (_item == null)
            {
                image.sprite = null;
                image.color = disabledColor;
            }
            else
            {
                image.sprite = _item.Icon;
                image.color = normalColor;
            }

            if (isPointerOver)
            {
                OnPointerExit(null);
                OnPointerEnter(null);
            }
		}
	}

	private int _amount;
	public int Amount{
		get{return _amount;}
		set
        {
			_amount = value;
			if (_amount < 0) _amount = 0;
			if (_amount == 0 && item != null) item = null;	// Some functions may require to check for item value before we set it

			if (amountText != null)
			{
				amountText.enabled = _item != null && _amount > 1;
				if (amountText.enabled)
                {
					amountText.text = _amount.ToString();
				}
			}
		}
	}

    /// <summary>
    /// If item has same ID with the item in slot, it returns true. If current class is ItemSlot class, it also checks for amount.
    /// </summary>
	public virtual bool CanAddStack(Item item, int amount = 1)
	{
		return this.item != null && this.item.ID == item.ID;
	}

    /// <summary>
    /// By default BaseItemSlot can't receive item, ItemSlot can.
    /// </summary>
	public virtual bool CanReceiveItem(Item item)
	{
		return false;
	}

	protected virtual void OnValidate()
	{
		if (image == null)
			image = GetComponent<Image>();

		if (amountText == null)
			amountText = GetComponentInChildren<Text>();

		item = _item;
		Amount = _amount;
	}

	protected virtual void OnDisable()
	{
		if (isPointerOver)
			OnPointerExit(null);
	}

	public void OnPointerClick(PointerEventData eventData)      // Registers on exit
	{
		if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
		{
			if (OnRightClickEvent != null)
            {
				OnRightClickEvent(this);
                //TODO: probably gonna break game later, fix it bitch
                OnPointerExitEvent(this);
                OnPointerEnterEvent(this);
            }
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isPointerOver = true;

		if (OnPointerEnterEvent != null)
			OnPointerEnterEvent(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isPointerOver = false;

		if (OnPointerExitEvent != null)
			OnPointerExitEvent(this);
	}
}
