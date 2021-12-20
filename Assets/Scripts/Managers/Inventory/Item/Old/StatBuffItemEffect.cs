using DungeonCrawler.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Item Effects/Stat Buff")]
public class StatBuffItemEffect : UsableItemEffectOld
{
    [SerializeField] string id;
    public string ID {get{return id;}}
    public Sprite Icon;


    [SerializeField] private StatType statType = StatType.Health;
    [SerializeField] private int Amount = 0;
    [SerializeField] private int Duration = 0;


    public int testInt;

    //public int AgilityBuff;
    //public float Duration;

    // This list can hold reference to stat type i want to use, temporary until i find a way or remember
    [SerializeField] List<CharacterStat> targetStat = new List<CharacterStat>();

    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        if(statType == StatType.Health)
        {
            //Debug.Log(character.health.value);
            //character.ChangeHealth(-Amount);
            //character.AddStatEffect(this);
            //character.harmEffects.Add(this);    // instead we should hold instance of this object.
            //Debug.Log(character.health.value);
            //character.StartCoroutine(DecreaseWithTime(character, Amount, Duration));
        }
        else
        {
            if(targetStat.Count == 0)   // get reference when used for first time
            {
                if(statType == StatType.Strength)
                    targetStat.Add(character.strength);
                else if(statType == StatType.Agility)
                    targetStat.Add(character.agility);
                else if(statType == StatType.Intelligence)
                    targetStat.Add(character.intelligence);
                else if(statType == StatType.Vitality)
                    targetStat.Add(character.vitality);
            }
            
            StatModifier statModifier = statModifier = new StatModifier(Amount, StatModType.flat, parentItem);
            //character.strength.AddModifier(statModifier);
            targetStat[0].AddModifier(statModifier);
            character.StartCoroutine(RemoveBuff(character, statModifier, Duration));
            character.UpdateStatValues();
        }
    }

    public override string GetDescription()
    {
        return "Grants " + Amount + " " + statType.ToString() + " for " + Duration + " seconds.";
    }

    private IEnumerator RemoveBuff(Character character, StatModifier statModifier, float duration)
    {
        yield return new WaitForSeconds(duration);
        targetStat[0].RemoveModifier(statModifier);
        //character.agility.RemoveModifier(statModifier);
        character.UpdateStatValues();
    }

    private IEnumerator DecreaseWithTime(Character character, float amount, float duration)
    {
        if(duration <= 0)
        {
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(1);
            character.ChangeHealth(-amount);
            character.UpdateStatValues();
            character.StartCoroutine(DecreaseWithTime(character, amount, duration - 1));
        }
    }

    private void OnValidate() 
    {
        targetStat.Clear();
        #if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
        #endif
    }
}