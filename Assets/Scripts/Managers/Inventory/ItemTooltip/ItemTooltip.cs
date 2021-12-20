using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected Text itemNameText;
    [SerializeField] protected Text itemTypeText;
    [SerializeField] protected Text itemDescriptionText;

    protected Color normalColor = Color.white;

    private void Start() {
        itemImage.color = normalColor;
    }

    public virtual void ShowTooltip(Item item, Item equippedItem = null)
    {
        try
        {
            itemImage.sprite = item.Icon;
            itemNameText.text = item.Name;
            itemTypeText.text = item.GetItemType();
            itemDescriptionText.text = item.GetDescription();

            gameObject.SetActive(true);
        }
        catch
        {
            HideTooltip();
        }
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
