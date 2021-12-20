using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Stat Direct Change")]
public class StatDirectChange : UsableItemEffectOld
{
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

}