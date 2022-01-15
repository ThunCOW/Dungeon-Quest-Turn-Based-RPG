using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Player : Character
{
    [Header("Skills")]
    [SerializeField] protected Item selectedSkillorUsable;
    [Space]
    [SerializeField] protected List<Item> characterSkills;

    [Header("Serialized Fields")]
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel equipmentPanel;
    [SerializeField] StatPanel statPanel;
    [SerializeField] ItemTooltip itemTooltip;
    [SerializeField] ItemTooltipCompare compareItemTooltip;
    [SerializeField] Image draggableItem = null;

    private BaseItemSlot dragItemSlot;

    protected override void OnValidate()
    {
        base.OnValidate();

        if(inventory == null)
            inventory = FindObjectOfType<Inventory>();
        if(equipmentPanel == null)
            equipmentPanel = FindObjectOfType<EquipmentPanel>();
        if(statPanel == null)
            statPanel = FindObjectOfType<StatPanel>();
        if(itemTooltip == null)
            itemTooltip = FindObjectOfType<ItemTooltip>();
        if(compareItemTooltip == null)
            compareItemTooltip = FindObjectOfType<ItemTooltipCompare>();
    }

    protected override void Awake()
    {
        base.Awake();

        StaticClass.inspectedCharacter = this;
        statPanel.SetStats(strength, agility, intelligence, vitality, minDamage, maxDamage);
        UpdateStatValues();

        // Setup Events:
        // Right Click:
        inventory.OnRightClickEvent += InventoryRightClick;
        equipmentPanel.OnRightClickEvent += UnequipFromEquipmentPanel;
        // Pointer Enter
        inventory.OnPointerEnterEvent += ShowTooltip;
        equipmentPanel.OnPointerEnterEvent += ShowTooltip;
        // Pointer Exit
        inventory.OnPointerExitEvent += HideTooltip;
        equipmentPanel.OnPointerExitEvent += HideTooltip;
        // Begin Drag
        inventory.OnBeginDragEvent += BeginDrag;
        equipmentPanel.OnBeginDragEvent += BeginDrag;
        // End Drag
        inventory.OnEndDragEvent += EndDrag;
        equipmentPanel.OnEndDragEvent += EndDrag;
        // Drag
        inventory.OnDragEvent += Drag;
        equipmentPanel.OnDragEvent += Drag;
        // Drop
        inventory.OnDropEvent += Drop;
        equipmentPanel.OnDropEvent += Drop;

        // Stat With Dependancies:
        // Damage Change
        minDamage.statToDependOn.AttributeChanged += minDamage.DependantStatChanged;
        maxDamage.statToDependOn.AttributeChanged += maxDamage.DependantStatChanged;
        // Health Change
        health.statToDependOn.AttributeChanged += health.DependantStatChanged;
    }

    protected override void Start()
    {
        base.Start();

        // Update/Apply equipped item stats at start
        foreach(CharacterEquipment characterEquipment in characterEquipments)
        {
            if(characterEquipment.item != null)
            {
                EquippableItem equippedItem = characterEquipment.item as EquippableItem;
                equippedItem.Equip(this);
                equipmentPanel.AddItem(equippedItem, out EquippableItem notNeeded);
                UpdateStatValues();
            }
        }
    }

    protected override void TurnActions()
    {
        characterMovement.MovementAction(movementPoint.currentStat);    // Movement Action is on characterMovement script
        Attack();
    }

    public void EndTurn()
    {
        if(battleState == BattleState.Active)
        {
            battleState = BattleState.Waiting;
        }
    }

    protected override void Attack()
    {
        MouseInputs();
    }

    void MouseInputs()
    {
        if(Input.GetMouseButtonDown(0) && !StaticClass.OnUI)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LayerMask mask = LayerMask.NameToLayer("Characters"); if((int)mask == -1) Debug.LogError("Incorrect Mask !");
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, 1 << mask);
            if(hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                Character targetedCharacter = hit.collider.gameObject.GetComponent<Character>();  // Targeted Character
                if(selectedSkillorUsable != null)
                {
                    if(selectedSkillorUsable is UsableItem)
                    {
                        animator.SetTrigger("Right");
                        UsableItem usableItem = selectedSkillorUsable as UsableItem;
                        usableItem.Use(targetedCharacter);
                    }
                }
                else
                {
                    Debug.LogError("No Skill Selected!");
                }
                //targetedCharacter.ChangeHealth(-minDamage.value);
            }
        }
    }

    /*
        Inventory input controllers
    */
    /*private void InventoryRightClick(BaseItemSlot itemSlot)
	{
		if (itemSlot.item is EquippableItem)
		{
			Equip((EquippableItem)itemSlot.item);
		}
		else if (itemSlot.item is UsableItem)
		{
			UsableItem usableItem = (UsableItem)itemSlot.item;
			usableItem.Use(this);

			if (usableItem.isConsumable)
			{
				itemSlot.Amount--;
				usableItem.Destroy();
			}
		}
	}*/

    private void InventoryRightClick(BaseItemSlot itemSlot)
    {
        Item item = itemSlot.item;
        if(item is EquippableItem)
        {
            EquippableItem equippableItem = item as EquippableItem;

            EquippableItem previousItem;
            if(equipmentPanel.AddItem(equippableItem, out previousItem))  // add it to the equipmentPanel
            {
                equipmentManager.AddItem(equippableItem);       // Add to characterEquipment through equipment manager
                if(previousItem != null)                        // if we had an item in the same slot equipped already
                {
                    inventory.SwapEquipmentItem(item, previousItem);     // return it back to inventory
                    previousItem.Unequip(this);
                    UpdateStatValues();
                }
                Debug.Log(inventory.RemoveItem(item));
                equippableItem.Equip(this);
                UpdateStatValues();
            }
        }
        else if(item is UsableItem)
        {
            UsableItem usableItem = (UsableItem)item;
            usableItem.Use(this);
            UpdateStatValues();

            if(usableItem.isConsumable)
            {
                inventory.RemoveItem(usableItem);
                usableItem.Destroy();
            }
        }
    }

    private void UnequipFromEquipmentPanel(BaseItemSlot itemSlot)
    {
        Item item = itemSlot.item;
        EquippableItem equippableItem = item as EquippableItem;
        if(inventory.CanAddItem(item) && equipmentPanel.RemoveItem(equippableItem))
        {
            equipmentManager.RemoveItem(equippableItem);       // Remove from characterEquipment through equipment manager
            inventory.AddItem(item);
            equippableItem.Unequip(this);
            UpdateStatValues();
        }
    }

    public void Equip(EquippableItem item)
	{
		if (inventory.RemoveItem(item))
		{
			EquippableItem previousItem;
			if (equipmentPanel.AddItem(item, out previousItem))
			{
				if (previousItem != null)
				{
					inventory.AddItem(previousItem);
					previousItem.Unequip(this);
					statPanel.UpdateStatValues();
				}
				item.Equip(this);
				statPanel.UpdateStatValues();
			}
			else
			{
				inventory.AddItem(item);
			}
		}
	}

	public void Unequip(EquippableItem item)
	{
		if (inventory.CanAddItem(item) && equipmentPanel.RemoveItem(item))
		{
			item.Unequip(this);
			statPanel.UpdateStatValues();
			inventory.AddItem(item);
		}
	}

    public override void ShowTooltip(BaseItemSlot itemSlot)
    {
        Item item = itemSlot.item;
        //isPointerOver = true;
        if(item is EquippableItem)
        {
            EquippableItem equippableItem = item as EquippableItem;
            EquippableItem equippedItem = equipmentPanel.GetEquipmentByType(equippableItem.equipmentType);
            if(equippedItem != null && equippedItem != item)
            {
                compareItemTooltip.ShowTooltip(item, equippedItem);
                compareItemTooltip.transform.position = itemSlot.transform.position;
            }
            else
            {
                itemTooltip.ShowTooltip(item);
                itemTooltip.transform.position = itemSlot.transform.position;
            }
        }
        else
        {
            itemTooltip.ShowTooltip(item);
            itemTooltip.transform.position = itemSlot.transform.position;
        }
    }

    public override void HideTooltip(BaseItemSlot itemSlot)
    {
        Item item = itemSlot.item;
        if(compareItemTooltip.gameObject.activeSelf)
            compareItemTooltip.HideTooltip();
        else if(itemTooltip.gameObject.activeSelf)
            itemTooltip.HideTooltip();
    }

    private void BeginDrag(BaseItemSlot itemSlot)
    {
        if(itemSlot.item != null)
        {
            dragItemSlot = itemSlot;
            draggableItem.sprite = itemSlot.item.Icon;
            //TODO: change later
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggableItem.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
            draggableItem.enabled = enabled;
        }
    }

    private void EndDrag(BaseItemSlot item)             // Runs After Drop
    {
        dragItemSlot = null;
        draggableItem.enabled = false;
    }

    private void Drag(BaseItemSlot itemSlot)    // while we are dragging
    {
        if(draggableItem.enabled)
        {
            //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //draggableItem.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
            draggableItem.transform.position = Input.mousePosition;
        }
    }

    private void Drop(BaseItemSlot dropItemSlot)        // Runs before EndDrag
    {
        if(dragItemSlot != null && dropItemSlot != null)
        {
            if(dropItemSlot.CanAddStack(dragItemSlot.item))             // Stacks
            {
                AddStacks(dropItemSlot);
            }
            else if(dropItemSlot.CanReceiveItem(dragItemSlot.item) && dragItemSlot.CanReceiveItem(dropItemSlot.item))
            {
                EquippableItem dragItem = dragItemSlot.item as EquippableItem;
                EquippableItem dropItem = dropItemSlot.item as EquippableItem;

                if(dragItemSlot is EquipmentSlot)
                {
                    //if(dragItem != null) dragItem.Unequip(this);
                    //if(dropItem != null) dropItem.Equip(this);
                    InventoryRightClick(dropItemSlot);
                }
                else if(dropItemSlot is EquipmentSlot)
                {
                    //if(dragItem != null) dragItem.Equip(this);
                    //if(dropItem != null) dropItem.Unequip(this);
                    InventoryRightClick(dragItemSlot);
                }
                else
                {
                    inventory.SwapInventoryItem(dragItemSlot as ItemSlot, dropItemSlot as ItemSlot);
                }
                UpdateStatValues();
            }
        }
    }

    private void AddStacks(BaseItemSlot dropItemSlot)
	{
		int numAddableStacks = dropItemSlot.item.MaximumStacks - dropItemSlot.Amount;
		int stacksToAdd = Mathf.Min(numAddableStacks, dragItemSlot.Amount);

		dropItemSlot.Amount += stacksToAdd;
		dragItemSlot.Amount -= stacksToAdd;
	}

    private void SwapItems(BaseItemSlot dropItemSlot)
	{
		EquippableItem dragEquipItem = dragItemSlot.item as EquippableItem;
		EquippableItem dropEquipItem = dropItemSlot.item as EquippableItem;

		if (dropItemSlot is EquipmentSlot)
		{
			if (dragEquipItem != null) dragEquipItem.Equip(this);
			if (dropEquipItem != null) dropEquipItem.Unequip(this);
		}
		if (dragItemSlot is EquipmentSlot)
		{
			if (dragEquipItem != null) dragEquipItem.Unequip(this);
			if (dropEquipItem != null) dropEquipItem.Equip(this);
		}
		statPanel.UpdateStatValues();

		Item draggedItem = dragItemSlot.item;
		int draggedItemAmount = dragItemSlot.Amount;

		dragItemSlot.item = dropItemSlot.item;
		dragItemSlot.Amount = dropItemSlot.Amount;

		dropItemSlot.item = draggedItem;
		dropItemSlot.Amount = draggedItemAmount;
	}


    public override void UpdateStatValues()
    {
        statPanel.UpdateStatValues();
        
        // So how it should is that every time we change weapon or put an item or use an item that increase our 
        // damage or the stat which increase our damage, we should update values in equipment panel
        // Updating values requires minDamage and maxDamage which we can send through character or directly min max variables
        equipmentPanel.BasicStats(minDamage, maxDamage, null);        // not pretty at all
    }
}
