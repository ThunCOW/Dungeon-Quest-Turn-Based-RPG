using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DungeonCrawler.CharacterStats;

public enum EffectType{
    Poison,
    Bleed,
    Burn,
    Regeneration,
    Buff,
    Debuff,
}

[System.Serializable]
public class StatTimedEffect : UsableItemEffect
{
    public string name = "name";
    [SerializeField] string id = "";
    public string ID {get{return id;}}
    public Sprite Icon;

    public EffectType effectType;
    [SerializeField] private StatType statItEffects = StatType.Health;
    
    private Character targetCharacter;  // this is needed so we can remove effect from duration set func (since we cant pass character to it)
    private StatModifier statModifier;  // this is only needed when timed effect is a status effect like stat buff or debuff 
    [SerializeField] private int Amount = 0;
    [SerializeField] private int _Duration = 0;
    public int Duration{
        get{return _Duration;}
        set{
            if(targetCharacter != null) // A character is targeted ( means this is not just created yet, it is active on a character )
            {
                if(value == 0)  // when hits zero, remove this effect
                {
                    targetCharacter.EndTurnEvent -= ApplyEffect;    // Remove listener
                    // Remove Element From List
                    if(effectType != EffectType.Buff && effectType != EffectType.Debuff)
                    {
                        // Poison, Bleed, Burn
                    }
                    else
                    {
                        targetStat.RemoveModifier(statModifier);
                        targetCharacter.UpdateStatValues();
                    }
                    targetCharacter.RemoveStatEffect(this);
                }
            }
            _Duration = value;
        }
    }
    public bool _isInBattle = false;
    public bool isInBattle{
        get{return _isInBattle;}
        set{
            // we are exiting battle
            if(_isInBattle == true && value == false)
            {
                targetCharacter.StartCoroutine(DecreaseDurationWithTime());
            }
            _isInBattle = value;
        }
    }

    public StatTimedEffect(StatTimedEffect effect)
    {
        this.name = effect.name;
        this.id = effect.id;
        this.Icon = effect.Icon;
        this.effectType = effect.effectType;
        this.statItEffects = effect.statItEffects;
        this.Amount = effect.Amount;
        this.Duration = effect.Duration;
        this.isInBattle = effect.isInBattle;
        if(Duration <= 0 || Amount <= 0)
            Debug.LogError("This skill is not set properly: " + name + " id: " + id);
    }
    /*public StatTimedEffect(string id)
    {
        this.id = id;
    }*/
    public void SetID(string id)
    {
        this.id = id;
    }

    //public int AgilityBuff;
    //public float Duration;

    public CharacterStat targetStat;

    // This method is called only ONCE for every use of a skill, it sets target character.
    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        targetCharacter = character;
        targetCharacter.EndTurnEvent += ApplyEffect;

        if(effectType == EffectType.Buff || effectType == EffectType.Debuff)
        {
            targetCharacter.EndTurnEvent += ApplyEffect;
            if(targetStat == null)   // get reference when used for first time
            {
                if(statItEffects == StatType.Strength)
                    targetStat = targetCharacter.strength;
                else if(statItEffects == StatType.Agility)
                    targetStat = targetCharacter.agility;
                else if(statItEffects == StatType.Intelligence)
                    targetStat = targetCharacter.intelligence;
                else if(statItEffects == StatType.Vitality)
                    targetStat = targetCharacter.vitality;
                else if(statItEffects == StatType.Health)
                    targetStat = targetCharacter.health;
            }
            
            statModifier = new StatModifier(Amount, StatModType.flat, parentItem);
            //character.strength.AddModifier(statModifier);
            targetStat.AddModifier(statModifier);
            targetCharacter.UpdateStatValues();
            //targetCharacter.StartCoroutine(RemoveBuff(targetCharacter, statModifier, Duration));
        }
        targetCharacter.StartCoroutine(DecreaseDurationWithTime());
    }

    public override string GetDescription()
    {
        return "Grants " + Amount + " " + statItEffects.ToString() + " for " + Duration + " seconds.";
    }

    /*private IEnumerator RemoveBuff(Character character, StatModifier statModifier, float duration)
    {
        yield return new WaitForSeconds(duration);
        targetStat[0].RemoveModifier(statModifier);
        //character.agility.RemoveModifier(statModifier);
        character.UpdateStatValues();
    }*/

    private IEnumerator DecreaseDurationWithTime()
    {
        yield return new WaitForSeconds(1);
        
        if(isInBattle)
            yield break;
        
        ApplyEffect();

        if(!isInBattle && Duration > 0)
        {
            targetCharacter.StartCoroutine(DecreaseDurationWithTime());
        }
    }

    private void ApplyEffect()
    {
        if(effectType == EffectType.Regeneration)
        {
            // Heal ticks, regen mana - hp
            targetCharacter.ChangeHealth(Amount);
        }
        else if(effectType != EffectType.Buff && effectType != EffectType.Debuff)
        {
            // Poison, Bleed, Burn
            targetCharacter.ChangeHealth(-Amount);
        }
        else    // this is a status effect like increase str or debuff for 10 sec
        {
            // Nothing to apply since it already is applied
        }
        targetCharacter.UpdateStatValues();
        
        Duration--;
    }
}