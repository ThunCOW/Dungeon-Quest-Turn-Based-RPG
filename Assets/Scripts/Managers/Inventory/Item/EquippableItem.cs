using UnityEngine;
using DungeonCrawler.CharacterStats;
using UnityEngine.Serialization;

public enum EquipmentType{
    Helmet,
    Chest,
    Gloves,
    Boots,
    Weapon,
    Off_Hand,
    Accessory1,
    Accessory2,
}

[CreateAssetMenu(menuName = "Items/Equipabble Item")]
public class EquippableItem : Item
{
    [SerializeField] private int minDamage = 0;
    [SerializeField] private int maxDamage = 0;
    [Space]
    [SerializeField] private int strengthBonus = 0;
    [SerializeField] private int agilityBonus = 0;
    [SerializeField] private int intelligenceBonus = 0;
    [SerializeField] private int vitalityBonus = 0;
    [Space]
    [SerializeField] private float strengthPercentBonus = 0;
    [SerializeField] private float agilityPercentBonus = 0;
    [SerializeField] private float intelligencePercentBonus = 0;
    [SerializeField] private float vitalityPercentBonus = 0;
    [Space]
    [FormerlySerializedAs("itemType")]
    public EquipmentType equipmentType;

    public override Item GetCopy()
    {
        Debug.LogWarning("New object instantiated");
        return Instantiate(this);
    }

    public override void Destroy()
    {
        Destroy(this);
    }

    public void Equip(Character c)
    {
        if(strengthBonus != 0)
        {
            c.strength.AddModifier(new StatModifier(strengthBonus, StatModType.flat, this));
        }
        if(agilityBonus != 0)
        {
            c.agility.AddModifier(new StatModifier(agilityBonus, StatModType.flat, this));
        }
        if(intelligenceBonus != 0)
        {
            c.intelligence.AddModifier(new StatModifier(intelligenceBonus, StatModType.flat, this));
        }
        if(vitalityBonus != 0)
        {
            c.vitality.AddModifier(new StatModifier(vitalityBonus, StatModType.flat, this));
        }

        if(strengthPercentBonus != 0)
        {
            c.strength.AddModifier(new StatModifier(strengthPercentBonus, StatModType.percentAdd, this));
        }
        if(agilityPercentBonus != 0)
        {
            c.agility.AddModifier(new StatModifier(agilityPercentBonus, StatModType.percentAdd, this));
        }
        if(intelligencePercentBonus != 0)
        {
            c.intelligence.AddModifier(new StatModifier(intelligencePercentBonus, StatModType.percentAdd, this));
        }
        if(vitalityPercentBonus != 0)
        {
            c.vitality.AddModifier(new StatModifier(vitalityPercentBonus, StatModType.percentAdd, this));
        }

        if(minDamage != 0)
        {
            c.minDamage.AddModifier(new StatModifier(minDamage, StatModType.flat, this));
        }
        if(maxDamage != 0)
        {
            c.maxDamage.AddModifier(new StatModifier(maxDamage, StatModType.flat, this));
        }
    }

    public void Unequip(Character c)
    {
        c.strength.RemoveAllModifiersFromSource(this);
        c.agility.RemoveAllModifiersFromSource(this);
        c.intelligence.RemoveAllModifiersFromSource(this);
        c.vitality.RemoveAllModifiersFromSource(this);

        c.minDamage.RemoveAllModifiersFromSource(this);
        c.maxDamage.RemoveAllModifiersFromSource(this);
    }

    public override string GetItemType()
    {
        return equipmentType.ToString();
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        AddWeaponDamage(minDamage, maxDamage);
        AddStat(strengthBonus, "Strength");
        AddStat(agilityBonus, "Agility");
        AddStat(intelligenceBonus, "Intelligence");
        AddStat(vitalityBonus, "Vitality");

        AddStat(strengthPercentBonus, "Strength", isPercentage: true);
        AddStat(agilityPercentBonus, "Agility", isPercentage: true);
        AddStat(intelligencePercentBonus, "Intelligence", isPercentage: true);
        AddStat(vitalityPercentBonus, "Vitality", isPercentage: true);
        return sb.ToString();
    }

    public override string CompareDescription(EquippableItem compareItem = null)
    {
        sb.Length = 0;
        AddWeaponDamage(minDamage, maxDamage);
        CompareStat(strengthBonus, compareItem.strengthBonus, "Strength");
        CompareStat(agilityBonus, compareItem.agilityBonus, "Agility");
        CompareStat(intelligenceBonus, compareItem.intelligenceBonus, "Intelligence");
        CompareStat(vitalityBonus, compareItem.vitalityBonus, "Vitality");

        CompareStat(strengthPercentBonus, compareItem.strengthPercentBonus, "Strength", isPercentage: true);
        CompareStat(agilityPercentBonus, compareItem.agilityPercentBonus, "Agility", isPercentage: true);
        CompareStat(intelligencePercentBonus, compareItem.intelligencePercentBonus, "Intelligence", isPercentage: true);
        CompareStat(vitalityPercentBonus, compareItem.vitalityPercentBonus, "Vitality", isPercentage: true);
        return sb.ToString();
    }

    
    public void AddStat(float value, string statName, bool isPercentage = false){
        if(value != 0)
        {
            if(sb.Length > 0)
                sb.AppendLine();    // if this is not the first line, move down a row
            
            if(value > 0)           // if value is positive add plus sign
                sb.Append("+");

            if(isPercentage){
                sb.Append(value);
                sb.Append("% ");
            }else{
                sb.Append(value);
                sb.Append(" ");
            }
            sb.Append(statName);
        }
    }

    public void CompareStat(float value1, float value2, string statName, bool isPercentage = false){
        if(value1 != 0)
        {
            if(sb.Length > 0)
                sb.AppendLine();
            
            if(value1 > 0)
                sb.Append("");

            if(isPercentage){
                sb.Append(value1);
                sb.Append("% ");
                sb.Append(statName);
            }else{
                if(value2 > value1)
                    sb.Append("<color=#E44F4B>");
                sb.Append(value1);
                sb.Append(" ");
                sb.Append(statName);
                if(value2 > 0)
                {
                    if(value1 > value2)
                        sb.Append(" (+" + (value1 - value2) + ")");
                    else
                    {
                        sb.Append(" (-" + (value2 - value1) + ")");
                        sb.Append("</color>");
                    }
                }
            }
        }
    }

    private void AddWeaponDamage(float minDamage, float maxDamage){
        if(minDamage != 0)
        {
            if(sb.Length > 0)
                sb.AppendLine();    // if this is not the first line, move down a row
            
            sb.Append(minDamage + " - " + maxDamage + " Physical Damage");
        }
    }
}
