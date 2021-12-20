#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Reflection;

public class CharacterEditor : EditorWindow
{
    [MenuItem ("Window/Character Viewer")]
    public static void  ShowWindow () 
    {
        EditorWindow.GetWindow(typeof(CharacterEditor));
    }

    protected static readonly StringBuilder sb = new StringBuilder();
    Vector2 textDimensions;
    float objectWidth;
    Vector2 characterInspectorScrollPos = Vector2.zero;
    Vector2 characterHierarcyScrollPos = Vector2.zero;
    
    List<string>[] charactersPrefabPath = new List<string>[5];
    bool[] toggleHierarcy = new bool[5];
    string[] characterTypes = {"Ghosts", "Humanoid", "Nature", "Skeletons", "Undeads"};
    private void OnGUI()
    {
        SerializedObject serializedCharacterObject = new SerializedObject(character);
        SerializedProperty sp;

        // Character Hierarcy
        Rect charactersHierarcyRect = new Rect(0, 0, 275, Screen.height);
        GUILayout.BeginArea(charactersHierarcyRect, EditorStyles.helpBox);
        characterHierarcyScrollPos = GUILayout.BeginScrollView(characterHierarcyScrollPos);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            int iButtonWidth = 70;

            GUILayout.BeginHorizontal();
            GUILayout.Space(charactersHierarcyRect.width - 75);
            if(GUILayout.Button("Refresh"))
            {
                for(int i = 0; i < toggleHierarcy.Length; i++)
                {
                    List<string> tempList = new List<string>();

                    sb.Length = 0;
                    string path = sb.Append("Assets/Prefabs/Characters/").ToString();

                    sb.Length = 0;
                    path += sb.Append(characterTypes[i]);
                    string[] assetsPaths = AssetDatabase.GetAllAssetPaths();
                    foreach(string assetPath in assetsPaths) 
                    {
                        if(assetPath.Contains(path) && !tempList.Contains(assetPath))
                        {
                            tempList.Add(assetPath);
                        }
                    }

                    if(charactersPrefabPath[i] == null)
                        charactersPrefabPath[i] = new List<string>();
                    else
                        charactersPrefabPath[i].Clear();

                    charactersPrefabPath[i] = tempList;
                }
            }
            GUILayout.EndHorizontal();
            
            int calculateSpaceForNextHierarcy = 0;
            for(int i = 0; i < toggleHierarcy.Length; i++)
            {
                GUILayout.BeginHorizontal();
                //GUILayout.Space(charactersHierarcyRect.width/2 - iButtonWidth /2);  // this middles one button with iButtonWidth size
                toggleHierarcy[i] = GUILayout.Toggle(toggleHierarcy[i], characterTypes[i], buttonStyle, GUILayout.Width(iButtonWidth));
                GUILayout.EndHorizontal();

                if(toggleHierarcy[i])
                {
                    int count = 0;
                    foreach(string prefabPath in charactersPrefabPath[i])
                    {
                        GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
                        if(prefab is null)
                            continue;

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(prefab, typeof(GameObject), true, GUILayout.MaxWidth(charactersHierarcyRect.width * 2 / 3));
                        
                        // Character Picture
                        //Rect rightBox = new Rect(characterInspectorRect.width / 2, 25, characterInspectorRect.width / 2, leftBox.height);
                        //GUILayout.BeginArea(rightBox, EditorStyles.helpBox);
                            // Draw Sprite
                                                                                            // sırasıyla, başlangıçtan itibaren(10) her sprite için 60 boşluk, tuşların boşluğu, kaç tane tür açık boşluk(calculateSpac..)
                        Rect hierarcyIconRect = new Rect(charactersHierarcyRect.width * 3 / 4, 10 + (count * 60) + (20 + 25 * i) + calculateSpaceForNextHierarcy, 50, 50);
                            Tilemap[] allCharacterPartTilemaps = prefab.GetComponentsInChildren<Tilemap>();
                            foreach(Tilemap tilemap in allCharacterPartTilemaps)
                            {
                                try
                                {
                                    Texture texture = tilemap.GetSprite(Vector3Int.zero).texture;
                                    GUI.DrawTexture(hierarcyIconRect, texture);
                                }catch{}
                            }
                            // Draw Sprite
                        //GUILayout.EndArea();
                        
                        if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
                        {
                            if(hierarcyIconRect.Contains(Event.current.mousePosition))
                            {
                                selectedCharacter = prefab;  // Selected Character
                                SetVariables();
                        
                                this.Repaint(); // Update window (runs onGUI method)
                            }
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Space(40);
                        count++;
                    }
                    calculateSpaceForNextHierarcy += count * 60;
                }
            }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        // Character Inspector
        Rect characterInspectorRect = new Rect(charactersHierarcyRect.width, 0, Screen.width - charactersHierarcyRect.width, Screen.height);
        GUILayout.BeginArea(characterInspectorRect, EditorStyles.helpBox);
        characterInspectorScrollPos = GUILayout.BeginScrollView(characterInspectorScrollPos);

            //Start Label
                GUILayout.BeginHorizontal();

                textDimensions = GUI.skin.label.CalcSize(new GUIContent(selectedCharacter.name));
                //selectedCharacter.name = 
                //objectWidth = textDimensions.x + 5;
                GUILayout.Space((characterInspectorRect.width / 2) - (textDimensions.x + 5) / 2);

                selectedCharacter.name = EditorGUILayout.TextField(selectedCharacter.name, GUILayout.MaxWidth(textDimensions.x + 5), GUILayout.MaxHeight(textDimensions.y + 2));
                
                GUILayout.EndHorizontal();
            // End Label

            // Top Panel
            GUILayout.BeginHorizontal();
            
                // Left Box
                Rect leftBox = new Rect(0, 25, characterInspectorRect.width / 2, 85);
                GUILayout.BeginArea(leftBox, EditorStyles.helpBox);
                    GUILayout.BeginHorizontal();
                    try
                    {
                        EditorGUILayout.LabelField("Health Point: ", GUILayout.MaxWidth(100f));
                        EditorGUILayout.LabelField(character.health.calculatedValue.ToString(), GUILayout.MaxWidth(50f));
                    }catch{}
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    try
                    {
                        EditorGUILayout.LabelField("Min Damage: ", GUILayout.MaxWidth(100f));
                        EditorGUILayout.LabelField(character.minDamage.calculatedValue.ToString(), GUILayout.MaxWidth(50f));
                    }catch{}
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    try
                    {
                        EditorGUILayout.LabelField("Max Damage: ", GUILayout.MaxWidth(100f));
                        EditorGUILayout.LabelField(character.maxDamage.calculatedValue.ToString(), GUILayout.MaxWidth(50f));
                    }catch{}
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    try
                    {
                        EditorGUILayout.LabelField("Initiative : ", GUILayout.MaxWidth(100f));
                        character.initiative.baseValue = EditorGUILayout.FloatField(character.initiative.baseValue, GUILayout.MaxWidth(50f));
                    }catch{}
                    GUILayout.EndHorizontal();
                GUILayout.EndArea();

                // Right Box
                Rect rightBox = new Rect(characterInspectorRect.width / 2, 25, characterInspectorRect.width / 2, leftBox.height);
                GUILayout.BeginArea(rightBox, EditorStyles.helpBox);
                    // Draw Sprite
                    Tilemap[] allTilemaps = selectedCharacter.GetComponentsInChildren<Tilemap>();
                    foreach(Tilemap tilemap in allTilemaps)
                    {
                        try
                        {
                            Texture texture = tilemap.GetSprite(Vector3Int.zero).texture;
                            GUI.DrawTexture(new Rect(rightBox.width/2 - 75/2, (rightBox.height - 75)/2, 75, 75), texture);
                        }catch{}
                    }
                    // Draw Sprite
                GUILayout.EndArea();

            GUILayout.EndHorizontal();
            // Top Panel End
            GUILayout.Space(leftBox.height + 5);
            
            Rect rect = new Rect(0, leftBox.height + 27, 350f, 105f);
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false));
            //GUILayout.BeginArea(rect, EditorStyles.helpBox);

                float textWidth = 100f;
                float floatWidth = 50f;
                GUILayoutOption textWidthOption = GUILayout.MaxWidth(textWidth);
                GUILayoutOption floatWidthOption = GUILayout.MaxWidth(floatWidth);
                
                /*GUILayout.BeginHorizontal();
                GUILayout.Space(100);
                EditorGUILayout.LabelField("Base Value", textWidthOption);
                GUILayout.Space(50);
                EditorGUILayout.LabelField("Calculated", textWidthOption);
                GUILayout.EndHorizontal();*/

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Health Point: ", textWidthOption);
                character.health.baseValue = EditorGUILayout.FloatField(character.health.baseValue, floatWidthOption);
                SetStatDependancy(character.health, (StatDependancy)EditorGUILayout.EnumPopup(character.health.statDependancy));
                EditorGUILayout.LabelField("x", GUILayout.MaxWidth(GUI.skin.label.CalcSize(new GUIContent("x")).x));
                sp = serializedCharacterObject.FindProperty("health");
                ShowPropertyRelative("multiplier", sp, serializedCharacterObject, floatWidthOption);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Min Damage: ", textWidthOption);
                character.minDamage.baseValue = EditorGUILayout.FloatField(character.minDamage.baseValue, floatWidthOption);
                SetStatDependancy(character.minDamage, (StatDependancy)EditorGUILayout.EnumPopup(character.minDamage.statDependancy));
                EditorGUILayout.LabelField("x", GUILayout.MaxWidth(GUI.skin.label.CalcSize(new GUIContent("x")).x));
                sp = serializedCharacterObject.FindProperty("minDamage");
                ShowPropertyRelative("multiplier", sp, serializedCharacterObject, floatWidthOption);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Max Damage: ", textWidthOption);
                character.maxDamage.baseValue = EditorGUILayout.FloatField(character.maxDamage.baseValue, floatWidthOption);
                SetStatDependancy(character.maxDamage, (StatDependancy)EditorGUILayout.EnumPopup(character.maxDamage.statDependancy));
                EditorGUILayout.LabelField("x", GUILayout.MaxWidth(GUI.skin.label.CalcSize(new GUIContent("x")).x));
                sp = serializedCharacterObject.FindProperty("maxDamage");
                ShowPropertyRelative("multiplier", sp, serializedCharacterObject, floatWidthOption);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Movement P: ", textWidthOption);
                character.movementPoint.baseValue = EditorGUILayout.FloatField(character.movementPoint.baseValue, floatWidthOption);
                SetStatDependancy(character.movementPoint, (StatDependancy)EditorGUILayout.EnumPopup(character.movementPoint.statDependancy));
                EditorGUILayout.LabelField("x", GUILayout.MaxWidth(GUI.skin.label.CalcSize(new GUIContent("x")).x));
                sp = serializedCharacterObject.FindProperty("movementPoint");
                ShowPropertyRelative("multiplier", sp, serializedCharacterObject, floatWidthOption);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Offensive P: ", textWidthOption);
                character.offensivePoints.baseValue = EditorGUILayout.FloatField(character.offensivePoints.baseValue, floatWidthOption);
                SetStatDependancy(character.offensivePoints, (StatDependancy)EditorGUILayout.EnumPopup(character.offensivePoints.statDependancy));
                EditorGUILayout.LabelField("x", GUILayout.MaxWidth(GUI.skin.label.CalcSize(new GUIContent("x")).x));
                sp = serializedCharacterObject.FindProperty("offensivePoints");
                ShowPropertyRelative("multiplier", sp, serializedCharacterObject, floatWidthOption);
                GUILayout.EndHorizontal();

            //GUILayout.EndArea();  
            GUILayout.EndVertical();
            
            // Stats Panel
            GUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.ExpandHeight(false));

                int iTextFieldWidth = 68;
                float spaceBetweenText = (characterInspectorRect.width - 25) - iTextFieldWidth * 4;

                GUILayout.Space(spaceBetweenText / 5);
                GUILayout.BeginVertical(GUILayout.Width(iTextFieldWidth));
                EditorGUILayout.LabelField("Strength", GUILayout.Width(iTextFieldWidth));
                character.strength.baseValue = EditorGUILayout.FloatField(character.strength.baseValue, floatWidthOption);
                GUILayout.EndVertical();

                GUILayout.Space(spaceBetweenText / 5);
                GUILayout.BeginVertical(GUILayout.Width(iTextFieldWidth));
                EditorGUILayout.LabelField("Agility", GUILayout.Width(iTextFieldWidth));
                character.agility.baseValue = EditorGUILayout.FloatField(character.agility.baseValue, floatWidthOption);
                GUILayout.EndVertical();

                GUILayout.Space(spaceBetweenText / 5);
                GUILayout.BeginVertical(GUILayout.Width(iTextFieldWidth));
                EditorGUILayout.LabelField("Intelligence", GUILayout.Width(iTextFieldWidth));
                character.intelligence.baseValue = EditorGUILayout.FloatField(character.intelligence.baseValue, floatWidthOption);
                GUILayout.EndVertical();

                GUILayout.Space(spaceBetweenText / 5);
                GUILayout.BeginVertical(GUILayout.Width(iTextFieldWidth));
                EditorGUILayout.LabelField("Vitality", GUILayout.Width(iTextFieldWidth));
                character.vitality.baseValue = EditorGUILayout.FloatField(character.vitality.baseValue, floatWidthOption);
                GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            // Toggle button Start
            GUILayout.BeginHorizontal();
            
            //GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            
            //int iButtonWidth = 70;
            
            //GUILayout.Space(Screen.width/2 - iButtonWidth /2);  // this middles one button with iButtonWidth size

            float spaceBetween = characterInspectorRect.width - iButtonWidth * 3;
            GUILayout.Space(spaceBetween / 4);
            enableAddSkill = GUILayout.Toggle(enableAddSkill, "Add", buttonStyle, GUILayout.Width(iButtonWidth));
            GUILayout.Space(spaceBetween / 4);
            enableRemoveSkill = GUILayout.Toggle(enableRemoveSkill, "Remove", buttonStyle, GUILayout.Width(iButtonWidth));
            GUILayout.Space(spaceBetween / 4);
            enableMoveSkill = GUILayout.Toggle(enableMoveSkill, "Move", buttonStyle, GUILayout.Width(iButtonWidth));

            if(enableAddSkill)
            {
                enableRemoveSkill = false;
                enableMoveSkill = false;
            }
            if(enableRemoveSkill)
            {
                enableAddSkill = false;
                enableMoveSkill = false;
            }
            if(enableMoveSkill)
            {
                enableRemoveSkill = false;
                enableAddSkill = false;
            }

            GUILayout.EndHorizontal();
            // Toggle Button End
            EditorGUILayout.Space(23);
            
            if(character is Player)
            {
                rect = new Rect(3, rect.y + rect.height + 50 + 28, characterInspectorRect.width - 6, 150);
                GUILayout.BeginArea(rect, EditorStyles.helpBox);
                //GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false));
                    SerializedProperty skillsArray = serializedCharacterObject.FindProperty("characterSkills");
                    
                    Vector2 skillSize = new Vector2(50, 50);
                    int skillNumber = (int)((rect.width - 10) / skillSize.x);
                    if((skillNumber - 1) * (skillSize.x + 2) > (rect.width - 10))
                        skillNumber = (skillNumber - 1 == 0) ? 1 : --skillNumber;

                    if(skillNumber == 0) skillNumber = 1;

                    float spaceBetweenBoxes = (rect.width - 10 - skillSize.x) / (skillNumber - 1);
                    for(int i = 0; i < skillsArray.arraySize; i++)
                    {
                        SerializedProperty skillProp = skillsArray.GetArrayElementAtIndex(i);
                        Item obj = GetTargetObjectOfProperty(skillProp) as Item;
                        
                        if(obj == null)
                            continue;
                        
                        if(i + 1 > skillNumber)
                            rect = new Rect(5 + ((i - skillNumber) * (spaceBetweenBoxes)), 5 + (skillSize.y + 5), skillSize.x, skillSize.y);
                        else
                            rect = new Rect(5 + (i * (spaceBetweenBoxes)), 5, skillSize.x, skillSize.y);
                        
                        Rect objRect = new Rect(rect.x + 5, rect.y + 5, 40, 40);
                        GUI.DrawTexture(objRect, obj.Icon.texture);
                        GUILayout.BeginArea(rect, EditorStyles.helpBox);
                            // Could not get it to draw inside
                            //GUI.DrawTexture(objRect, obj.Icon.texture);
                        GUILayout.EndArea();

                        if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            if(rect.Contains(Event.current.mousePosition))
                            {
                                if(enableAddSkill)
                                {
                                    if(Selection.activeObject is UsableItem)
                                    {
                                        // Get skill we selected
                                        //character.addSkill = Selection.activeObject as Item;
                                        this.addSkill = Selection.activeObject as Item;
                                        // insert element at index
                                        skillsArray.InsertArrayElementAtIndex(i);
                                        // get reference to inserted element
                                        SerializedProperty propertyAtIndex = skillsArray.GetArrayElementAtIndex(i);
                                        // change object of inserted element to the skill we selected
                                        propertyAtIndex.objectReferenceValue = this.addSkill;
                                    }
                                }
                                else if(enableRemoveSkill)
                                {
                                    skillsArray.DeleteArrayElementAtIndex(i);
                                    skillsArray.DeleteArrayElementAtIndex(i);
                                }
                                else if(enableMoveSkill)
                                {

                                }
                                else    // nothing is selected
                                {
                                    Selection.activeObject = obj;
                                }
                                serializedCharacterObject.ApplyModifiedProperties();
                                this.Repaint();
                            }
                        }
                    }
                    
                //GUILayout.EndVertical();
                GUILayout.EndArea();
            }
            if(character is Enemy)
            {
                rect = new Rect(3, rect.y + rect.height + 50 + 28, characterInspectorRect.width - 6, skillAreaHeight);
                GUILayout.Space(skillAreaHeight);
                GUILayout.BeginArea(rect, EditorStyles.helpBox);
                    
                    float spaceBetweenChoiceBoxes = (rect.width - 9) / 3;
                    float biggestAreaSize = 0;
                    for(int x = 0; x < 3; x++)
                    {
                        
                        Rect choiceRect = new Rect(3 * (x + 1) + (x * spaceBetweenChoiceBoxes), 3, spaceBetweenChoiceBoxes - 3, skillAreaHeight - 5);
                        rect = choiceRect;
                        GUILayout.BeginArea(choiceRect, EditorStyles.helpBox);

                            Vector2 skillSize = new Vector2(50, 50);
                            
                            sb.Length = 0;
                            SerializedProperty skillsArray;
                            switch (x)
                            {
                                case 0:
                                    skillsArray = serializedCharacterObject.FindProperty("firstChoiceSkills");
                                    sb.Append("First Choice");
                                    break;
                                case 1:
                                    skillsArray = serializedCharacterObject.FindProperty("secondChoiceSkills");
                                    sb.Append("Second Choice");
                                    //biggestAreaSize = 8 + skillsArray.arraySize * (skillSize.y + 5);
                                    break;
                                case 2:
                                    skillsArray = serializedCharacterObject.FindProperty("thirdChoiceSkills");
                                    sb.Append("Third Choice");
                                    //biggestAreaSize = 8 + skillsArray.arraySize * (skillSize.y + 5);
                                    break;
                                default:
                                    skillsArray = serializedCharacterObject.FindProperty("firstChoiceSkills");
                                    break;
                            }
                            float tempSize = 8 + (skillsArray.arraySize * (skillSize.y + 5));
                            if(biggestAreaSize < tempSize) biggestAreaSize = tempSize;

                            GUILayout.BeginHorizontal();
                            textDimensions = GUI.skin.label.CalcSize(new GUIContent(sb.ToString()));
                            GUILayout.Space((choiceRect.width / 2) - ((textDimensions.x + 5) / 2));
                            EditorGUILayout.LabelField(sb.ToString());
                            GUILayout.EndHorizontal();
                            biggestAreaSize += textDimensions.y;
                        
                            GUILayout.BeginHorizontal();
                            
                            

                            float spaceBetweenBoxes = (rect.width / 2) - (skillSize.x / 2);

                            if(skillsArray.arraySize == 0)
                            {
                                skillsArray.InsertArrayElementAtIndex(0);
                                SerializedProperty propertyAtIndex = skillsArray.GetArrayElementAtIndex(0);
                                propertyAtIndex.objectReferenceValue = tempSkill;
                                serializedCharacterObject.ApplyModifiedProperties();
                                //this.Repaint();
                            }
                            for(int i = 0; i < skillsArray.arraySize; i++)
                            {
                                SerializedProperty skillProp = skillsArray.GetArrayElementAtIndex(i);
                                Item obj = GetTargetObjectOfProperty(skillProp) as Item;
                                
                                if(obj == null)
                                    continue;
                                
                                rect = new Rect(spaceBetweenBoxes, (5 + i * (skillSize.y + 5)) + textDimensions.y, skillSize.x, skillSize.y);
                                
                                Rect objRect = new Rect(rect.x + 5, rect.y + 5, 40, 40);
                                GUI.DrawTexture(objRect, obj.Icon.texture);
                                GUILayout.BeginArea(rect, EditorStyles.helpBox);
                                    // Could not get it to draw inside
                                    //GUI.DrawTexture(objRect, obj.Icon.texture);
                                GUILayout.EndArea();

                                if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
                                {
                                    if(rect.Contains(Event.current.mousePosition))
                                    {
                                        if(enableAddSkill)
                                        {
                                            if(Selection.activeObject is UsableItem)
                                            {
                                                // Get skill we selected
                                                //character.addSkill = Selection.activeObject as Item;
                                                this.addSkill = Selection.activeObject as Item;
                                                // insert element at index
                                                skillsArray.InsertArrayElementAtIndex(i);
                                                // get reference to inserted element
                                                SerializedProperty propertyAtIndex = skillsArray.GetArrayElementAtIndex(i);
                                                // change object of inserted element to the skill we selected
                                                propertyAtIndex.objectReferenceValue = this.addSkill;
                                            }
                                        }
                                        else if(enableRemoveSkill)
                                        {
                                            skillsArray.DeleteArrayElementAtIndex(i);
                                            skillsArray.DeleteArrayElementAtIndex(i);
                                        }
                                        else if(enableMoveSkill)
                                        {
                                            //this.addSkill = GetTargetObjectOfProperty(skillsArray.GetArrayElementAtIndex(i)) as Item;
                                            movedSkillProperty = skillsArray.GetArrayElementAtIndex(i);
                                        }
                                        else    // nothing is selected
                                        {
                                            Selection.activeObject = obj;
                                        }
                                    }
                                }
                                if(Event.current.type == EventType.MouseUp && Event.current.button == 0)
                                {
                                    if(rect.Contains(Event.current.mousePosition))
                                    {
                                        if(enableMoveSkill)
                                        {
                                            /*skillsArray.InsertArrayElementAtIndex(i);
                                            serializedCharacterObject.ApplyModifiedProperties();

                                            SerializedProperty propertyAtIndex = skillsArray.GetArrayElementAtIndex(i);
                                            SerializedProperty tempProp = skillsArray.GetArrayElementAtIndex(i + 1);
                                            //Object tempObject = tempProp.objectReferenceValue;
                                            Object objectAtIndex = GetTargetObjectOfProperty(tempProp) as Object;
                                            
                                            Debug.Log(objectAtIndex.name);
                                            //Debug.Log(tempObject.name);
                                            propertyAtIndex.objectReferenceValue = movedSkillProperty.objectReferenceValue;
                                            serializedCharacterObject.ApplyModifiedProperties();
                                            movedSkillProperty.objectReferenceValue = objectAtIndex;
                                            movedSkillProperty.serializedObject.ApplyModifiedProperties();*/
                                        }
                                    }
                                }
                                serializedCharacterObject.ApplyModifiedProperties();
                                this.Repaint();
                            }
                            GUILayout.EndHorizontal();
                        
                        GUILayout.EndArea();
                    }
                    if(skillAreaHeight != biggestAreaSize) skillAreaHeight = biggestAreaSize;
                    
                GUILayout.EndArea();
            }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
    public Item tempSkill;
    Item addSkill;
    SerializedProperty movedSkillProperty;
    bool enableAddSkill = false;
    bool enableRemoveSkill = false;
    bool enableMoveSkill = false;
    float skillAreaHeight = 150;

    void ShowPropertyRelative(string relativePropertyPath, SerializedProperty serializedProperty,  SerializedObject serializedObject, GUILayoutOption width)
    {
        SerializedProperty relativeProperty = serializedProperty.FindPropertyRelative(relativePropertyPath);
        relativeProperty.floatValue = EditorGUILayout.FloatField(relativeProperty.floatValue, width);
        serializedObject.ApplyModifiedProperties();
    }

    void SetStatDependancy(StatWithDepency stat, StatDependancy statDependancy)
    {
        stat.statDependancy = statDependancy;
        stat.DependantStatChanged();
        switch (statDependancy)
        {
            case StatDependancy.Strength:
                stat.statToDependOn = character.strength;
                break;
            case StatDependancy.Agility:
                stat.statToDependOn = character.agility;
                break;
            case StatDependancy.Intelligence:
                stat.statToDependOn = character.intelligence;
                break;
            case StatDependancy.Vitality:
                stat.statToDependOn = character.vitality;
                break;
        }
    }

    string header_selectedCharacter = "Right Click To Select A Character";
    GameObject selectedCharacter;
    Texture texture_selectedCharacter;
    float character_health;
    Character character;
    FlexibleStat stat;
    private void OnSceneGUI(SceneView sceneView)
    {
        // Right click to select a character
        if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
            Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
            mousePosition = ray.origin;

            LayerMask mask = LayerMask.NameToLayer("Characters"); if((int)mask == -1) Debug.LogError("Incorrect Mask !");
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, 1 << mask);
            if(hit.collider != null)
            {
                selectedCharacter = hit.collider.gameObject;  // Selected Character
                SetVariables();
                
                this.Repaint(); // Update window (runs onGUI method)
            }
        }
    }

    void SetVariables()
    {
        // When a character selected, these attribues will be - atanacak
        sb.Length = 0;
        header_selectedCharacter = sb.Append(selectedCharacter.name).ToString();
        
        Tilemap tilemapSprite = selectedCharacter.transform.GetChild(0).transform.GetChild(0).GetComponent<Tilemap>();
        texture_selectedCharacter = tilemapSprite.GetSprite(Vector3Int.zero).texture;

        character = selectedCharacter.GetComponent<Character>();
        character_health = character.health.baseValue;
    }

    void OnEnable()
    {
        if(selectedCharacter == null)
        {
            selectedCharacter = GameObject.FindObjectOfType<Player>().gameObject;
            SetVariables();
        }
        SceneView.duringSceneGui += this.OnSceneGUI;
    }
    void OnDisable()
    {
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    public static object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        if (prop == null) return null;

        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }
        return obj;
    }

    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }

    private static object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;

        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext()) return null;
        }
        return enm.Current;
    }
}
#endif