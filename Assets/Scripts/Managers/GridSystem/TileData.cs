using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System;

public enum TileType{
    walkable,
    unwalkable,
    seeThrough,
    door,
}

[System.Serializable]
public class TileData : IHeapItem<TileData>
{
    public string name;
    public TileType tileType;
    //public int gridX{get; private set;}   // local tile position on grid
    //public int gridY{get; private set;}   // local tile position on grid
    public int gridX;
    public int gridY;
    //public int visibility;
    public float worldX;                    // world position
    public float worldY;                    // world position
    //public Vector3 worldPos;
    
    public bool walkable;                   // is it obstacle
    public bool doorLocked = false;
    public bool doorOpen = false;

    /// <summary>
    /// This bool is active when character has to change its own sorting order before movement begins.
    /// </summary> 
    public bool instantSortingOrderTransitionBool = false;

    [NonSerialized] public List<TileData> myNeighbours = new List<TileData>();
    [NonSerialized] public List<TileData> myFourNeighbours = new List<TileData>();
    [NonSerialized] public TileData[] closestWalkable = new TileData[4]; // hold closest walkable tile if you are not walkable yourself

    public Tilemap tilemap;
    public TileData parentNode;
    
    public int gCost;   //the distance from starting cell node
    public int hCost;   //the distance from ending cell node
    private int heapIndex;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    
    public int characterSortingOrder = EffectTiles.defaultSortingOrderForCharaters;

    public void Init(int gridX, int gridY, float worldX, float worldY, bool walkable, TileType tileType, Tilemap tilemap)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        this.worldX = worldX;
        this.worldY = worldY;
        this.walkable = walkable;
        this.tileType = tileType;
        this.tilemap = tilemap;
        this.name = gridX + ", " + gridY;
    }

    public int HeapIndex
    {
        get{
            return heapIndex;
        }
        set{
            heapIndex = value;
        }
    }

    public int CompareTo(TileData tdToCompare)
    {
        int compare = fCost.CompareTo(tdToCompare.fCost);
        if(compare == 0){
            compare = hCost.CompareTo(tdToCompare.hCost);
        }
        return -compare;
    }
}