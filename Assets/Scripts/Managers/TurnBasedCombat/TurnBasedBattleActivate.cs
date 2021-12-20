using UnityEngine;

public class TurnBasedBattleActivate : MonoBehaviour
{
    [SerializeField] GameObject combatObjects = null;
    [Space]
    [SerializeField] GameObject turnBasedCombatPrefab = null;
    //public List<GameObject> activeBattles = new List<GameObject>();

    //[Space] [SerializeField] List<Character> characterInCombat = new List<Character>();

    public void StartCombat(Character attackingCharacter, Character targetedCharacter)
    {
        GameObject combat = (GameObject)Instantiate(turnBasedCombatPrefab);
        combat.transform.parent = combatObjects.transform;

        // Just so that player character is written first in go name
        if(attackingCharacter is Player)
            combat.name = attackingCharacter.name + " VS " + targetedCharacter.name;
        else
            combat.name = targetedCharacter.name + " VS " + attackingCharacter.name;

        TurnBasedCombat activeCombat = combat.GetComponent<TurnBasedCombat>();
        activeCombat.InitCombat(attackingCharacter, targetedCharacter);
        
        //return activeCombat.JoinBattle(attackingCharacter);
    }
}