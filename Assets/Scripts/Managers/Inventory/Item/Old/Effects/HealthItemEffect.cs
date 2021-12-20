using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Health Potion")]
public class HealthItemEffect : UsableItemEffectOld
{
    [SerializeField] private int HealthAmount = 0;

    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        character.health.currentStat += HealthAmount;
    }

    public override string GetDescription()
    {
        return "Heals for" + HealthAmount + " health.";
    }
}
