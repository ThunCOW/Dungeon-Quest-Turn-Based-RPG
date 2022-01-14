#if (UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

[CustomEditor(typeof(EffectTiles))]
[CanEditMultipleObjects]
public class EffectTilesEditor : Editor
{
    protected static readonly StringBuilder sb = new StringBuilder();

    private bool editFrontPositions;
    private bool editFrontOnlyPositions;
    private bool editBehindPosition;
    private bool editBlockedPosition;
    
    private bool instantSortingOrderTransition;
    
    private GUIStyle boldStyle = new GUIStyle();

    public override void OnInspectorGUI()
    {
        
//Start Label
EditorGUILayout.BeginHorizontal();

        sb.Length = 0;
        sb.Append("Selection Mode");
        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(sb.ToString()));
        float labelWidth = textDimensions.x;
        GUILayout.Space(Screen.width/2 - labelWidth);

        EditorGUILayout.LabelField(sb.ToString(), boldStyle);

EditorGUILayout.EndHorizontal();
//End Label

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        int iButtonWidth = 120;

// Toggle button Start
GUILayout.BeginHorizontal();


        //GUILayout.Space(Screen.width/2 - iButtonWidth /2);  // this middles one button with iButtonWidth size
        //GUILayout.Space(Screen.width/7 - iButtonWidth/2);

        bool previousEditFrontPosition = editFrontPositions;
        editFrontPositions = GUILayout.Toggle(editFrontPositions, "Edit Front Position", buttonStyle, GUILayout.Width(iButtonWidth));
        if(previousEditFrontPosition == false && editFrontPositions == true)
        {
            editFrontOnlyPositions = false;
            editBehindPosition = false;
            editBlockedPosition = false;
            instantSortingOrderTransition = false;
        }

        bool previousEditBehindPosition = editBehindPosition;
        editBehindPosition = GUILayout.Toggle(editBehindPosition, "Edit Behind position", buttonStyle, GUILayout.Width(iButtonWidth));
        if(previousEditBehindPosition == false && editBehindPosition == true)
        {
            editFrontPositions = false;
            editFrontOnlyPositions = false;
            editBlockedPosition = false;
            instantSortingOrderTransition = false;
        }

        bool previouseditBlockedPosition = editBlockedPosition;
        editBlockedPosition = GUILayout.Toggle(editBlockedPosition, "Block Tiles", buttonStyle, GUILayout.Width(iButtonWidth));
        if(previouseditBlockedPosition == false && editBlockedPosition == true)
        {
            editFrontPositions = false;
            editFrontOnlyPositions = false;
            editBehindPosition = false;
            instantSortingOrderTransition = false;
        }

GUILayout.EndHorizontal();
// Toggle Button End



GUILayout.BeginHorizontal();

        GUILayout.Space(Screen.width/3 - 170/2);
        
        bool previousEditFrontOnlyPositions = editFrontOnlyPositions;
        editFrontOnlyPositions = GUILayout.Toggle(editFrontOnlyPositions, "Front Only Tiles", buttonStyle, GUILayout.Width(iButtonWidth));
        if(previousEditFrontOnlyPositions == false && editFrontOnlyPositions == true)
        {
            editFrontPositions = false;
            editBehindPosition = false;
            editBlockedPosition = false;
            instantSortingOrderTransition = false;
        }
        
        instantSortingOrderTransition = GUILayout.Toggle(instantSortingOrderTransition, "Sorting Order Instant Tiles", buttonStyle, GUILayout.Width(170));
        if(instantSortingOrderTransition == true)
        {
            editFrontPositions = false;
            editFrontOnlyPositions = false;
            editBehindPosition = false;
            editBlockedPosition = false;
        }
GUILayout.EndHorizontal();
    }

    private void OnSceneGUI()
    {
        // If Toggled, change mouse mode
        if(editFrontPositions || editFrontOnlyPositions || editBehindPosition || editBlockedPosition || instantSortingOrderTransition)
        {
            Tools.current = Tool.None;

            // Disables tool to be able to select game object in Scene
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        PathPainter();
    }

    Vector3 lastMousePos;   
    bool isRemoving;        // when mouse button is hold, if clicked on existing position we are removing tiles, if not we are adding
    void PathPainter()
    {
        /*
                Front Positions
        */
        if(editFrontPositions) // if right tool is selected
        {
            // Singular mouse click
            if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector3 mousePosition = Event.current.mousePosition;
                mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                mousePosition = ray.origin;

                if(GridManager.gridBase != null)
                {
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;
                    if(!Selection.activeGameObject.GetComponent<EffectTiles>().characterInFront.Contains(mousePosInt))
                    {
                        if(!Selection.activeGameObject.GetComponent<EffectTiles>().characterBehind.Contains(mousePosInt) && 
                            !Selection.activeGameObject.GetComponent<EffectTiles>().blockedPositions.Contains(mousePosInt))
                        {
                            SerializedProperty frontTiles = selectedObject.FindProperty("characterInFront");
                            if(frontTiles.arraySize > 0)
                                frontTiles.InsertArrayElementAtIndex(frontTiles.arraySize - 1);
                            else
                                frontTiles.InsertArrayElementAtIndex(0);
                            SerializedProperty sp = frontTiles.GetArrayElementAtIndex(frontTiles.arraySize - 1);
                            sp.vector3IntValue = mousePosInt;

                            selectedObject.ApplyModifiedProperties();
                            this.Repaint(); // Update window (runs onGUI method)

                            // Clicked on a position that does not exist in our list
                            isRemoving = false;
                        }
                    }
                    else
                    {
                        SerializedProperty frontTiles = selectedObject.FindProperty("characterInFront");
                        int index = Selection.activeGameObject.GetComponent<EffectTiles>().characterInFront.IndexOf(mousePosInt);
                        frontTiles.DeleteArrayElementAtIndex(index);
                        
                        selectedObject.ApplyModifiedProperties();
                        this.Repaint(); // Update window (runs onGUI method)

                        // We clicked on a pos that already exists
                        isRemoving = true;
                    }
                }
            }

            // While we are holding/dragging mouse button down
            if(isRemoving)  // Removing tiles
            {
                if(Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    Vector3 mousePosition = Event.current.mousePosition;
                    mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                    Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                    mousePosition = ray.origin;
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;

                    if(lastMousePos != mousePosInt)
                    {
                        lastMousePos = mousePosInt;

                        if(GridManager.gridBase != null)
                        {
                            if(Selection.activeGameObject.GetComponent<EffectTiles>().characterInFront.Contains(mousePosInt))
                            {
                                SerializedProperty frontTiles = selectedObject.FindProperty("characterInFront");
                                int index = Selection.activeGameObject.GetComponent<EffectTiles>().characterInFront.IndexOf(mousePosInt);
                                frontTiles.DeleteArrayElementAtIndex(index);
                                
                                selectedObject.ApplyModifiedProperties();
                                this.Repaint(); // Update window (runs onGUI method)
                            }
                        }
                        else
                            Debug.LogError("Gridbase is not set!");
                    }
                }
            }
            else        // Adding tiles
            {
                if(Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    Vector3 mousePosition = Event.current.mousePosition;
                    mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                    Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                    mousePosition = ray.origin;
                    Grid tempGrid = new Grid();
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;

                    if(lastMousePos != mousePosInt)
                    {
                        lastMousePos = mousePosInt;

                        if(GridManager.gridBase != null)
                        {
                            if(!Selection.activeGameObject.GetComponent<EffectTiles>().characterInFront.Contains(mousePosInt))
                            {
                                if(!Selection.activeGameObject.GetComponent<EffectTiles>().characterBehind.Contains(mousePosInt) && !Selection.activeGameObject.GetComponent<EffectTiles>().blockedPositions.Contains(mousePosInt))
                                {
                                    SerializedProperty frontTiles = selectedObject.FindProperty("characterInFront");
                                    frontTiles.InsertArrayElementAtIndex(frontTiles.arraySize - 1);
                                    SerializedProperty sp = frontTiles.GetArrayElementAtIndex(frontTiles.arraySize - 1);
                                    sp.vector3IntValue = mousePosInt;
                                    
                                    selectedObject.ApplyModifiedProperties();
                                    this.Repaint(); // Update window (runs onGUI method)
                                }
                            }
                        }
                        else
                            Debug.LogError("Gridbase is not set!");
                    }
                }
            }
        }

        /*
                Behind Positions
        */
        if(editBehindPosition) // if right tool is selected
        {
            // Singular mouse click
            if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector3 mousePosition = Event.current.mousePosition;
                mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                mousePosition = ray.origin;

                if(GridManager.gridBase != null)
                {
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;
                    if(!Selection.activeGameObject.GetComponent<EffectTiles>().characterBehind.Contains(mousePosInt))
                    {
                        if(!Selection.activeGameObject.GetComponent<EffectTiles>().characterInFront.Contains(mousePosInt) && !Selection.activeGameObject.GetComponent<EffectTiles>().blockedPositions.Contains(mousePosInt))
                        {
                            SerializedProperty frontTiles = selectedObject.FindProperty("characterBehind");
                            if(frontTiles.arraySize > 0)
                                frontTiles.InsertArrayElementAtIndex(frontTiles.arraySize - 1);
                            else
                                frontTiles.InsertArrayElementAtIndex(0);
                            SerializedProperty sp = frontTiles.GetArrayElementAtIndex(frontTiles.arraySize - 1);
                            sp.vector3IntValue = mousePosInt;

                            selectedObject.ApplyModifiedProperties();
                            this.Repaint(); // Update window (runs onGUI method)

                            // Clicked on a position that does not exist in our list
                            isRemoving = false;
                        }
                    }
                    else
                    {
                        SerializedProperty frontTiles = selectedObject.FindProperty("characterBehind");
                        int index = Selection.activeGameObject.GetComponent<EffectTiles>().characterBehind.IndexOf(mousePosInt);
                        frontTiles.DeleteArrayElementAtIndex(index);
                        
                        selectedObject.ApplyModifiedProperties();
                        this.Repaint(); // Update window (runs onGUI method)

                        // We clicked on a pos that already exists
                        isRemoving = true;
                    }
                }
            }

            // While we are holding/dragging mouse button down
            if(isRemoving)  // Removing tiles
            {
                if(Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    Vector3 mousePosition = Event.current.mousePosition;
                    mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                    Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                    mousePosition = ray.origin;
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;

                    if(lastMousePos != mousePosInt)
                    {
                        lastMousePos = mousePosInt;

                        if(GridManager.gridBase != null)
                        {
                            if(Selection.activeGameObject.GetComponent<EffectTiles>().characterBehind.Contains(mousePosInt))
                            {
                                SerializedProperty frontTiles = selectedObject.FindProperty("characterBehind");
                                int index = Selection.activeGameObject.GetComponent<EffectTiles>().characterBehind.IndexOf(mousePosInt);
                                frontTiles.DeleteArrayElementAtIndex(index);
                                
                                selectedObject.ApplyModifiedProperties();
                                this.Repaint(); // Update window (runs onGUI method)
                            }
                        }
                        else
                            Debug.LogError("Gridbase is not set!");
                    }
                }
            }
            else        // Adding tiles
            {
                if(Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    Vector3 mousePosition = Event.current.mousePosition;
                    mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                    Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                    mousePosition = ray.origin;
                    Grid tempGrid = new Grid();
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;

                    if(lastMousePos != mousePosInt)
                    {
                        lastMousePos = mousePosInt;

                        if(GridManager.gridBase != null)
                        {
                            if(!Selection.activeGameObject.GetComponent<EffectTiles>().characterBehind.Contains(mousePosInt))
                            {
                                if(!Selection.activeGameObject.GetComponent<EffectTiles>().characterInFront.Contains(mousePosInt) && !Selection.activeGameObject.GetComponent<EffectTiles>().blockedPositions.Contains(mousePosInt))
                                {
                                    SerializedProperty frontTiles = selectedObject.FindProperty("characterBehind");
                                    frontTiles.InsertArrayElementAtIndex(frontTiles.arraySize - 1);
                                    SerializedProperty sp = frontTiles.GetArrayElementAtIndex(frontTiles.arraySize - 1);
                                    sp.vector3IntValue = mousePosInt;
                                    
                                    selectedObject.ApplyModifiedProperties();
                                    this.Repaint(); // Update window (runs onGUI method)
                                }
                            }
                        }
                        else
                            Debug.LogError("Gridbase is not set!");
                    }
                }
            }
        }

        /*
                Blocked Positions
        */
        if(editBlockedPosition) // if right tool is selected
        {
            // Singular mouse click
            if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector3 mousePosition = Event.current.mousePosition;
                mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                mousePosition = ray.origin;

                if(GridManager.gridBase != null)
                {
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;
                    if(!Selection.activeGameObject.GetComponent<EffectTiles>().blockedPositions.Contains(mousePosInt)) // Adding Singular Click
                    {
                        SerializedProperty frontTiles = selectedObject.FindProperty("blockedPositions");
                        if(frontTiles.arraySize > 0)
                            frontTiles.InsertArrayElementAtIndex(frontTiles.arraySize - 1);
                        else
                            frontTiles.InsertArrayElementAtIndex(0);
                        SerializedProperty sp = frontTiles.GetArrayElementAtIndex(frontTiles.arraySize - 1);
                        sp.vector3IntValue = mousePosInt;

                        selectedObject.ApplyModifiedProperties();
                        this.Repaint(); // Update window (runs onGUI method)

                        // Clicked on a position that does not exist in our list
                        isRemoving = false;
                    }
                    else    // Removing Singular Click
                    {
                        SerializedProperty frontTiles = selectedObject.FindProperty("blockedPositions");
                        int index = Selection.activeGameObject.GetComponent<EffectTiles>().blockedPositions.IndexOf(mousePosInt);
                        frontTiles.DeleteArrayElementAtIndex(index);
                        
                        selectedObject.ApplyModifiedProperties();
                        this.Repaint(); // Update window (runs onGUI method)

                        // We clicked on a pos that already exists
                        isRemoving = true;
                    }
                }
            }

            // While we are holding/dragging mouse button down
            if(isRemoving)  // Removing tiles
            {
                if(Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    Vector3 mousePosition = Event.current.mousePosition;
                    mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                    Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                    mousePosition = ray.origin;
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;

                    if(lastMousePos != mousePosInt)
                    {
                        lastMousePos = mousePosInt;

                        if(GridManager.gridBase != null)
                        {
                            if(Selection.activeGameObject.GetComponent<EffectTiles>().blockedPositions.Contains(mousePosInt))
                            {
                                SerializedProperty frontTiles = selectedObject.FindProperty("blockedPositions");
                                int index = Selection.activeGameObject.GetComponent<EffectTiles>().blockedPositions.IndexOf(mousePosInt);
                                frontTiles.DeleteArrayElementAtIndex(index);
                                
                                selectedObject.ApplyModifiedProperties();
                                this.Repaint(); // Update window (runs onGUI method)
                            }
                        }
                        else
                            Debug.LogError("Gridbase is not set!");
                    }
                }
            }
            else        // Adding tiles
            {
                if(Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    Vector3 mousePosition = Event.current.mousePosition;
                    mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                    Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                    mousePosition = ray.origin;
                    Grid tempGrid = new Grid();
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;

                    if(lastMousePos != mousePosInt)
                    {
                        lastMousePos = mousePosInt;

                        if(GridManager.gridBase != null)
                        {
                            if(!Selection.activeGameObject.GetComponent<EffectTiles>().blockedPositions.Contains(mousePosInt))
                            {
                                SerializedProperty frontTiles = selectedObject.FindProperty("blockedPositions");
                                frontTiles.InsertArrayElementAtIndex(frontTiles.arraySize - 1);
                                SerializedProperty sp = frontTiles.GetArrayElementAtIndex(frontTiles.arraySize - 1);
                                sp.vector3IntValue = mousePosInt;
                                
                                selectedObject.ApplyModifiedProperties();
                                this.Repaint(); // Update window (runs onGUI method)
                            }
                        }
                        else
                            Debug.LogError("Gridbase is not set!");
                    }
                }
            }
        }

        /*
                Instant Sorting Order Change Positions
        */
        if(instantSortingOrderTransition) // if right tool is selected
        {
            // Singular mouse click
            if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector3 mousePosition = Event.current.mousePosition;
                mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                mousePosition = ray.origin;

                if(GridManager.gridBase != null)
                {
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;
                    if(!Selection.activeGameObject.GetComponent<EffectTiles>().instantSortingOrderTransitionPositions.Contains(mousePosInt)) // Adding Singular Click
                    {
                        SerializedProperty frontTiles = selectedObject.FindProperty("instantSortingOrderTransitionPositions");
                        if(frontTiles.arraySize > 0)
                            frontTiles.InsertArrayElementAtIndex(frontTiles.arraySize - 1);
                        else
                            frontTiles.InsertArrayElementAtIndex(0);
                        SerializedProperty sp = frontTiles.GetArrayElementAtIndex(frontTiles.arraySize - 1);
                        sp.vector3IntValue = mousePosInt;

                        selectedObject.ApplyModifiedProperties();
                        this.Repaint(); // Update window (runs onGUI method)

                        // Clicked on a position that does not exist in our list
                        isRemoving = false;
                    }
                    else    // Removing Singular Click
                    {
                        SerializedProperty frontTiles = selectedObject.FindProperty("instantSortingOrderTransitionPositions");
                        int index = Selection.activeGameObject.GetComponent<EffectTiles>().instantSortingOrderTransitionPositions.IndexOf(mousePosInt);
                        frontTiles.DeleteArrayElementAtIndex(index);
                        
                        selectedObject.ApplyModifiedProperties();
                        this.Repaint(); // Update window (runs onGUI method)

                        // We clicked on a pos that already exists
                        isRemoving = true;
                    }
                }
            }
        }
    
        /*
        *       Front Only Positions
        */
        /*
                Instant Sorting Order Change Positions
        */
        if(editFrontOnlyPositions) // if right tool is selected
        {
            // Singular mouse click
            if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector3 mousePosition = Event.current.mousePosition;
                mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                mousePosition = ray.origin;

                if(GridManager.gridBase != null)
                {
                    Vector3Int mousePosInt = GridManager.gridBase.WorldToCell(mousePosition);
                    mousePosInt.z = 0;
                    if(!Selection.activeGameObject.GetComponent<EffectTiles>().characterFrontOnly.Contains(mousePosInt)) // Adding Singular Click
                    {
                        if(!Selection.activeGameObject.GetComponent<EffectTiles>().characterInFront.Contains(mousePosInt) &&
                            !Selection.activeGameObject.GetComponent<EffectTiles>().characterBehind.Contains(mousePosInt))
                        {
                            SerializedProperty frontTiles = selectedObject.FindProperty("characterFrontOnly");
                            if(frontTiles.arraySize > 0)
                                frontTiles.InsertArrayElementAtIndex(frontTiles.arraySize - 1);
                            else
                                frontTiles.InsertArrayElementAtIndex(0);
                            SerializedProperty sp = frontTiles.GetArrayElementAtIndex(frontTiles.arraySize - 1);
                            sp.vector3IntValue = mousePosInt;

                            selectedObject.ApplyModifiedProperties();
                            this.Repaint(); // Update window (runs onGUI method)

                            // Clicked on a position that does not exist in our list
                            isRemoving = false;
                        }
                    }
                    else    // Removing Singular Click
                    {
                        SerializedProperty frontTiles = selectedObject.FindProperty("characterFrontOnly");
                        int index = Selection.activeGameObject.GetComponent<EffectTiles>().characterFrontOnly.IndexOf(mousePosInt);
                        frontTiles.DeleteArrayElementAtIndex(index);
                        
                        selectedObject.ApplyModifiedProperties();
                        this.Repaint(); // Update window (runs onGUI method)

                        // We clicked on a pos that already exists
                        isRemoving = true;
                    }
                }
            }
        }
    }

    SerializedObject selectedObject;
    void OnEnable()
    {
        boldStyle.fontStyle = FontStyle.Bold;

        selectedObject = serializedObject;
        selectedObject.FindProperty("isSelectedOnEditor").boolValue = true;
        selectedObject.ApplyModifiedProperties();
    }

    void OnDisable()
    {
        selectedObject.FindProperty("isSelectedOnEditor").boolValue = false;
        selectedObject.ApplyModifiedProperties();

        Tools.current = Tool.Move;
    }
}
#endif