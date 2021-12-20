using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : ItemSlot
{
    public EquipmentType equipmentType;
    
    [SerializeField] private Text equipmentNameText = null;

    private void Start() 
    {
        try{
            equipmentNameText.text = "";
        }catch{}
    }

    public override Item item
    {
        get{return _item;}
        set{
            _item = value;
            if(_item == null && Amount != 0) Amount = 0;
            if(_item == null)
            {
                image.color = disabledColor;
                try{
                    equipmentNameText.text = "";
                }catch{}
            }
            else
            {
                image.sprite = _item.Icon;
                image.color = normalColor;

                try{
                    equipmentNameText.text = item.Name;
                }catch{}
            }
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = equipmentType.ToString() + " Slot";
    }

    public override bool CanReceiveItem(Item item)
    {
        if(item == null)
            return true;

        EquippableItem equippableItem = item as EquippableItem;
        return equippableItem != null && equippableItem.equipmentType == equipmentType;
    }
}
