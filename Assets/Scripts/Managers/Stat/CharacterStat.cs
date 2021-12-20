using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DungeonCrawler.CharacterStats
{
    [Serializable]
    public class CharacterStat
    {
        // This is a root stat, if this stat changes other stats will be notified with an event
        internal event Action AttributeChanged;

        // default value of stat
        [SerializeField] protected float _baseValue;
        public virtual float baseValue{
            get{return _baseValue;}
            set
            {
                if(_baseValue != value)
                {
                    _baseValue = value;
                    isDirty = true;
                    if(AttributeChanged != null)
                    {
                        AttributeChanged();
                    }
                }
            }
        }

        // calculated value of a stat which gets effected with buffs debuffs etc
        [SerializeField] protected float _calculatedValue;
        public virtual float calculatedValue { 
            get{
                if(isDirty){
                    _calculatedValue = CalculateFinalValue();
                    isDirty = false;
                    if(AttributeChanged != null)
                    {
                        AttributeChanged();
                    }
                }
                return _calculatedValue;
            }
        }

        protected bool isDirty = true;

        protected readonly List<StatModifier> statModifiers;
        public readonly ReadOnlyCollection<StatModifier> StatModifiers;

        public CharacterStat(){
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        public CharacterStat(float baseValue) : this(){
            this.baseValue = baseValue;
        }

        public virtual void AddModifier(StatModifier mod){
            isDirty = true;
            statModifiers.Add(mod);
            statModifiers.Sort(CompareModifierOrder);
        }

        public virtual bool RemoveModifier(StatModifier mod){
            if(statModifiers.Remove(mod)){
                isDirty = true;
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source){
            
            bool didRemove = false;

            for(int i = statModifiers.Count - 1; i >= 0; i--){
                if(statModifiers[i].source == source){
                    isDirty = true;
                    didRemove = true;
                    statModifiers.RemoveAt(i);
                }
            }
            return didRemove;
        }
        
        protected virtual int CompareModifierOrder(StatModifier A, StatModifier B){
            if(A.order < B.order)
                return -1;
            else if(A.order > B.order)
                return 1;
            return 0; // if (A.order == B.order)
        }

        protected virtual float CalculateFinalValue(){
            float finalValue = baseValue;
            float sumPercentAdd = 0;

            for(int i = 0; i < statModifiers.Count; i++){
                
                StatModifier mod = statModifiers[i];

                if(mod.type == StatModType.flat)
                {
                    finalValue += mod.value;
                }
                else if(mod.type == StatModType.percentAdd)
                {
                    sumPercentAdd += mod.value;
                    if(i + 1 >= statModifiers.Count || statModifiers[i + 1].type != StatModType.percentAdd){
                        finalValue *= 1 + sumPercentAdd / 100;
                        sumPercentAdd = 0;
                    }
                }
                else if(mod.type == StatModType.percentMult)
                {
                    finalValue *= 1 + mod.value / 100;
                }
            }

            // 12.0001f != 12f
            return (float)Math.Round(finalValue, 4);
        }
    }
}
