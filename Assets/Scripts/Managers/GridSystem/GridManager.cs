using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;

//Grid
public class GridManager : MonoBehaviour
{
    [FormerlySerializedAs("tileMaps")]
    [SerializeField] private List<Tilemap> allTilemaps = null;
    //[SerializeField] private GameObject selectionBox = null;
    
    [SerializeField, HideInInspector] private TileData[,] nodes;           // sorted 2d array of nodes, may contain null entries if the map is of an odd shape e.g. gaps
    
    public Tile door;

    public static Grid gridBase;
    public static GridManager gridManager;

    [Space]
    public bool FixBounds = false;
    private void OnValidate()
    {
        /*if(gridBase == null)
            gridBase = gameObject.GetComponent<Grid>();

        if(gridManager == null)
            gridManager = this;*/

        /* 
            Functions with triggers
        */

        if(FixBounds)
        {
            FixBounds = false;
            foreach(Tilemap tilemap in allTilemaps)
            {
                tilemap.CompressBounds();
            }
        }

        /*if(createMapData)
        {
            createMapData = false;
            foreach(Tilemap tilemap in allTilemaps)
            {
                // here we circle through tilemaps to find xMin xMax yMin yMax
                if(tilemap.cellBounds.xMin < bounds[StaticClass.xMin])
                    bounds[StaticClass.xMin] = tilemap.cellBounds.xMin;
                if(tilemap.cellBounds.xMax > bounds[StaticClass.xMax])
                    bounds[StaticClass.xMax] = tilemap.cellBounds.xMax;
                if(tilemap.cellBounds.yMin < bounds[StaticClass.yMin])
                    bounds[StaticClass.yMin] = tilemap.cellBounds.yMin;
                if(tilemap.cellBounds.yMax > bounds[StaticClass.yMax])
                    bounds[StaticClass.yMax] = tilemap.cellBounds.yMax;
            }
            CreateGrid();
        }*/
    }
    
    [Space]
    public bool createMapData = false;

    private void Awake()
    {
        if(gridBase == null)
            gridBase = gameObject.GetComponent<Grid>();

        if(gridManager == null)
            gridManager = this;
        
        //CreateGrid();
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

    public static int[] bounds = new int[7];
    void CreateGrid()
    {
        Debug.Log("Creating Grid");
        foreach(Tilemap tilemap in allTilemaps)
        {
            // here we circle through tilemaps to find xMin xMax yMin yMax
            if(tilemap.cellBounds.xMin < bounds[StaticClass.xMin])
                bounds[StaticClass.xMin] = tilemap.cellBounds.xMin;
            if(tilemap.cellBounds.xMax > bounds[StaticClass.xMax])
                bounds[StaticClass.xMax] = tilemap.cellBounds.xMax;
            if(tilemap.cellBounds.yMin < bounds[StaticClass.yMin])
                bounds[StaticClass.yMin] = tilemap.cellBounds.yMin;
            if(tilemap.cellBounds.yMax > bounds[StaticClass.yMax])
                bounds[StaticClass.yMax] = tilemap.cellBounds.yMax;
        }

        // create nodes array as big as our map size we calculated above
        nodes = new TileData[Mathf.Abs(bounds[StaticClass.xMin]) + Mathf.Abs(bounds[StaticClass.xMax]) + 1, Mathf.Abs(bounds[StaticClass.yMin]) + Mathf.Abs(bounds[StaticClass.yMax]) + 1];
        
        // Create tiles 
        for (int x = bounds[StaticClass.xMin]; x < bounds[StaticClass.xMax]; x++)
        {
            for (int y = bounds[StaticClass.yMin]; y < bounds[StaticClass.yMax]; y++)
            {
                Vector3Int localPos = (new Vector3Int(x, y, (int)gridBase.transform.position.z));    // local positions of tiles
                Vector3 worldPos = gridBase.CellToWorld(localPos);                                   // exact world positions of tiles
                
                foreach (Tilemap tilemap in allTilemaps)   // circle through tilemaps in our scene
                {
                    if(tilemap.HasTile(localPos))   // if there is no tile, there is nothing to create or change
                    {
                        TileData tileData = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];

                        if(tileData != null)    // means this position has two tile, like a floor and door on top of it.
                        {
                            if(tilemap.CompareTag("Unwalkable"))
                            {
                                tileData.walkable = false;
                                tileData.tileType = TileType.unwalkable;
                            }
                            if(tilemap.CompareTag("Door"))
                            {
                                tileData.walkable = true;
                                tileData.tileType = TileType.door;
                            }
                            if(tilemap.CompareTag("SeeThrough"))
                            {
                                tileData.walkable = false;
                                tileData.tileType = TileType.seeThrough;
                            }
                        }
                        else    // this position has no TileData created, create TileData for position
                        {
                            tileData = CreateSingularTileData(localPos);

                            if(tilemap.CompareTag("Floor"))    // if floor
                            {
                                tileData.Init(localPos.x, localPos.y, worldPos.x, worldPos.y, true, TileType.walkable, tilemap);
                            }
                            else if(tilemap.CompareTag("Unwalkable"))
                            {
                                tileData.Init(localPos.x, localPos.y, worldPos.x, worldPos.y, false, TileType.unwalkable, tilemap);
                            }
                            else if(tilemap.CompareTag("Door"))
                            {
                                tileData.Init(localPos.x, localPos.y, worldPos.x, worldPos.y, true, TileType.door, tilemap);
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

                        // direction 
                        //  0 = south   / -y
                        //  1 = west    / -x
                        //  2 = north   / y
                        //  3 = east    / x
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

    public TileData CreateSingularTileData(Vector3Int localPos)
    {
        TileData td = new TileData();
        nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])] = td;
        StaticClass.tileCount++;

        return td;
    }

    // This will translate world position to local position before locating TD
    public TileData GetTileDataByLocalPosition(Vector3 worldPosition)
    {
        Vector3Int localPos = gridBase.WorldToCell(worldPosition); // grid component can be used instead of this
        
        TileData tileData = null;
        try
        {
            tileData = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];
            if(tileData.walkable)
                return tileData;
            else
                return tileData;
        }
        catch
        {
            // target is outside of map
        }
        return null;
    }

    public void UnBlockTile(Vector3 tilePos)
    {
        Vector3Int localPos = gridBase.WorldToCell(tilePos); // grid component can be used instead of this

        TileData td = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];
        td.walkable = true;
    }
    
    public void BlockTile(Vector3 tilePos)
    {   
        Vector3Int localPos = gridBase.WorldToCell(tilePos); // grid component can be used instead of this

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
                foreach (TileData tileData in nodes)
                {
                    if(tileData != null)
                    {
                        i++;
                        if(tileData.tilemap.CompareTag("Door"))
                            Gizmos.color = Color.magenta;
                        else
                            Gizmos.color = (tileData.walkable)?Color.green:Color.red;
                        
                        Vector3 worldPos = new Vector3(tileData.worldX + StaticClass.cellSize / 2, tileData.worldY + StaticClass.cellSize / 2, 0);
                        Gizmos.DrawCube(worldPos, Vector3.one * 0.35f);
                    }
                }
                //Debug.Log("Tile Number: " + i);
            }
        }
    }
}