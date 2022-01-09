#if UNITY_EDITOR

using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using UnityEditor.EditorTools;

public class AIWanderingTiles : EditorWindow
{
    [MenuItem ("Window/Grid Actions")]
    public static void  ShowWindow () 
    {
        EditorWindow.GetWindow(typeof(AIWanderingTiles));
    }

    AIMovement aiMovement;
    SerializedProperty aiPatrolTiles;
    
    protected static readonly StringBuilder sb = new StringBuilder();
    private bool enablePathMode;
    private bool enableGridMode;
    private bool enableEditorTileEffectMode;
    private void OnGUI()
    {
        //Start Label
        GUILayout.BeginHorizontal();

        sb.Length = 0;
        sb.Append("Selection Mode");
        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(sb.ToString()));
        float labelWidth = textDimensions.x;
        GUILayout.Space(Screen.width/2 - labelWidth /2);

        EditorGUILayout.LabelField(sb.ToString());
        
        GUILayout.EndHorizontal();
        // End Label

        // Toggle button Start
        GUILayout.BeginHorizontal();
        
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        
        int iButtonWidth = 100;
        
        //GUILayout.Space(Screen.width/2 - iButtonWidth /2);  // this middles one button with iButtonWidth size
        GUILayout.Space(Screen.width/2 - iButtonWidth);
        enablePathMode = GUILayout.Toggle(enablePathMode, "Path", buttonStyle, GUILayout.Width(iButtonWidth));
        
        enableGridMode = GUILayout.Toggle(enableGridMode, "Grid", buttonStyle, GUILayout.Width(iButtonWidth));

        GUILayout.EndHorizontal();
        // Toggle Button End

        if(aiMovement != null)
        {
            EditorGUILayout.ObjectField(aiMovement.gameObject, typeof(GameObject), true);
            //EditorGUILayout.IntField(aiMovement.wanderingTiles.cou)
            /*SerializedObject so = new SerializedObject(aiMovement);
            SerializedProperty stringsProperty = so.FindProperty("wanderingTiles");
    
            EditorGUILayout.PropertyField(stringsProperty, new GUIContent("Wandering Positions"), true);
            so.ApplyModifiedProperties();*/

            /*List<Vector3> wanderingTiles = aiMovement.wanderingTiles;
            int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("size", wanderingTiles.Count));
            while (newCount < wanderingTiles.Count)
                wanderingTiles.RemoveAt( wanderingTiles.Count - 1 );
            while (newCount > wanderingTiles.Count)
                wanderingTiles.Add(Vector3.zero);

            for(int i = 0; i < wanderingTiles.Count; i++)
            {
                EditorGUILayout.LabelField(wanderingTiles[i].ToString());
            }*/
        }
        else
        {
            GUILayout.Space(5);

            //Start Label
            GUILayout.BeginHorizontal();

            sb.Length = 0;
            sb.Append("Select A Character With Right Click");
            textDimensions = GUI.skin.label.CalcSize(new GUIContent(sb.ToString()));
            labelWidth = textDimensions.x;
            GUILayout.Space(Screen.width/2 - labelWidth /2);

            EditorGUILayout.LabelField(sb.ToString());
            
            GUILayout.EndHorizontal();
            // End Label
        }
    }

    // When we close the window, go through all and disable draw gizmos
    private List<AIMovement> allSelectedUntilNow = new List<AIMovement>();
    
    private void OnSceneGUI(SceneView sceneView)
    {
        if( SceneView.lastActiveSceneView != null )
        {
            Handles.BeginGUI();

            // If Toggled, change mouse mode
            if(enablePathMode)
            {
                Tools.current = Tool.None;

                // Disables tool to be able to select game object in Scene
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                
                // Press space to see character gizmo
                if(aiMovement != null)
                {
                    if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
                    {
                        aiMovement.canDrawGizmo = !aiMovement.canDrawGizmo;

                        if(allSelectedUntilNow.Contains(aiMovement))
                        {
                            allSelectedUntilNow.Remove(aiMovement);
                        }
                        else
                        {
                            allSelectedUntilNow.Add(aiMovement);
                            aiMovement.gizmosColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                        }
                    }
                }
            }
            else    // When Toggle is Off
            {
                Tools.current = Tool.Move;

                if(aiMovement != null)
                    aiMovement.isSelectedOnEditor = false;

                foreach (AIMovement character in allSelectedUntilNow)
                {
                    character.canDrawGizmo = false;
                }
                allSelectedUntilNow.Clear();
            }

            PathPainter();

            
            if(enableGridMode)
            {
                if(GridManager.gridManager != null)
                {
                    GridManager.gridManager.canDrawGizmo = true;
                }else
                    Debug.LogError("Game is not being played");
            }
            else
            {
                if(GridManager.gridManager != null)
                {
                    GridManager.gridManager.canDrawGizmo = false;
                }
            }
        

            
            Handles.EndGUI();
            SceneView.lastActiveSceneView.Repaint();
        }
    }

    Vector3 lastMousePos;   
    bool isRemoving;        // when mouse button is hold, if clicked on existing position we are removing tiles, if not we are adding
    void PathPainter()
    {
        if(enablePathMode)
        {
            // Right click to select character
            if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                Vector3 mousePosition = Event.current.mousePosition;
                mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;
                Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
                mousePosition = ray.origin;

                // If it is Character object
                LayerMask mask = LayerMask.NameToLayer("Characters"); if((int)mask == -1) Debug.LogError("Incorrect Mask !");
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, 1 << mask);
                if(hit.collider != null)
                {
                    if(aiMovement != null)
                        aiMovement.isSelectedOnEditor = false;  // old selected set to false

                    aiMovement = hit.collider.gameObject.GetComponent<AIMovement>();  // Targeted Character
                    
                    aiMovement.isSelectedOnEditor = true;   // new selected set to true
                    
                    this.Repaint(); // Update window (runs onGUI method)
                }
            }
        }
        
        //if(Tools.current == Tool.Custom) // if right tool is selected
        if(enablePathMode) // if right tool is selected
        {
            // if character is selected above, proceed in if statemenbt and select what action we are going to do next
            if(aiMovement != null)
            {
                SerializedObject so = new SerializedObject(aiMovement);

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
                        if(!aiMovement.wanderingTiles.Contains(mousePosInt))
                        {
                            //aiMovement.wanderingTiles.Add(mousePosInt);
                            aiPatrolTiles = so.FindProperty("wanderingTiles");
                            if(aiPatrolTiles.arraySize > 0)
                                aiPatrolTiles.InsertArrayElementAtIndex(aiPatrolTiles.arraySize - 1);
                            else
                                aiPatrolTiles.InsertArrayElementAtIndex(0);
                            SerializedProperty sp = aiPatrolTiles.GetArrayElementAtIndex(aiPatrolTiles.arraySize - 1);
                            sp.vector3Value = mousePosInt;

                            so.ApplyModifiedProperties();
                            this.Repaint(); // Update window (runs onGUI method)

                            // Clicked on a position that does not exist in our list
                            isRemoving = false;
                        }
                        else
                        {
                            //aiMovement.wanderingTiles.Remove(mousePosInt);
                            aiPatrolTiles = so.FindProperty("wanderingTiles");
                            int index = aiMovement.wanderingTiles.IndexOf(mousePosInt);
                            aiPatrolTiles.DeleteArrayElementAtIndex(index);
                            
                            so.ApplyModifiedProperties();
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
                                if(aiMovement.wanderingTiles.Contains(mousePosInt))
                                {
                                    //aiMovement.wanderingTiles.Remove(mousePosInt);
                                    aiPatrolTiles = so.FindProperty("wanderingTiles");
                                    int index = aiMovement.wanderingTiles.IndexOf(mousePosInt);
                                    aiPatrolTiles.DeleteArrayElementAtIndex(index);
                                    
                                    so.ApplyModifiedProperties();
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
                                if(!aiMovement.wanderingTiles.Contains(mousePosInt))
                                {
                                    //aiMovement.wanderingTiles.Add(mousePosInt);
                                    aiPatrolTiles = so.FindProperty("wanderingTiles");
                                    aiPatrolTiles.InsertArrayElementAtIndex(aiPatrolTiles.arraySize - 1);
                                    SerializedProperty sp = aiPatrolTiles.GetArrayElementAtIndex(aiPatrolTiles.arraySize - 1);
                                    sp.vector3Value = mousePosInt;
                                    
                                    so.ApplyModifiedProperties();
                                    this.Repaint(); // Update window (runs onGUI method)
                                }
                            }
                            else
                                Debug.LogError("Gridbase is not set!");
                        }
                    }
                }
            }
        }
    }

    // When window is open(even tho not currently selected), OnSceneGUI runs
    void OnEnable()
    {
        SceneView.duringSceneGui += this.OnSceneGUI;
    }
    void OnDisable()
    {
        if(aiMovement != null)
        {
            aiMovement.isSelectedOnEditor = false;
            aiMovement = null;
        }

        foreach (AIMovement character in allSelectedUntilNow)
        {
            character.canDrawGizmo = false;
        }
        allSelectedUntilNow.Clear();

        if(GridManager.gridManager != null)
            GridManager.gridManager.canDrawGizmo = false;

        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    // This makes it so OnSceneGUI runs every frame even tho this(current) window is not open
    // Window has been selected
    /*void OnFocus() 
    {
        // Remove delegate listener if it has previously
        // been assigned.
        SceneView.duringSceneGui -= this.OnSceneGUI;
        // Add (or re-add) the delegate.
        SceneView.duringSceneGui += this.OnSceneGUI;
    }*/
}

// Custom Editor Toolbar
/*[EditorTool("Select Character")]
class SelectCharacter : EditorTool
{
    // Serialize this value to set a default value in the Inspector.
    [SerializeField]
    Texture2D m_ToolIcon = null;

    GUIContent m_IconContent;

    void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "Select Character",
            tooltip = "Select Character"
        };
    }

    public override GUIContent toolbarIcon
    {
        get { return m_IconContent; }
    }

    // This is called for each window that your tool is active in. Put the functionality of your tool here.
    public override void OnToolGUI(EditorWindow window)
    {
        //EditorGUI.BeginChangeCheck();
        
        // Disables tool to be able to select game object in Scene
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Vector3 position = Tools.handlePosition;
        
        using (new Handles.DrawingScope(Color.green))
        {
            position = Handles.Slider(position, Vector3.right);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Vector3 delta = position - Tools.handlePosition;

            Undo.RecordObjects(Selection.transforms, "Move Platform");

            foreach (var transform in Selection.transforms)
                transform.position += delta;
        }
    }
}*/
#endif