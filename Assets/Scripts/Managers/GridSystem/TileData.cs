using System.Collections.Generic;
using UnityEngine.Tilemaps;
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
    public string name;                     // tile name
    public TileType tileType;               // tile types
    public int gridX{get; private set;}     // local tile position on grid
    public int gridY{get; private set;}     // local tile position on grid
    public float worldX;                    // world position
    public float worldY;                    // world position
    
    public bool walkable;                   // is it obstacle
    
    /// <summary>
    /// Character may change sorting order depending on tile to stay behind or on front of objects, we store that information on tile.
    /// </summary> 
    public int characterSortingOrder = EffectTiles.defaultSortingOrderForCharaters;
    
    /// <summary>
    /// This bool is active when character has to change its own sorting order before movement begins, so it won't move over objects etc.
    /// </summary> 
    public bool instantSortingOrderTransitionBool = false;


    [NonSerialized] public List<TileData> myNeighbours = new List<TileData>();
    [NonSerialized] public List<TileData> myFourNeighbours = new List<TileData>();
    [NonSerialized] public TileData[] closestWalkable = new TileData[4]; // hold closest walkable tile if you are not walkable yourself

    public Tilemap tilemap;                 // The tilemap this TileData belongs to 
    public TileData parentNode;             // Required for pathing, when making a path every tile holds parent node to create a path
    
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
    
    public MainTileObject tileObject;       // A scriptable which stores some information, refer to object for more info.

    public void Init(int gridX, int gridY, float worldX, float worldY, bool walkable, TileType tileType, Tilemap tilemap, MainTileObject tileObject = null)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        this.worldX = worldX;
        this.worldY = worldY;
        this.walkable = walkable;
        this.tileType = tileType;
        this.tilemap = tilemap;
        this.tileObject = tileObject;
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

public class DoorTile : TileData
{
    public Tile tileOpen;
    public Tile tileClosed;
    
    public bool doorLocked = false;
    public bool doorOpen = false;
}