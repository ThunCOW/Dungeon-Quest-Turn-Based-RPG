using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipCompare : ItemTooltip
{
    [SerializeField] Image equippedItemImage = null;
    [SerializeField] Text equippeditemNameText = null;
    [SerializeField] Text equippeditemTypeText = null;
    [SerializeField] Text equippeditemDescriptionText = null;
    

    private void Start() {
        itemImage.color = normalColor;
        equippedItemImage.color = normalColor;
    }

    public override void ShowTooltip(Item item, Item equippedItem = null)
    {
        try
        {
            itemImage.sprite = item.Icon;
            itemNameText.text = item.Name;
            itemTypeText.text = item.GetItemType();
            itemDescriptionText.text = item.CompareDescription((EquippableItem)equippedItem);

            equippedItemImage.sprite = equippedItem.Icon;
            equippeditemNameText.text = equippedItem.Name;
            equippeditemTypeText.text = equippedItem.GetItemType();
            equippeditemDescriptionText.text = equippedItem.GetDescription();
            
            gameObject.SetActive(true);
        }
        catch
        {
            HideTooltip();
        }
    }
}
