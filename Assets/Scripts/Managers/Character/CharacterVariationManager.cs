using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;

public class CharacterVariationManager : MonoBehaviour
{
    [SerializeField] List<VarietyClass> variations;
    
    [SerializeField]

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class VarietyClass
{
    public string name;
    public List<Tile> varietyTiles;
    //public 
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(VarietyClass))]
public class VarietyClassCustomDrawer: PropertyDrawer 
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var nameRect = new Rect(position.x, position.y, 30, position.height);
        var varietyTilesRect = new Rect(position.x + 35, position.y, 50, position.height);
        //var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(varietyTilesRect, property.FindPropertyRelative("varietyTiles"), GUIContent.none);
        //EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        //GUILayout.TextField("test");
        
        //property.FindPropertyRelative("name").stringValue = EditorGUILayout.TextField(property.FindPropertyRelative("name").stringValue);
        //var varietyTiles = EditorGUILayout.PropertyField(property.FindPropertyRelative("varietyTiles"));

        EditorGUI.EndProperty();
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        var name = new PropertyField(property.FindPropertyRelative("name"));
        var varietyTiles = new PropertyField(property.FindPropertyRelative("varietyTiles"));

        container.Add(name);
        container.Add(varietyTiles);

        return container;
    }
}

/*[CustomEditor(typeof(CharacterVariationManager))]
public class CharacterVariationManagerCustomEditor : Editor 
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
    }
}*/
#endif