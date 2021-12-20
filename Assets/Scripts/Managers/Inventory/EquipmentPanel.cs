using UnityEngine;
using UnityEngine.UI;
using System;

public class EquipmentPanel : MonoBehaviour
{
    [SerializeField] Transform equipmentSlotsParent = null;
    [SerializeField] EquipmentSlot[] equipmentSlots;
    
    [SerializeField] Text damageText = null;
    [SerializeField] Text armorText = null;

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<Item> OnPointerExitEvent;
    public event Action<Item> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<Item> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    private void Start()
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            equipmentSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            equipmentSlots[i].OnRightClickEvent += OnRightClickEvent;
            equipmentSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            equipmentSlots[i].OnEndDragEvent += OnEndDragEvent;
            equipmentSlots[i].OnDragEvent += OnDragEvent;
            equipmentSlots[i].OnDropEvent += OnDropEvent;
        }
    }
    
    private void OnValidate()
    {
        equipmentSlots = equipmentSlotsParent.GetComponentsInChildren<EquipmentSlot>();
    }

    /*private void OnEnable()
    {
        OpenEquipmentPanel();
    }

    public void OpenEquipmentPanel()
    {
        Player character = StaticClass.inspectedCharacter as Player;
        foreach(CharacterEquipment equipment in character.characterEquipments)
        {
            foreach(EquipmentSlot equipmentSlot in equipmentSlots)
            {
                if(equipmentSlot.equipmentType == equipment.equipmentType)
                {
                    equipmentSlot.item = equipment.item;
                    break;
                }
            }
        }
    }

    private void OnDisable()
    {
        foreach(EquipmentSlot equipmentSlot in equipmentSlots)
        {
            equipmentSlot.item = null;
        }
    }**/

    public bool AddItem(EquippableItem equippableItem, out EquippableItem previousItem)
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            if(equipmentSlots[i].equipmentType == equippableItem.equipmentType)  // equip it to right slot
            {
                previousItem = (EquippableItem)equipmentSlots[i].item;  // we will return previous item back to inventory
                equipmentSlots[i].item = equippableItem;
                return true;
            }
        }
        previousItem = null;
        return false;
    }

    public bool RemoveItem(EquippableItem item)
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            if(equipmentSlots[i].item == item)
            {
                equipmentSlots[i].item = null;
                return true;
            }
        }
        return false;
    }

    public EquippableItem GetEquipmentByType(EquipmentType itemType)
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            if(equipmentSlots[i].equipmentType == itemType)
            {
                return equipmentSlots[i].item as EquippableItem;
            }
        }
        return null;
    }

    public bool BasicStats(StatWithDepency minDamage, StatWithDepency maxDamage, StatWithDepency armor)
    {
        damageText.text = "Damage: " + minDamage.calculatedValue + " - " + maxDamage.calculatedValue;
        armorText.text = "works";
        return true;
    }
}
