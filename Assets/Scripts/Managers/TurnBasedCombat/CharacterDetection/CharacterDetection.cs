using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterDetection : MonoBehaviour
{
    [SerializeField] Character currentCharacter;
    [SerializeField] TurnBasedBattleActivate turnBasedBattleActivate = null;

    private void OnValidate() {
        if(currentCharacter == null)
            currentCharacter = GetComponent<Character>();
        if(turnBasedBattleActivate == null)
            turnBasedBattleActivate = FindObjectOfType<TurnBasedBattleActivate>();
    }

    [Header("Serialized Fields")]
    public List<Character> charactersInRange;

    public bool AddCharacter(Character character)
    {
        if(!charactersInRange.Contains(character))
            charactersInRange.Add(character);

        // There is going to be neutral characters
        // Neutral to player or neutral to all ? or neutral to certain enemies ?
        // Right now it enters here for every object with collider other than objects with same tag as itself
        if(currentCharacter is Player)
            return true;    // player CAN'T start battle, only AI start battles, player has the right to get in range or move away.
        else
            AggroCheck(character);
        
        return true;
    }

    public bool RemoveCharacter(Character character)
    {
        charactersInRange.Remove(character);
        
        return true;
    }

    // Check for aggro every time characters moves
    // Check for all closest characters, 
    // Check how many of them is in your vision, 
    // Check if they are is in combat, if in then join their battle
    private void AggroCheck(Character targetedCharacter)
    {
        if(currentCharacter.battleState == BattleState.Ended)                // if we are NOT in combat
        {
            if(targetedCharacter.battleState == BattleState.Ended)  // if targeted character is NOT in combat
            {
                if(currentCharacter.turnBasedCombat == null) // This should be null, if it is not there is a problem since character is not in combat state
                {
                    if(!targetedCharacter.CompareTag(gameObject.tag))   // if targeted character is NOT in the same group as ours
                    {
                        // We should not enter here if both characters are in same group
                        turnBasedBattleActivate.StartCombat(currentCharacter, targetedCharacter);
                        CallCloseCharactersToBattle();

                        CharacterDetection temp = targetedCharacter.GetComponent<CharacterDetection>();
                        temp.CheckCharactersInRange();
                    }
                }
                else
                {
                    Debug.LogError(this.name + " character is not in combat but connected to a battle!\nFailed to start combat!");
                }
            }
            else    // if target is in combat already, this character joins too
            {
                targetedCharacter.turnBasedCombat.JoinBattle(currentCharacter);
                CallCloseCharactersToBattle();
            }
        }
        else                                                // if we ARE in combat
        {
            // Do nothing for now, targeted character can join battle itself when it runs it's own script
        }

        for(int i = 0; i < charactersInRange.Count; i++)
        {
            if(charactersInRange[i].battleState == BattleState.Ended)   // if character in range is not in combat,
            {
                // this can't be targeted character since it is already joined/started combat
                // the other way to handle this is that all ai character share their vision with each other, 
                // that way when one of them sees player all of them sees at the same time.
                //charactersInRange[i].AggroCheck(targetedCharacter);
            }
        }
    }

    private void CheckCharactersInRange()
    {
        foreach(Character targetedCharacter in charactersInRange)
        {
            AggroCheck(targetedCharacter);
        }
    }

    public LayerMask layerMask;
    // This list will hold units that are around but not within our 
    void CallCloseCharactersToBattle()
    {
        List<CharacterDetection> possibleDetections = new List<CharacterDetection>();

        Vector3 pos = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z);
        //LayerMask layerMask = LayerMask.NameToLayer("Characters");

        Collider2D[] collisions = Physics2D.OverlapCircleAll(pos, 5, layerMask);

        foreach (Collider2D coll in collisions)
        {
            CharacterDetection charInRange = coll.GetComponent<CharacterDetection>();
            if(charInRange != this)   // do not detect yourself
                possibleDetections.Add(charInRange);
        }

        foreach (CharacterDetection characterDetection in possibleDetections)
        {
            characterDetection.CheckCharactersInRange();
        }
    }
}
