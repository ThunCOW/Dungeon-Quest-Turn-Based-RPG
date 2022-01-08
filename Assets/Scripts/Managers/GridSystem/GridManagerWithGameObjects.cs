using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Grid
public class GridManagerWithGameObjects : MonoBehaviour
{
    [SerializeField] private List<Tilemap> tileMaps = null;
    //[SerializeField] private GameObject selectionBox = null;
 
    [SerializeField] private GameObject gridNode = null;            // where the generated tiles will be stored
    [SerializeField] private GameObject nodePrefab = null;          // world tile prefab
    
    private GameObject[,] nodes;           // sorted 2d array of nodes, may contain null entries if the map is of an odd shape e.g. gaps
    public Tile door;
    private void Awake()
    {
        //StaticClass.gridManager = this;
        GridManager.gridBase = gameObject.GetComponent<Grid>();
    }

    void Start ()
    {
        CreateGrid();
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Destroy(gridNode.gameObject); 
            GameObject go = new GameObject("GridNode");
            gridNode = go;
            CreateGrid();
        }
    }

    void CreateGrid(){

        foreach(Tilemap tempTileMap in tileMaps){   // here we circle through tilemaps to find xMin xMax yMin yMax 
            if(tempTileMap.cellBounds.xMin < bounds[StaticClass.xMin])
                bounds[StaticClass.xMin] = tempTileMap.cellBounds.xMin;
            if(tempTileMap.cellBounds.xMax > bounds[StaticClass.xMax])
                bounds[StaticClass.xMax] = tempTileMap.cellBounds.xMax;
            if(tempTileMap.cellBounds.yMin < bounds[StaticClass.yMin])
                bounds[StaticClass.yMin] = tempTileMap.cellBounds.yMin;
            if(tempTileMap.cellBounds.yMax > bounds[StaticClass.yMax])
                bounds[StaticClass.yMax] = tempTileMap.cellBounds.yMax;
        }
        //Debug.Log("xMin = " + bounds[StaticClass.xMin] + " xMax = " + bounds[StaticClass.xMax] + "\nyMin = " + bounds[StaticClass.yMin] + " yMax = " + bounds[StaticClass.yMax]);
        nodes = new GameObject[Mathf.Abs(bounds[StaticClass.xMin]) + Mathf.Abs(bounds[StaticClass.xMax]) + 1, Mathf.Abs(bounds[StaticClass.yMin]) + Mathf.Abs(bounds[StaticClass.yMax]) + 1];
        //tileCount = Mathf.Abs(bounds[StaticClass.xMin]) + Mathf.Abs(bounds[StaticClass.xMax]) * Mathf.Abs(bounds[StaticClass.yMin]) + Mathf.Abs(bounds[StaticClass.yMax]);

        for (int x = bounds[StaticClass.xMin]; x < bounds[StaticClass.xMax]; x++){
            for (int y = bounds[StaticClass.yMin]; y < bounds[StaticClass.yMax]; y++){
                foreach (Tilemap tempTileMap in tileMaps)   // circle through tilemaps in our scene and check their tag next
                {
                    Vector3Int localPos = (new Vector3Int(x, y, (int)tempTileMap.transform.position.z));    // local positions of tiles
                    Vector3 worldPos = tempTileMap.CellToWorld(localPos);                                   // exact world positions of tiles
                    worldPos = new Vector3(worldPos.x + StaticClass.cellSize/2, worldPos.y + StaticClass.cellSize/2, worldPos.z);   // being used to create a game object in the middle of a tile
                    
                    // if node already exist, and next tilemap has tile over same node, dont create another gameObject.
                    //TODO: if next tilemap blocks the way, like a door, set it to not walkable
                    if(nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])] != null){
                        //Debug.Log("node already exist" + nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])].name);
                        if(tempTileMap.CompareTag("Unwalkable") && tempTileMap.HasTile(localPos))
                        {
                            TileData td = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])].GetComponent<TileData>();
                            GameObject node = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];

                            td.walkable = false;
                            td.tileType = TileType.unwalkable;
                            node.name = "Unwalkable_" + x.ToString() + "_" + y.ToString();
                            node.GetComponent<SpriteRenderer>().color = Color.red;
                        }
                        if(tempTileMap.CompareTag("Door") && tempTileMap.HasTile(localPos)){
                            TileData td = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])].GetComponent<TileData>();
                            GameObject node = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];

                            td.walkable = true;
                            td.tileType = TileType.door;
                            node.name = "Door_" + x.ToString() + "_" + y.ToString();
                            node.GetComponent<SpriteRenderer>().color = Color.magenta;
                        }

                    }
                    else{
                        if(tempTileMap.CompareTag("Floor") && tempTileMap.HasTile(localPos))    // if floor
                        {
                            //Debug.Log("new node" + localPos);
                            GameObject node = (GameObject)Instantiate(nodePrefab, worldPos, Quaternion.Euler(0, 0, 0));
                            TileData td = node.GetComponent<TileData>();
                            td.Init(localPos.x, localPos.y, worldPos.x, worldPos.y, true, TileType.walkable, tempTileMap);
                            node.transform.parent = gridNode.transform;
                            nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])] = node;
                            StaticClass.tileCount++;

                            td.tilemap = tempTileMap;
                            node.name = "Walkable_" + x.ToString() + "_" + y.ToString();
                            node.GetComponent<SpriteRenderer>().color = Color.green;
                        }
                        else if(tempTileMap.CompareTag("Unwalkable") && tempTileMap.HasTile(localPos))
                        {
                            //Debug.Log("new node" + localPos);
                            GameObject node = (GameObject)Instantiate(nodePrefab, worldPos, Quaternion.Euler(0, 0, 0));
                            TileData td = node.GetComponent<TileData>();
                            td.Init(localPos.x, localPos.y, worldPos.x, worldPos.y, false, TileType.unwalkable, tempTileMap);
                            node.transform.parent = gridNode.transform;
                            nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])] = node;
                            StaticClass.tileCount++;

                            td.tilemap = tempTileMap;
                            node.name = "Unwalkable_" + x.ToString() + "_" + y.ToString();
                            node.GetComponent<SpriteRenderer>().color = Color.red;
                        }else if(tempTileMap.CompareTag("Door") && tempTileMap.HasTile(localPos))
                        {
                            //Debug.Log("new node" + localPos);
                            GameObject node = (GameObject)Instantiate(nodePrefab, worldPos, Quaternion.Euler(0, 0, 0));
                            TileData td = node.GetComponent<TileData>();
                            td.Init(localPos.x, localPos.y, worldPos.x, worldPos.y, true, TileType.door, tempTileMap);
                            node.transform.parent = gridNode.transform;
                            nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])] = node;
                            StaticClass.tileCount++;

                            td.tilemap = tempTileMap;
                            node.name = "Door_" + x.ToString() + "_" + y.ToString();
                            node.GetComponent<SpriteRenderer>().color = Color.magenta;
                        }
                    }
                }
            }
        }

        // every tile stores their neighbours
        for (int x = bounds[StaticClass.xMin]; x < bounds[StaticClass.xMax]; x++){
            for (int y = bounds[StaticClass.yMin]; y < bounds[StaticClass.yMax]; y++){
                if (nodes[x + Mathf.Abs(bounds[StaticClass.xMin]), y + Mathf.Abs(bounds[StaticClass.yMin])] != null) { // if there is a tile at this position, get its neighbours
                    TileData td = nodes[x + Mathf.Abs(bounds[StaticClass.xMin]), y + Mathf.Abs(bounds[StaticClass.yMin])].GetComponent<TileData>(); 
                    if(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) + 1, y + Mathf.Abs(bounds[StaticClass.yMin])] != null){ // x + 1 yani sağında node var mı
                        td.myNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) + 1, y + Mathf.Abs(bounds[StaticClass.yMin])].GetComponent<TileData>()); 
                        td.myFourNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) + 1, y + Mathf.Abs(bounds[StaticClass.yMin])].GetComponent<TileData>());
                    }
                    if(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) + 1, y + Mathf.Abs(bounds[StaticClass.yMin]) + 1] != null) // x + 1 y + 1 yani sağ çapraz üst node var mı
                        td.myNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) + 1, y + Mathf.Abs(bounds[StaticClass.yMin]) + 1].GetComponent<TileData>());
                    try{    // index -1 olunca hata almamak için 
                        if(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) + 1, y + Mathf.Abs(bounds[StaticClass.yMin]) - 1] != null) // x + 1 y - 1 yani sağ çapraz alt node var mı
                            td.myNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) + 1, y + Mathf.Abs(bounds[StaticClass.yMin]) - 1].GetComponent<TileData>());
                    }catch{}
                    try{
                        if(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) - 1, y + Mathf.Abs(bounds[StaticClass.yMin])] != null){ // x - 1 yani solunda node var mı
                            td.myNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) - 1, y + Mathf.Abs(bounds[StaticClass.yMin])].GetComponent<TileData>()); 
                            td.myFourNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) - 1, y + Mathf.Abs(bounds[StaticClass.yMin])].GetComponent<TileData>());
                        }
                    }catch{}
                    try{
                        if(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) - 1, y + Mathf.Abs(bounds[StaticClass.yMin]) + 1] != null) // x - 1 y + 1 yani sol çapraz üst node var mı
                            td.myNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) - 1, y + Mathf.Abs(bounds[StaticClass.yMin]) + 1].GetComponent<TileData>());
                    }catch{}
                    try{
                        if(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) - 1, y + Mathf.Abs(bounds[StaticClass.yMin]) - 1] != null) // x - 1 y - 1 yani sol çapraz alt node var mı
                            td.myNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]) - 1, y + Mathf.Abs(bounds[StaticClass.yMin]) - 1].GetComponent<TileData>());
                    }catch{}
                    if(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]), y + Mathf.Abs(bounds[StaticClass.yMin]) + 1] != null){ // y + 1 yani üstte node var mı
                        td.myNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]), y + Mathf.Abs(bounds[StaticClass.yMin]) + 1].GetComponent<TileData>()); 
                        td.myFourNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]), y + Mathf.Abs(bounds[StaticClass.yMin]) + 1].GetComponent<TileData>());
                    }
                    try{
                        if(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]), y + Mathf.Abs(bounds[StaticClass.yMin]) - 1] != null){ // y - 1 yani altta node var mı
                            td.myNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]), y + Mathf.Abs(bounds[StaticClass.yMin]) - 1].GetComponent<TileData>()); 
                            td.myFourNeighbours.Add(nodes[x + Mathf.Abs(bounds[StaticClass.xMin]), y + Mathf.Abs(bounds[StaticClass.yMin]) - 1].GetComponent<TileData>());
                        }
                    }catch{}
                }
            }
        }
    }

    public TileData GetTileDataByLocalPosition(Vector3 worldPosition)
    {
        Vector3Int localPos = GridManager.gridBase.WorldToCell(worldPosition); // grid component can be used instead of this
        
        TileData tempTileData = null;
        try{
            tempTileData = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])].GetComponent<TileData>();
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
        Vector3Int localPos = GridManager.gridBase.WorldToCell(tilePos); // grid component can be used instead of this

        GameObject tile = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];

        TileData td = tile.GetComponent<TileData>();
        td.walkable = true;
        tile.name = "Walkable_" + ((int)tilePos.x).ToString() + "_" + ((int)tilePos.y).ToString();
        tile.GetComponent<SpriteRenderer>().color = Color.green;
    }
    
    public void BlockTile(Vector3 tilePos)
    {
        Vector3Int localPos = GridManager.gridBase.WorldToCell(tilePos); // grid component can be used instead of this

        GameObject tile = nodes[localPos.x + Mathf.Abs(bounds[StaticClass.xMin]), localPos.y + Mathf.Abs(bounds[StaticClass.yMin])];

        TileData td = tile.GetComponent<TileData>();
        td.walkable = false;
        tile.name = "Unwalkable_" + ((int)tilePos.x).ToString() + "_" + ((int)tilePos.y).ToString();
        tile.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public int[] bounds = new int[7];
}