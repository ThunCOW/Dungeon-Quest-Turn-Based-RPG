using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectTiles : MonoBehaviour
{
    [HideInInspector] public List<Vector3Int> characterInFront = new List<Vector3Int>();
    [HideInInspector] public List<Vector3Int> characterBehind = new List<Vector3Int>();
    [HideInInspector] public List<Vector3Int> blockedPositions = new List<Vector3Int>();

    /// <summary>
    /// The character's sorting order when standing in front of an object
    /// </summary>
    public const int frontSortingOrder = 30;

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
        foreach(Vector3Int vec in characterBehind)
        {
            TileData td = GridManager.gridManager.GetTileDataByLocalPosition(vec);
            td.characterSortingOrder = behindSortingOrder;
        }

        /*
                Object Blocking Tile Changes
        */
        foreach(Vector3Int vec in blockedPositions)
        {
            GridManager.gridManager.BlockTile(vec);
        }
    }

    // Gizmos for WanderingPath creator editor
    [HideInInspector] public bool isSelectedOnEditor = false;
    [ExecuteInEditMode]
    protected virtual void OnDrawGizmos() 
    {
        #if UNITY_EDITOR

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
        }
    }

        #endif
}
