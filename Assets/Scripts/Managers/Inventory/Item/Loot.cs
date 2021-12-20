using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] Item item = null;
    [SerializeField] int amount = 1;
    [SerializeField] Inventory inventory = null;

    private bool canLoot = false;
    private bool isEmpty;

    private void OnValidate() {
        if(inventory == null)
            inventory = FindObjectOfType<Inventory>();
    }

    private void Update(){
        if(canLoot && !isEmpty && Input.GetKeyDown(KeyCode.E))
        {
            Item itemCopy = Instantiate(item.GetCopy());
            if(inventory.AddItem(itemCopy)){
                amount--;
                if(amount == 0)
                {
                    isEmpty = true;
                }
            }
            else    // if adding is not succesfull destroy the copy
            {
                itemCopy.Destroy();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
            canLoot = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player"))
            canLoot = false;
    }
}
