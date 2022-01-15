using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectTiles : MonoBehaviour
{
    [HideInInspector] public List<Vector3Int> blockedPositions = new List<Vector3Int>();
    
    /// <summary>
    /// The tile positions when character is in front of object
    /// </summary>
    [HideInInspector] public List<Vector3Int> characterInFront = new List<Vector3Int>();
    
    /// <summary>
    /// The tile positions when character is in front of object and can not move to behind
    /// </summary>
    [HideInInspector] public List<Vector3Int> characterFrontOnly = new List<Vector3Int>();
    
    /// <summary>
    /// The tile positions when character is behind of an object
    /// </summary>
    [HideInInspector] public List<Vector3Int> characterBehind = new List<Vector3Int>();

    /// <summary>
    /// The tile positions when character needs to instantly change sorting order to not look like sliding under or hovering above objects
    /// </summary>
    [HideInInspector] public List<Vector3Int> instantSortingOrderTransitionPositions = new List<Vector3Int>();
    
    /// <summary>
    /// This value is being used to differentiate tiles with objects and without, so character can't move from front to behind.
    /// </summary>
    public const int defaultSortingOrderForCharaters = 32;

    /// <summary>
    /// The character's sorting order when standing in front of an object
    /// </summary>
    public const int frontSortingOrder = 30;
    
    /// <summary>
    /// The character's sorting order when can not move from front to behind
    /// </summary>
    public const int frontOnlySortingOrder = 31;

    /// <summary>
    /// The character's sorting order when standing behind of an object
    /// </summary>
    public const int behindSortingOrder = 10;

    // Start is called before the first frame update
    void Start()
    {
        ObjectTileEffect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ObjectTileEffect()
    {
        /*
                Character Sorting Order Changes
        */
        foreach(Vector3Int vec in characterInFront)
        {
            TileData td = GridManager.gridManager.GetTileDataByLocalPosition(vec);
            td.characterSortingOrder = frontSortingOrder;
        }
        foreach(Vector3Int vec in characterFrontOnly)
        {
            TileData td = GridManager.gridManager.GetTileDataByLocalPosition(vec);
            td.characterSortingOrder = frontOnlySortingOrder;
        }
        foreach(Vector3Int vec in characterBehind)
        {
            TileData td = GridManager.gridManager.GetTileDataByLocalPosition(vec);
            td.characterSortingOrder = behindSortingOrder;
        }
        foreach(Vector3Int vec in instantSortingOrderTransitionPositions)
        {
            TileData td = GridManager.gridManager.GetTileDataByLocalPosition(vec);
            td.instantSortingOrderTransitionBool = true;
        }

        /*
                Object Blocking Tile Changes
        */
        foreach(Vector3Int vec in blockedPositions)
        {
            GridManager.gridManager.BlockTile(vec);
        }
    }

#if UNITY_EDITOR
    // Gizmos for WanderingPath creator editor
    [HideInInspector] public bool isSelectedOnEditor = false;
    [ExecuteInEditMode]
    protected virtual void OnDrawGizmos() 
    {

        if(isSelectedOnEditor)
        {
            if(characterInFront.Count > 0)
            {
                foreach (Vector3 tempPos in characterInFront)
                {
                    Gizmos.color = Color.green;

                    Vector3 pos = tempPos + (Vector3.one * StaticClass.cellSize) / 2;

                    Gizmos.DrawCube(pos, Vector3.one * 0.35f);
                }
            }
            if(characterFrontOnly.Count > 0)
            {
                foreach (Vector3 tempPos in characterFrontOnly)
                {
                    Gizmos.color = Color.green;
                    Vector3 pos = tempPos + (Vector3.one * StaticClass.cellSize) / 2;
                    Gizmos.DrawCube(pos, Vector3.one * 0.35f);
                    
                    Gizmos.color = Color.gray;
                    Gizmos.DrawCube(pos, Vector3.one * 0.17f);
                }
            }
            if(characterBehind.Count > 0)
            {
                foreach (Vector3 tempPos in characterBehind)
                {
                    Gizmos.color = Color.yellow;

                    Vector3 pos = tempPos + (Vector3.one * StaticClass.cellSize) / 2;

                    Gizmos.DrawCube(pos, Vector3.one * 0.35f);
                }
            }
            if(blockedPositions.Count > 0)
            {
                foreach (Vector3 tempPos in blockedPositions)
                {
                    Gizmos.color = Color.red;

                    Vector3 pos = tempPos + (Vector3.one * StaticClass.cellSize) / 2;

                    Gizmos.DrawCube(pos, Vector3.one * 0.35f);
                }
            }
            if(instantSortingOrderTransitionPositions.Count > 0)
            {
                foreach (Vector3 tempPos in instantSortingOrderTransitionPositions)
                {
                    Gizmos.color = Color.magenta;

                    Vector3 pos = tempPos + (Vector3.one * StaticClass.cellSize) / 2;

                    Gizmos.DrawCube(pos, Vector3.one * 0.17f);
                }
            }
        }
    }
#endif

}