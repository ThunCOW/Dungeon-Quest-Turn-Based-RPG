using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDataOld : MonoBehaviour
{
    //public int gridX{get; private set;}   // local tile position on grid
    //public int gridY{get; private set;}   // local tile position on grid
    public int gridX;
    public int gridY;
    public float worldX;                    // world position
    public float worldY;                    // world position
    
    public bool walkable;                   // is it obstacle
    //private float cellSize = 0.7f;          // scale of grid, thus a tile

    public List<TileData> myNeighbours;
    public TileData parentNode;
    public int gCost;
    public int hCost;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    private void Start() {
        worldX = transform.position.x;
        worldY = transform.position.y;
    }

    public void Init(int gridX, int gridY, bool walkable) {
        this.gridX = gridX;
        this.gridY = gridY;
        this.walkable = walkable;

        /*Debug.DrawLine(new Vector3(width, height, 0), new Vector3(width, height + cellSize, 0), Color.white, 100f);
        Debug.DrawLine(new Vector3(width, height, 0), new Vector3(width + cellSize, height, 0), Color.white, 100f);
        
        Debug.DrawLine(new Vector3(width, height + cellSize, 0), new Vector3(width + cellSize, height + cellSize, 0), Color.white, 100f);
        Debug.DrawLine(new Vector3(width + cellSize, height, 0), new Vector3(width + cellSize, height + cellSize, 0), Color.white, 100f);*/
    }
}
