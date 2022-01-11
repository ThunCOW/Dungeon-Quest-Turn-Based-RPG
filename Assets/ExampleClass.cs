using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Cell
{
    TileData tileData;
    public Cell(int gridX, int gridY, int worldX, int worldY, bool walkable, TileType tileType, Tilemap tilemap)
    {
        tileData = new TileData();
        tileData.Init(gridX, gridY, worldX, worldY, walkable, tileType, tilemap);
    }
}

[System.Serializable]
public class Array
{
    public List<Cell> cells = new List<Cell>();
    public Cell this[int index] => cells[index];
}

[System.Serializable]
public class TileMatrix
{
    public List<Array> arrays = new List<Array>();
    public Cell this[int x, int y] => arrays[x][y];
}

public class ExampleClass : MonoBehaviour
{
    private void OnValidate() 
    {
        TileMatrix[,] tm = new TileMatrix[4,4];
        Debug.Log(tm[0,1]);
    }
}