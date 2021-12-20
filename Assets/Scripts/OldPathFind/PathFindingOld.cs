using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFindingOld : MonoBehaviour
{
    /*public GameObject walkableTiles;
    public Transform seeker, target;

    GridManager grid;

    public List<TileData> path;

    void Start()
    {
        grid = StaticClass.grid;
    }

    void Update()
    {
        if(Input.GetMouseButton(1)){
            path.Clear();
            FindPath(seeker.position, target.position);
        }
        if(Input.GetMouseButtonDown(0)){
            Stopwatch sw = new Stopwatch();
            sw.Start();

            path.Clear();
            
            for(int i = 0; i < walkableTiles.transform.childCount; i++)
                FindPath(seeker.position, walkableTiles.transform.GetChild(i).transform.position);

            sw.Stop();
            print("path found: " + sw.ElapsedMilliseconds + " ms");
        }
    }

    void FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        TileData startNode = grid.GetTileDataByLocalPosition(startPosition);
        TileData targetNode = grid.GetTileDataByLocalPosition(endPosition);
        if(targetNode == null)
            return;
        if(targetNode.walkable == false){
            // if target is unwalkable we can search for closest walkable tile if needed, not gonna implement yet.
        }
    
        Heap<TileData> openSet = new Heap<TileData>(StaticClass.tileCount);     // holds walkable tiles
        HashSet<TileData> closedSet = new HashSet<TileData>();              // holds the tiles that are already been walked through
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            TileData currentNode = openSet.RemoveFirst();   // tile that we will move to
            
            closedSet.Add(currentNode); 
    
            if (currentNode == targetNode)  // if the next tile is the target, path is found, exit while
            {
                //sw.Stop();
                //print("path found: " + sw.ElapsedMilliseconds + " ms");

                RetracePath(startNode, targetNode);
                return;
            }
    
            // if path is not found
            foreach (TileData neighbour in currentNode.myFourNeighbours) {
                if(!neighbour.walkable && neighbour == targetNode){     // if target is a not walkable tile, and neighbour to a walkable tile
                    neighbour.parentNode = currentNode;                 // dont add it to walkable path but still calculate a path                         
                    RetracePath(startNode, targetNode);
                    return;
                }
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;     // if neighbour is not walkable or we already been on it, skip
    
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parentNode = currentNode;
    
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }
    }

    void FindClosestWalkable(Vector3 startPosition, Vector3 endPosition){

        TileData startNode = grid.GetTileDataByLocalPosition(endPosition);
        TileData targetNode = grid.GetTileDataByLocalPosition(startPosition);
        if(targetNode == null)
            return;
        if(targetNode.walkable == false){

        }
    
        Heap<TileData> openSet = new Heap<TileData>(StaticClass.tileCount);
        HashSet<TileData> closedSet = new HashSet<TileData>();
        openSet.Add(startNode);
    
        while (openSet.Count > 0)
        {
            TileData currentNode = openSet.RemoveFirst();
            
            closedSet.Add(currentNode);
    
            if (currentNode == targetNode)  //path found
            {
                //sw.Stop();
                //print("path found: " + sw.ElapsedMilliseconds + " ms");

                RetracePath(startNode, targetNode);
                return;
            }
    
            foreach (TileData neighbour in currentNode.myNeighbours) {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;
    
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parentNode = currentNode;
    
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }
    }
    
    List<TileData> RetracePath(TileData startNode, TileData targetNode)
    {
        //List<TileData> tempPath = new List<TileData>();
        TileData currentNode = targetNode;
    
        while(currentNode != startNode) {       // as of now it adds tiles that are unwalkable tiles too that are 1 tile close to a walkable tile
            path.Add(currentNode);
            //tempPath.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
    
        //tempPath.Reverse();
        //testing.path = tempPath;
        return path;
    }

    int GetDistance(TileData nodeA, TileData nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX- nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
    
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private void OnDrawGizmos() {
        if(path != null){
            foreach (TileData tempTileData in path)
            {
                Gizmos.color = (tempTileData.walkable)?Color.white:Color.red;
                
                Vector3 tempWorldPos = new Vector3(tempTileData.worldX, tempTileData.worldY, 0);
                Gizmos.DrawCube(tempWorldPos, Vector3.one * 0.35f);
            }
        }
    }*/
}
