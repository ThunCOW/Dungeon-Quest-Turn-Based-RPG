using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState{
    Waiting,
    Active,
    Ended,
}

[Serializable]
public class TurnBasedCombat : MonoBehaviour
{
    [SerializeField] List<Character> charactersInCombat = new List<Character>();
    [SerializeField] List<Character> playerCharacters = new List<Character>();
    [SerializeField] List<Character> aiCharacters = new List<Character>();
    [SerializeField] List<Character> sortedPlayList;

    [Space]
    [SerializeField] Character activeCharacter = null;
    [SerializeField] int turn;
    [SerializeField] bool battleStarted = false;

    private string Player = "Player";
    private string AI_Hostile_1 = "AI Hostile 1";

    public void InitCombat(Character attackingCharacter, Character targetedCharacter)
    {
        JoinBattle(attackingCharacter);
        JoinBattle(targetedCharacter);

        StartCoroutine(StartCombat());
    }

    IEnumerator StartCombat()
    {
        yield return new WaitForSeconds(1);
        turn = 0;
        //SortCharacterTurn();
        if(playerCharacters.Count > 0 && aiCharacters.Count > 0)
        {
            battleStarted = true;
            activeCharacter = sortedPlayList[0];
            activeCharacter.battleState = BattleState.Active;
        }
        else
        {
            Debug.LogError("Characters Failed To Join Battle!");
        }
    }

    private void Update()
    {
        if(battleStarted)
        {
            if(playerCharacters.Count > 0 && aiCharacters.Count > 0)    // Both side needs to have at least one character to continue, otherwise game ends 
            {
                if(!activeCharacter.ActiveTurn() && activeCharacter != null) // ActiveTurn should return false when character's turn is over
                {
                    EndCharacterTurn();
                }
                else if(activeCharacter == null)
                {
                    Debug.LogError("No character is set to Active to play turn!");
                }
            }
            else    // if either one side dies completely, end battle
            {
                EndCombat();
            }
        }
    }

    private void EndCharacterTurn()
    {
        sortedPlayList.Remove(activeCharacter);
        if(sortedPlayList.Count != 0)                       // IF there is more character left to play for this battle turn
        {
            activeCharacter = sortedPlayList[0];                // Assuming active character is in first slot after past character got removed from sorted play list
        }
        else        // Else is when sortedPlayList is empty( in theory this happens when a battle turn for every character is passed )
        {
            SortCharacterTurn();
            turn++; // increase battle turn
        }
        
        activeCharacter.battleState = BattleState.Active;   // New active character is set to active now
    }

    private void EndCombat()
    {
        for(int i = 0; i < charactersInCombat.Count; i++)
        {
            charactersInCombat[i].battleState = BattleState.Ended;
            charactersInCombat[i].turnBasedCombat = null;
        }
        Destroy(this.gameObject);       // Memory of this script is not freed until references are dropped from characters
    }

    // When one of two character is in range of aggro, they should start a turn based battle system and populate list
    // This requires characters in game to know each other when they aggro, if a character goes in combat other should too,
    // They should have reference to newly created battle and call JoinBattle function.
    public void JoinBattle(Character c)
    {
        if(c.CompareTag(Player))
        {
            playerCharacters.Add(c);
        }
        else if(c.CompareTag(AI_Hostile_1))
        {
            aiCharacters.Add(c);
        }
        c.battleState = BattleState.Waiting;
        
        sortedPlayList.Add(c);
        charactersInCombat.Add(c);
        
        // what if a character faster than us enters combat in the middle of combat?
        sortedPlayList = charactersInCombat.OrderByDescending(o=>o.initiative.calculatedValue).ToList();

        c.turnBasedCombat = this;
        //return this;
    }

    public void LeaveBattle(Character c)             // if a character dies or goes out of combat some how
    {
        if(c.CompareTag(Player))
        {
            playerCharacters.Remove(c);             // Remove it from players list
        }
        else if(c.CompareTag(AI_Hostile_1))
        {
            aiCharacters.Remove(c);                 // Remove it from ai list 
        }

        charactersInCombat.Remove(c);               // Remove it from general list
        sortedPlayList.Remove(c);                   // Remove it from sorted list
        if(activeCharacter = c)                           // if it was the active character, set a new character
        {
            activeCharacter = sortedPlayList[0];
        }

        c.battleState = BattleState.Ended;
        c.turnBasedCombat = null;
    }

    private void SortCharacterTurn()                     // if new character joins battle sort play order again
    {
        sortedPlayList = charactersInCombat.OrderBy(o=>o.initiative.calculatedValue).ToList();
        for(int i = 1; i < sortedPlayList.Count - 1; i++)
        {
            sortedPlayList[i].battleState = BattleState.Waiting;
        }
        activeCharacter = sortedPlayList[0];
    }
}
