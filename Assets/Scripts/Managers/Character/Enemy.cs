using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Enemy : Character
{
    [Header("Skills")]
    [SerializeField] protected List<Item> firstChoiceSkills = new List<Item>();
    [SerializeField] protected List<Item> secondChoiceSkills = new List<Item>();
    [SerializeField] protected List<Item> thirdChoiceSkills = new List<Item>();

    protected override void TurnActions()
    {
        characterMovement.MovementAction(movementPoint.currentStat);    // Movement Action is on characterMovement script
    }

    // End turn when Ai played all its actions, all courutines ended etc
    // so we need a list of Ai actions and at the end of list we end turn maybe?
    void EndTurn()
    {
        if(battleState == BattleState.Active)
        {
            battleState = BattleState.Waiting;
        }
    }

    protected override void Attack()
    {
        
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(Item))]
public class SkillPropertyDrawer : PropertyDrawer
{
    private const int spriteHeight = 50;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);

        if(property.isExpanded)
        {
            Debug.Log("works");
            SerializedProperty iconSkill = property.FindPropertyRelative("Icon");
            Sprite icon = (Sprite) iconSkill.objectReferenceValue;
            if (icon != null) 
            {
                int previousIndentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 2;

                Rect indentedRect = EditorGUI.IndentedRect (position);
                float fieldHeight = base.GetPropertyHeight (property, label) + 2;
                Vector3 enemySize = icon.bounds.size;
                Rect texturePosition = new Rect(indentedRect.x, indentedRect.y + fieldHeight * 4, enemySize.x / enemySize.y * spriteHeight, spriteHeight);

                EditorGUI.DropShadowLabel(texturePosition, new GUIContent(icon.texture));

                EditorGUI.indentLevel = previousIndentLevel;
            }
        }
    }

    /*public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
    {
        SerializedProperty iconSkill = property.FindPropertyRelative("Icon");
        Sprite icon = (Sprite) iconSkill.objectReferenceValue;
        
        if (property.isExpanded && iconSkill != null) 
        {
            return EditorGUI.GetPropertyHeight (property) + spriteHeight;	
        } 
        else
        {
            return EditorGUI.GetPropertyHeight (property);
        }
    }*/
}
#endif