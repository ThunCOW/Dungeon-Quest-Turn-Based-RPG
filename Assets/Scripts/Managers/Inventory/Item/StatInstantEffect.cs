using UnityEngine;

[System.Serializable]
public class StatInstantEffect : UsableItemEffect
{
    public string name = "name";
    [SerializeField] string id = "0";
    public string ID {get{return id;}}
    public Sprite Icon;

    [SerializeField] StatType statType = StatType.Health;
    [SerializeField] private int Amount = 0;

    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        if(statType == StatType.Health)
            character.health.currentStat += Amount;
    }

    public override string GetDescription()
    {
        return "Heals for" + Amount + " health.";
    }

    public void Init(string id)
    {
        this.id = id;
    }
}
