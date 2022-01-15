﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDataWithGameObject : MonoBehaviour, IHeapItem<TileData>
{
    public TileType tileType;
    //public int gridX{get; private set;}   // local tile position on grid
    //public int gridY{get; private set;}   // local tile position on grid
    public int gridX;
    public int gridY;
    //public int visibility;
    public float worldX;                    // world position
    public float worldY;                    // world position
    
    public bool walkable;                   // is it obstacle
    public bool doorLocked = false;
    public bool doorOpen = false;

    public List<TileData> myNeighbours;
    public List<TileData> myFourNeighbours;

    public TileData parentNode;
    
    public int gCost;   //the distance from starting cell node
    public int hCost;   //the distance from ending cell node
    private int heapIndex;

    public Tilemap tilemap;
    public TileData closestWalkable; // hold closest walkable tile if you are not walkable yourself
    
    public int fCost    
    {
        get
        {
            return gCost + hCost;
        }
    }

    public void Init(int gridX, int gridY, float worldX, float worldY, bool walkable, TileType tileType) {
        this.gridX = gridX;
        this.gridY = gridY;
        this.worldX = worldX;
        this.worldY = worldY;
        this.walkable = walkable;
        this.tileType = tileType;
    }

    public int HeapIndex{
        get{
            return heapIndex;
        }
        set{
            heapIndex = value;
        }
    }

    public int CompareTo(TileData tdToCompare){
        int compare = fCost.CompareTo(tdToCompare.fCost);
        if(compare == 0){
            compare = hCost.CompareTo(tdToCompare.hCost);
        }
        return -compare;
    }
}
