#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UsableItem))]
public class UsableItemEditor : Editor
{
    private UsableItem _usableItem = null;

    void OnEnable()
    {
        _usableItem = (UsableItem)target;
        //GetTarget = new SerializedObject(target);
        //EffectsList = GetTarget.FindProperty("Effects");
    }
    
    /*public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        //objNames = EditorGUI.TextField(new Rect(10, 125, 10, 20), "New Names:", objNames);
        //StatBuffItemEffectTwo teststat = Selection.activeObject.name
        //if(test != null)
        //StatBuffItemEffectTwo obj = Selection.activeObject as StatBuffItemEffectTwo;
        
        if(EditorGUIUtility.hotControl != 0)
        {
            //string stat = EditorGUIUtility.GetStateObject(typeof(string), EditorGUIUtility.hotControl) as string;
            //UsableItem effect = target as UsableItem;
            //Selection.SetActiveObjectWithContext(Selection.activeObject as UsableItem, Selection.activeObject as StatBuffItemEffectTwo);
        }
        //UsableItem usableItem = Selection.activeObject as UsableItem;
        //target as StatBuffItemEffectTwo

        if(GUILayout.Button("Add Timed Stat Effect"))
        {
            UsableItem skill = target as UsableItem;

            string path = AssetDatabase.GetAssetPath(skill);
            StatBuffItemEffectTwo temp = new StatBuffItemEffectTwo("TimedStatEffect_" + AssetDatabase.AssetPathToGUID(path));

            skill.TimedStatEffects.Add(temp);
            skill.Effects.Add(temp);
        }
        else if(GUILayout.Button("Add Instant Stat Effect"))
        {
            UsableItem skill = target as UsableItem;
            string path = AssetDatabase.GetAssetPath(skill);
            StatInstantChange temp = new StatInstantChange();
            skill.Effects.Add(temp);
            skill.InstantStatEffects.Add(temp);
        }
        else if(GUILayout.Button("Remove Stat Effect"))
        {
            //EditorGUILayout.ObjectField(target, typeof(UsableItem))
            UsableItem skill = target as UsableItem;
            for(int i = skill.Effects.Count - 1; i >= 0; i--)
            {
                if(skill.Effects[i].deleteMe)
                {
                    if(skill.Effects[i] is StatBuffItemEffectTwo)
                    {
                        skill.TimedStatEffects.Remove((StatBuffItemEffectTwo)skill.Effects[i]);
                    }
                    else if(skill.Effects[i] is StatInstantChange)
                    {
                        skill.InstantStatEffects.Remove((StatInstantChange)skill.Effects[i]);
                    }
                    skill.Effects.Remove(skill.Effects[i]);
                }
            }
        }
    }*/

    /*
    enum displayFieldType {DisplayAsAutomaticFields, DisplayAsCustomizableGUIFields}
    displayFieldType DisplayFieldType;

    SerializedObject GetTarget;
    SerializedProperty EffectsList;
    int EffectsList_Size;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GetTarget.Update();
        //Choose how to display the list<> Example purposes only
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();

        DisplayFieldType = (displayFieldType)EditorGUILayout.EnumPopup("",DisplayFieldType);

        //Resize our list
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
        EditorGUILayout.LabelField("Define the list size with a number");

        EffectsList_Size = EffectsList.arraySize;
        EffectsList_Size = EditorGUILayout.IntField ("List Size", EffectsList_Size);

        if(EffectsList_Size != EffectsList.arraySize){
            while(EffectsList_Size > EffectsList.arraySize){
                EffectsList.InsertArrayElementAtIndex(EffectsList.arraySize);
            }
            while(EffectsList_Size < EffectsList.arraySize){
                EffectsList.DeleteArrayElementAtIndex(EffectsList.arraySize - 1);
            }
        }

        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
        EditorGUILayout.LabelField("Or");
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
   
        //Or add a new item to the List<> with a button
        EditorGUILayout.LabelField("Add a new item with a button");

        if(GUILayout.Button("Add Timed Stat Effect"))
        {
            UsableItem skill = target as UsableItem;

            string path = AssetDatabase.GetAssetPath(skill);
            StatBuffItemEffectTwo temp = new StatBuffItemEffectTwo("TimedStatEffect_" + AssetDatabase.AssetPathToGUID(path));

            skill.TimedStatEffects.Add(temp);
            skill.Effects.Add(temp);
            
            if(skill.Effects.Count > 0)
            {
                StatBuffItemEffectTwo test = skill.Effects[0] as StatBuffItemEffectTwo;
                Debug.Log(test.ID);
            }
        }

        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
   
        //Display our list to the inspector window
        for(int i = 0; i < EffectsList.arraySize; i++)
        {
            SerializedProperty MyListRef = EffectsList.GetArrayElementAtIndex(i);
        }
    }*/
}
#endif