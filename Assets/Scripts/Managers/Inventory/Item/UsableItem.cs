using UnityEngine;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;

[CreateAssetMenu(menuName = "Items/Usable Item - Skill")]
public class UsableItem : Item
{
    public GameObject itemAnimationObject;
    
    /// <summary>
    /// If an item is consumable, it will be removed after use.
    /// </summary>
    public bool isConsumable;

    //public List<UsableItemEffect> Effects;    // the original

    //public List<UsableItemEffectTwo> Effects = new List<UsableItemEffectTwo>();

    /// <summary>
    /// Time based effects will trigger every turn or second.
    /// </summary>
    public List<StatTimedEffect> TimedStatEffects;
    //[ContextMenuItem("Delete Effect", "DeleteEffectt")]

    /// <summary>
    /// Instant effects will trigger only once.
    /// </summary>
    public List<StatInstantEffect> InstantStatEffects;
    //[ContextMenuItem("Delete Effect", "DeleteEffectt")]

    /*private void DeleteEffectt()
    {
        //int index = System.Array.IndexOf(TimedStatEffects, )
        //TimedStatEffects.RemoveAt(mouse)
        for(int i = Effects.Count - 1; i >= 0; i--)
        {
            if(Effects[i].deleteMe)
            {
                if(Effects[i] is StatBuffItemEffectTwo)
                {
                    TimedStatEffects.Remove((StatBuffItemEffectTwo)Effects[i]);
                }
                else if(Effects[i] is StatInstantChange)
                {
                    InstantStatEffects.Remove((StatInstantChange)Effects[i]);
                }
                Effects.Remove(Effects[i]);
            }
        }
    }*/
    
    protected override void OnValidate() 
    {
        base.OnValidate();
        foreach (StatInstantEffect instantEffect in InstantStatEffects)
        {
            if(instantEffect.ID == "")
            {
                int value;
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    byte[] randomNumber = new byte[4];//4 for int32
                    rng.GetBytes(randomNumber);
                    value = BitConverter.ToInt32(randomNumber, 0);
                }
                instantEffect.Init("instant_effect." + value.ToString());
            }
        }
        foreach (StatTimedEffect timedEffect in TimedStatEffects)
        {
            if(timedEffect.ID == "")
            {
                int value;
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    byte[] randomNumber = new byte[4];//4 for int32
                    rng.GetBytes(randomNumber);
                    value = BitConverter.ToInt32(randomNumber, 0);
                }
                timedEffect.SetID("timed_effect." + value.ToString());
            }
        }
    }

    public virtual void Use(Character character)    // takes target character
    {
        List<UsableItemEffect> Effects = GetAllEffects();

        foreach (UsableItemEffect effect in Effects)
        {
            if(effect is StatTimedEffect)   // timed effect
            {
                StatTimedEffect tempEffect = new StatTimedEffect((StatTimedEffect)effect);

                if(character.CanAddStat(tempEffect))
                {
                    if(character.battleState != BattleState.Ended)
                        tempEffect.isInBattle = true;

                    character.AddStatEffect(tempEffect);
                    tempEffect.ExecuteEffect(this, character);
                }
                else
                {
                    // its gonna return false if this effect is already on targeted character, 
                    // what to do now? replace it? check which one is stronger?
                    // if im gonna simply replace it, might as well remove it from other and add it.
                }
            }
            else    // instant effect, no need to store it
                effect.ExecuteEffect(this, character);
        }

        if(itemAnimationObject != null)
        {
            Instantiate(itemAnimationObject, character.transform.position + (Vector3.one / 2), character.transform.rotation);
            if(Sound != null)
            {
                //character.audioSource.PlayOneShot(Sound);
                AudioSource.PlayClipAtPoint(Sound, character.transform.position);
            }
        }
    }

    public override Item GetCopy()
    {
        Debug.LogWarning("New object instantiated");
        return Instantiate(this);
    }

    public override string GetItemType()
    {
        return isConsumable ? "Consumable" : "Usable";
    }

    public override string GetDescription()
    {
        List<UsableItemEffect> Effects = GetAllEffects();

        sb.Length = 0;
        foreach (UsableItemEffect effect in Effects)
        {
            sb.AppendLine(effect.GetDescription());
        }
        return sb.ToString();
    }

    
    private List<UsableItemEffect> GetAllEffects()
    {
        List<UsableItemEffect> Effects = new List<UsableItemEffect>();
        foreach (StatInstantEffect instantEffect in InstantStatEffects)
        {
            Effects.Add(instantEffect);
        }
        foreach (StatTimedEffect timedEffect in TimedStatEffects)
        {
            Effects.Add(timedEffect);
        }
        return Effects;
    }
}
