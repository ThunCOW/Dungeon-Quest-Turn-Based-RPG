using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler , IDragHandler, IDropHandler
{
    [SerializeField] protected Image image = null;
    [SerializeField] Text amountText = null;

    protected bool isPointerOver = false;
    
    // Actions will trigger when the item slot is right clicked, inventory and equipment manager
    // both implements them as they use itemSlots, then they are all being handled in Character script
    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<Item> OnPointerExitEvent;
    public event Action<Item> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<Item> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    protected Color normalColor = Color.white;
    protected Color disabledColor = Color.clear;
    
    protected Item _item;
    public virtual Item item{
        get{return _item;}
        set{
            _item = value;
            if(_item == null && Amount != 0) Amount = 0;
            if(_item == null){
                image.color = disabledColor;
            }else{
                image.sprite = _item.Icon;
                image.color = normalColor;
            }

            if(isPointerOver)
            {
                OnPointerExit(null);
                OnPointerEnter(null);
            }
        }
    }

    private int _amount;
    public int Amount{
        get{return _amount;}
        set{
            _amount = value;
            if(_amount < 0) _amount = 0;
            //amountText.enabled = _item != null && item.MaximumStacks > 1 && _amount > 1;
            amountText.text = _amount.ToString();
            //if(amountText.enabled)
        }
    }

    protected virtual void OnValidate() {
        if(image == null)
            image = GetComponent<Image>();
        if(amountText == null)
            amountText = GetComponentInChildren<Text>();
    }

    public virtual bool CanReceiveItem(Item item)
    {
        return true;
    }

    /*protected virtual void OnDisable() 
    {
        if(isPointerOver)
            OnPointerExit(null);
    }*/

    public void OnPointerClick(PointerEventData eventData)  // Registers on exit
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null && OnRightClickEvent != null)
            {
                OnRightClickEvent(item);
                //TODO: probably gonna break game later, fix it bitch
                OnPointerExitEvent(item);
                OnPointerEnterEvent(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(OnPointerEnterEvent != null)
            OnPointerEnterEvent(this);
        //isPointerOver = true;
        //itemTooltip.ShowTooltip(item);
        //if(item != null)
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(OnPointerExitEvent != null)
            OnPointerExitEvent(item);
        //isPointerOver = false;
        //if(itemTooltip.gameObject.activeSelf)
        //    itemTooltip.HideTooltip();
    }

    Vector2 originalPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(OnBeginDragEvent != null)
            OnBeginDragEvent(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(OnEndDragEvent != null)
            OnEndDragEvent(this);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if(OnDragEvent != null)
            OnDragEvent(item);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(OnDropEvent != null)
            OnDropEvent(this);
    }
}
