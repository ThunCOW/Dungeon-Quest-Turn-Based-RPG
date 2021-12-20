using UnityEngine;
using DungeonCrawler.CharacterStats;
using System;

public enum StatDependancy{
    None,
    Strength,
    Agility,
    Intelligence,
    Vitality,
}

[Serializable]
public class StatWithDepency : CharacterStat
{
    [HideInInspector] public StatDependancy _statDependancy;
    public StatDependancy statDependancy{
        get{return _statDependancy;}
        set
        {
            /*GameObject go = GameObject.FindGameObjectWithTag("anan");
            Character c = go.GetComponent<Character>();
            this.statToDependOn = c.health;*/
            _statDependancy = value;
        }
    }
    [NonSerialized] public CharacterStat statToDependOn;    // class this stat dependant on
    [SerializeField] private float multiplier = 1;  // multiplier for dependancy, 

    public override float calculatedValue { 
        get{
            if(isDirty){
                _calculatedValue = CalculateFinalValue() + statToDependOn.calculatedValue * multiplier;
                isDirty = false;
            }
            return _calculatedValue;
        }
    }

    public void DependantStatChanged() {
        isDirty = true;
    }
}
