using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipmentManager : MonoBehaviour
{
    [SerializeField] private CharacterEquipment[] characterEquipments;

    private void OnValidate()
    {
        //characterEquipments = GetComponentsInChildren<CharacterEquipment>();
    }

    private void Start() 
    {
        characterEquipments = GetComponentsInChildren<CharacterEquipment>();
        // Update/Apply equipped item stats at start
        /*foreach(CharacterEquipment characterEquipment in characterEquipments)
        {
            if(characterEquipment.item != null)
            {
                EquippableItem equippedItem = characterEquipment.item as EquippableItem;
                Character character = GetComponent<Character>();
                equippedItem.Equip(character);
                character.UpdateStatValues();
            }
        }*/
    }
    
    public void AddItem(EquippableItem equippableItem)
    {
        foreach(CharacterEquipment characterEquipment in characterEquipments)
        {
            if(characterEquipment.equipmentType == equippableItem.equipmentType)
            {
                characterEquipment.item = equippableItem;
            }
        }
    }

    public void RemoveItem(EquippableItem equippableItem)
    {
        foreach(CharacterEquipment characterEquipment in characterEquipments)
        {
            if(characterEquipment.equipmentType == equippableItem.equipmentType)
            {
                characterEquipment.item = null;
            }
        }
    }
}
