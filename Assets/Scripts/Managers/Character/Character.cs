using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using DungeonCrawler.CharacterStats;

/*
https://pavcreations.com/level-systems-and-character-growth-in-rpg-games/
https://forum.unity.com/threads/tutorial-character-stats-aka-attributes-system.504095/
https://gamedevelopment.tutsplus.com/tutorials/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243
https://www.google.com/search?q=unity+rpg+games+how+to+change+health+with+stats&oq=unity+rpg+games+how+to+change+health+with+stats&aqs=chrome..69i57.33319j0j1&sourceid=chrome&ie=UTF-8
*/
public class Character : MonoBehaviour
{
    public delegate void EndTurnAction();
    public event EndTurnAction EndTurnEvent;

    [SerializeField] private BattleState _battleState;
    public BattleState battleState{
        get{return _battleState;}
        set
        {
            // battle started
            if(_battleState == BattleState.Ended && value != BattleState.Ended)
            {
                characterMovement.ClearPath();
                
                foreach (StatTimedEffect effect in activeEffects)
                {
                    effect.isInBattle = true;
                }
            }
            // battle ended
            else if(value == BattleState.Ended)
            {
                foreach (StatTimedEffect effect in activeEffects)
                {
                    effect.isInBattle = false;
                }

                if(EndTurnEvent != null)
                    EndTurnEvent();
            }
            // turn ended
            else if(_battleState == BattleState.Active && (value == BattleState.Waiting || value == BattleState.Ended))
            {
                if(EndTurnEvent != null)
                    EndTurnEvent();
            }
            _battleState = value;
        }
    }
    public TurnBasedCombat turnBasedCombat;

    // Events for characters
    //[SerializeField] UnityEvent OnDeath = null;
    //public event Action<Character> InCombat;

    [Header("Basic Stats")]
    public FlexibleStat health;

    public StatWithDepency minDamage;
    public StatWithDepency maxDamage;

    [Space]
    [SerializeField] protected float _experience;
    public float experience{
        get{return _experience;}
        set
        {
            _experience = value;
        }
    }

    public float currentExperience;

    [Header("Action Points")]
    public FlexibleStat movementPoint;
    public FlexibleStat offensivePoints;
    public CharacterStat initiative;

    [Header("Character Stats")]
    public CharacterStat strength;
    public CharacterStat agility;
    public CharacterStat intelligence;
    public CharacterStat vitality;
    public FlexibleStat hitChance;

    [Header("Active Effects")]
    [SerializeField] private List<StatTimedEffect> buffs = new List<StatTimedEffect>();
    [SerializeField] private List<StatTimedEffect> debuffs = new List<StatTimedEffect>();
    private List<StatTimedEffect> activeEffects = new List<StatTimedEffect>();

    //[SerializeField] protected List<Character> charactersInRange;

    //[SerializeField] TurnBasedBattleActivate turnBasedBattleActivate = null;
    
    // Private Declerations
    protected CharacterMovement characterMovement;
    protected CharacterEquipmentManager equipmentManager;
    protected CharacterEquipment[] characterEquipments;

    protected virtual void OnValidate()
    {
        
    }

    protected virtual void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        // Setup Events:
        // On Character Death
        //OnDeath += turnBasedCombat.CharacterDeath;

        health.statToDependOn = vitality;
        minDamage.statToDependOn = strength;
        maxDamage.statToDependOn = strength;
    }

    protected virtual void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        equipmentManager = GetComponent<CharacterEquipmentManager>();
        characterEquipments = GetComponentsInChildren<CharacterEquipment>();
    }

    // This function is played ONLY when character is set to active by combat system, 
    // and returns false WHEN character decides to end turn.
    public virtual bool ActiveTurn()                            // Functions that requires a check for every frame
    {
        if(battleState != BattleState.Ended)                    // In Combat
        {
            if(battleState == BattleState.Active)               // Ready to Play
            {
                TurnActions();
            }
            else if(battleState == BattleState.Waiting)         // End Turn function is triggered
            {
                return false;
            }
        }
        else
        {
            Debug.Log("Character is not in combat!");
        }
        return true;
    }

    // This is the actions a character is capable to make in one turn
    protected virtual void TurnActions(){}

    public bool invis;
    protected virtual void Update()
    {
        //Debug.Log(health.calculatedValue);
        if(battleState == BattleState.Ended)                    // Out Of Combat
        {
            characterMovement.MovementAction(movementPoint.currentStat);                 // Can Move
        }
        else                                                    // In Combat
        {
            if(invis == true && turnBasedCombat != null)
            {
                turnBasedCombat.LeaveBattle(this);
            }
        }
    }
    
    /*protected virtual void LevelUp()
    {
        maxHealth.baseValue += 20;
        maxHealth.isDirty = true;

        minDamage.baseValue += 7;
        minDamage.isDirty = true;

        maxDamage.baseValue += 7;
        maxDamage.isDirty = true;
    }*/

    // Tooltip functions
    public virtual void ShowTooltip(BaseItemSlot itemSlot){}
    public virtual void HideTooltip(BaseItemSlot itemSlot){}
    public virtual void UpdateStatValues(){}

    public virtual void ChangeHealth(float damage)
    {
        health.currentStat += damage;
        if(health.currentStat == 0 && battleState != BattleState.Ended)
        {
            if(turnBasedCombat != null)
            {
                turnBasedCombat.LeaveBattle(this);
            }
            else
            {
                Debug.Log(this.name + " character either died outside of combat OR there is a problem");
            }
        }
    }

    [HideInInspector] protected Animator animator;
    [HideInInspector] public AudioSource audioSource;
    protected virtual void Attack()
    {
        
    }


    // Add Stat Effects
    public void AddStatEffect(StatTimedEffect effect)
    {
        if(activeEffects.Count > 0)
        {
            if(activeEffects.Contains(effect))
            {
                Debug.Log("This Effect Already On");
            }
            else
            {
                activeEffects.Add(effect);
                if(effect.effectType == EffectType.Buff  || effect.effectType == EffectType.Regeneration)
                    buffs.Add(effect);
                else
                    debuffs.Add(effect);
            }
        }
        else    // No Effect On Character
        {
            activeEffects.Add(effect);
            if(effect.effectType == EffectType.Buff  || effect.effectType == EffectType.Regeneration)
                buffs.Add(effect);
            else
                debuffs.Add(effect);
        }
    }
    public void RemoveStatEffect(StatTimedEffect effect)
    {
        if(activeEffects.Contains(effect))
        {
            activeEffects.Remove(effect);
            if(effect.effectType == EffectType.Buff || effect.effectType == EffectType.Regeneration)
                buffs.Remove(effect);
            else
                debuffs.Remove(effect);
        }
        else
        {
            Debug.LogError("This effect could not be found!");
        }
    }
    public bool CanAddStat(StatTimedEffect effect)
    {
        foreach (StatTimedEffect activeEffect in activeEffects)
        {
            if(activeEffect.ID == effect.ID)
            {
                activeEffect.Duration = 0;  // this triggers it to remove
                activeEffect.isInBattle = true; // this prevents it from playing first applyEffect for first courutine
                // TODO: for now i remove it to replace it with newly applied effect, so it returns true here.
                return true;
            }
        }
        return true;
    }
}
