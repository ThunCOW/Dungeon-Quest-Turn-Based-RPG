using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;

//Grid
public class GridManager : MonoBehaviour
{
    [FormerlySerializedAs("tileMaps")]
    [SerializeField] private List<Tilemap> allTileMaps = null;
    //[SerializeField] private GameObject selectionBox = null;
    
    public TileData[,] nodes;           // sorted 2d array of nodes, may contain null entries if the map is of an odd shape e.g. gaps
    public Tile door;

    private void OnValidate()
    {
        /*if(StaticClass.gridBase == null)
            StaticClass.gridBase = gameObject.GetComponent<Grid>();
        
        if(StaticClass.gridManager == null)
            StaticClass.gridManager = this;*/
    }
    
    private void Awake() 
    {
        StaticClass.gridBase = gameObject.GetComponent<Grid>();
        StaticClass.gridManager = this;
    }

    void Start () 
    {
        CreateGrid();
    }

    /*private void Update() {
        if(Input.GetKeyDown(KeyCode.R)){
            Destroy(gridNode.gameObject); 
            GameObject go = new GameObject("GridNode");
            gridNode = go;
            CreateGrid();
        }
    }*/

    public int[] bounds = new int[7];
    void CreateGrid()
    {
        foreach(Tilemap tempTileMap in allTileMaps)
        {   // here we circle through tilemaps to find xMin xMax yMin yMax 
            if(tempTileMap.cellBounds.xMin < bounds[StaticClass.xMin])
                bounds[StaticClass.xMin] = tempTileMap.cellBounds.xMin;
            if(tempTileMap.cellBounds.xMax > bounds[StaticClass.xMax])
                bounds[StaticClass.xMax] = tempTileMap.cellBounds.xMax;
            if(tempTileMap.cellBounds.yMin < bounds[StaticClass.yMin])
                bounds[StaticClass.yMin] = tempTileMap.cellBounds.yMin;
            if(tempTileMap.cellBounds.yMax > bounds[StaticClass.yMax])
                bounds[StaticClass.yMax] = tempTileMap.cellBounds.yMax;
        }

        // create nodes array as big as our map size we calculated above
        nodes = new TileData[Mathf.Abs(bounds[StaticClass.xMin]) + Mathf.Abs(bounds[StaticClass.xMax]) + 1, Mathf.Abs(bounds[StaticClass.yMin]) + Mathf.Abs(bounds[StaticClass.yMax]) + 1];

        // Create tiles 
        for (int x = bounds[StaticClass.xMin]; x < bounds[StaticClass.xMax]; x++)
        {
            for (int y = bounds[StaticClass.yMin]; y < bounds[StaticClass.yMax]; y++)
            {
                Vector3Int localPos = (new Vector3Int(x, y, (int)StaticClass.gridBase.transform.position.z));    // local positions of tiles
                Vector3 worldPos = StaticClass.gridBase.CellToWorld(localPos);                                   // exact world positions of tiles
                
                foreach (Tilemap tileMap in allTileMaps)   // circle through tilemaps in our scene
                {
                    if(tileMap.HasTile(localPos))   // if there is no tile, there is nothing to create or change
                    {
                        TileData tileData = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];

                        if(tileData != null)    // means this position has two tile, like a floor and door on top of it.
                        {
                            if(tileMap.CompareTag("Unwalkable"))
                            {
                                tileData.walkable = false;
                                tileData.tileType = TileType.unwalkable;
                            }
                            if(tileMap.CompareTag("Door"))
                            {
                                tileData.walkable = true;
                                tileData.tileType = TileType.door;
                            }
                        }
                        else    // this position has no TileData created, create TileData for position
                        {
                            tileData = CreateSingularTileData(localPos, tileMap);

                            if(tileMap.CompareTag("Floor"))    // if floor
                            {
                                tileData.Init(localPos.x, localPos.y, worldPos.x, worldPos.y, true, TileType.walkable);
                            }
                            else if(tileMap.CompareTag("Unwalkable"))
                            {
                                tileData.Init(localPos.x, localPos.y, worldPos.x, worldPos.y, false, TileType.unwalkable);
                            }
                            else if(tileMap.CompareTag("Door"))
                            {
                                tileData.Init(localPos.x, localPos.y, worldPos.x, worldPos.y, true, TileType.door);
                            }
                        }
                    }
                }
            }
        }

        // Find neighbours of every tile
        for (int x = bounds[StaticClass.xMin]; x < bounds[StaticClass.xMax]; x++)
        {
            for (int y = bounds[StaticClass.yMin]; y < bounds[StaticClass.yMax]; y++)
            {
                TileData currentTileData = nodes[x + Mathf.Abs(bounds[StaticClass.xMin]), y + Mathf.Abs(bounds[StaticClass.yMin])];
                
                if(currentTileData != null)
                {
                    // c for column r for row, traverse all 8 neighbour of one square tile
                    for(int c = -1; c <= 1; c++)
                    {
                        for(int r = -1; r <= 1; r++)
                        {
                            try
                            {
                                if(c == 0 && r == 0)    // Dont add yourself to neighbour list
                                    continue;
                                
                                TileData neighbour = nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) + c, y + Mathf.Abs(bounds[StaticClass.yMin]) + r];
                                if(neighbour != null)
                                {
                                    // 8 neighbour of tile
                                    currentTileData.myNeighbours.Add(neighbour);
                                    
                                    // 4 neighbour of tile
                                    if(Mathf.Abs(c) != Mathf.Abs(r))
                                    {
                                        currentTileData.myFourNeighbours.Add(neighbour);
                                    }
                                }
                            }
                            catch{}
                        }
                    }
                    
                    // Find closest walkable tile for 4 directions
                    if(currentTileData.walkable == false)
                    {

                        /* direction 
                            0 = south   / -y
                            1 = west    / -x
                            2 = north   / y
                            3 = east    / x
                        */
                        for(int direction = 0; direction < 4; direction++)
                        {
                            int increasePoint = 1;
                            if(direction == 0 || direction == 1)
                            {
                                increasePoint = -1; // -y and -x will decrease in number, others will increase
                            }
                            
                            // For every direction we will continue in that direction until we find a tile or find that there is no tile
                            TileData walkableTile = null;
                            while(true)
                            {
                                if(direction == 0 || direction == 2)
                                {
                                    try
                                    {
                                        // Only y axis will increase / decrease
                                        walkableTile = nodes[x + Mathf.Abs(bounds[StaticClass.xMin]), y + Mathf.Abs(bounds[StaticClass.yMin]) + increasePoint];
                                    }catch{}
                                    
                                    // if goes out of bound, means there is a wall tile that connected to nothing
                                    if(bounds[StaticClass.yMin] > increasePoint || bounds[StaticClass.yMax] < increasePoint)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        // Only x axis will increase / decrease
                                        walkableTile = nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) + increasePoint, y + Mathf.Abs(bounds[StaticClass.yMin])];
                                    }catch{}
                                    
                                    // if goes out of bound, means there is a wall tile that connected to nothing
                                    if(bounds[StaticClass.xMin] > increasePoint || bounds[StaticClass.xMax] < increasePoint)
                                    {
                                        break;
                                    }
                                }

                                if(walkableTile == null)
                                    break;

                                if(walkableTile.walkable)
                                {
                                    currentTileData.closestWalkable[direction] = walkableTile;
                                    break;
                                }
                                increasePoint = (increasePoint > 0)?++increasePoint:--increasePoint;
                            }
                        }
                    }
                }
            }
        }
    }

    public TileData CreateSingularTileData(Vector3Int localPos, Tilemap tilemap)
    {
        TileData td = new TileData();
        nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])] = td;
        StaticClass.tileCount++;

        td.tilemap = tilemap;
        return td;
    }

    // This will translate world position to local position before locating TD
    public TileData GetTileDataByLocalPosition(Vector3 worldPosition)
    {
        Vector3Int localPos = StaticClass.gridBase.WorldToCell(worldPosition); // grid component can be used instead of this
        
        TileData tempTileData = null;
        try{
            tempTileData = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];
            if(tempTileData.walkable)
                return tempTileData;
            else
                return tempTileData;
        }catch{
            // target is outside of map
        }
        return null;
    }

    public void UnBlockTile(Vector3 tilePos)
    {
        Vector3Int localPos = StaticClass.gridBase.WorldToCell(tilePos); // grid component can be used instead of this

        TileData td = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];
        td.walkable = true;
    }
    
    public void BlockTile(Vector3 tilePos)
    {
        Vector3Int localPos = StaticClass.gridBase.WorldToCell(tilePos); // grid component can be used instead of this

        TileData td = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];
        td.walkable = false;
    }

    // TODO: Duplicate of PathFinding GetDistance
    private int GetDistance(TileData nodeA, TileData nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX- nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
    
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    [HideInInspector] public bool canDrawGizmo = false;
    private void OnDrawGizmos() 
    {
        if(canDrawGizmo)
        {
            if(nodes != null){
                int i = 0;
                foreach (TileData tempTileData in nodes)
                {
                    if(tempTileData != null)
                    {
                        i++;
                        if(tempTileData.tilemap.CompareTag("Door"))
                            Gizmos.color = Color.magenta;
                        else
                            Gizmos.color = (tempTileData.walkable)?Color.green:Color.red;
                        
                        Vector3 tempWorldPos = new Vector3(tempTileData.worldX + StaticClass.cellSize / 2, tempTileData.worldY + StaticClass.cellSize / 2, 0);
                        Gizmos.DrawCube(tempWorldPos, Vector3.one * 0.35f);
                    }
                }
                //Debug.Log("Tile Number: " + i);
            }
        }
    }
}