using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFinding
{

    public void FindPath(Vector3 startPosition, Vector3 endPosition, List<TileData> path, int movementPoint = int.MaxValue)
    {
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        TileData startNode = GridManager.gridManager.GetTileDataByLocalPosition(startPosition);
        TileData targetNode = GridManager.gridManager.GetTileDataByLocalPosition(endPosition);
        if(targetNode == null)
            return;

        if(targetNode.walkable == false)
        {
            int walkableTiles = 4;
            foreach (TileData neighbour in targetNode.myFourNeighbours) 
            {
                if(!neighbour.walkable)
                    walkableTiles--;
            }
            if(walkableTiles == 0)
            {
                List<TileData> tempList;
                targetNode = FindClosestWalkable(startNode, targetNode, out tempList);
                path.AddRange(tempList);
                return;
            }
        }
    
        Heap<TileData> openSet = new Heap<TileData>(StaticClass.tileCount);     // holds walkable tiles
        HashSet<TileData> closedSet = new HashSet<TileData>();              // holds the tiles that are already been walked through
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            TileData currentNode = openSet.RemoveFirst();   
            
            closedSet.Add(currentNode);     // tile that we will move to
    
            if (currentNode == targetNode)  // if the next tile is the target, path is found, exit while loop
            {
                //sw.Stop();
                //print("path found: " + sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode, targetNode, path);
                return;
            }
    
            // if path is not found yet, select next tile we want to move
            foreach (TileData neighbour in currentNode.myFourNeighbours) 
            {
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                // Enters here ONLY when target is NOT a walkable tile BUT next to a walkable tile
                if(!neighbour.walkable && neighbour == targetNode)      // if target is not a walkable tile, and neighbour to a walkable tile
                {
                    neighbour.parentNode = currentNode;
                    targetNode = currentNode;                           // currentNode is the tile that has targetNode as neighbour
                    RetracePath(startNode, targetNode, path);
                    return;
                }
                
                if (!neighbour.walkable || closedSet.Contains(neighbour) || movementPoint <= 0) continue;     // if neighbour is not walkable or we already been on it, skip
                
                // If we can not move behind of an object to front of it, we have to go around it.
                if(currentNode.characterSortingOrder == EffectTiles.behindSortingOrder &&
                    (neighbour.characterSortingOrder == EffectTiles.frontSortingOrder ||
                    neighbour.characterSortingOrder == EffectTiles.frontOnlySortingOrder)) continue;
                // If we can not move from front of an object to behind of it, we have to go around it.
                if(currentNode.characterSortingOrder == EffectTiles.frontOnlySortingOrder &&
                    neighbour.characterSortingOrder == EffectTiles.behindSortingOrder) continue;

                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parentNode = currentNode;
                    //neighbour.gameObject.transform.localScale = new Vector3(4, 4, 1);
                    //neighbour.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);             // not only does this func add it to heap, also sorts it.
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
            movementPoint--;
        }
        // If we exit while loop to continue, that means we couldn't find a viable path

        return;
    }

    TileData FindClosestWalkable(TileData startNode, TileData targetNode, out List<TileData> bestPath){
        
        TileData closestTile = null;
        
        // Here we find closest walkable tile to character
        foreach(TileData walkable in targetNode.closestWalkable)
        {
            if(walkable != null)
            {   
                if(closestTile == null)
                {
                    closestTile = walkable;
                }
                else
                {
                    // We go through every walkable tile of target tile and choose the one that is closest to character
                    if(GetDistance(walkable, startNode) < GetDistance(closestTile, startNode))
                    {
                        closestTile = walkable;
                    }
                }
            }
        }
        
        // 
        List<TileData> walkableWithSameDistances = new List<TileData>();

        // Here we go through closest tiles of target tile again
        foreach(TileData walkable in targetNode.closestWalkable)
        {
            if(walkable != null)
            {
                // If two or more closest tiles of target tile are in same distance to target tile, closest one to character gets chosen.
                if(GetDistance(walkable, targetNode) == GetDistance(closestTile, targetNode))
                {
                    walkableWithSameDistances.Add(walkable);
                }
                // We check if closest tiles of target tile are closer to target tile than chosen closestTile, if yes we choose them over.
                if(GetDistance(walkable, targetNode) < GetDistance(closestTile, targetNode))
                {
                    closestTile = walkable;
                }
            }
        }

        // Here we calculate path length to chosen closestTile
        bestPath = new List<TileData>();

        Vector3 closestTilePos = new Vector3(closestTile.gridX, closestTile.gridY);
        Vector3 startPosition = new Vector3(startNode.gridX, startNode.gridY);
        FindPath(startPosition, closestTilePos, bestPath);

        // This time we will calculate path length for walkable tiles with same distances to targetNode and compare path length
        foreach (TileData walkable in walkableWithSameDistances)
        {
            Vector3 walkablePos = new Vector3(walkable.gridX, walkable.gridY);

            List<TileData> possiblePath = new List<TileData>();

            // TODO: might be bad for performance, we calculate distance between cclosest tiles by checking path distance of each
            // if removed, when there is a blocade to go around, our character dont know it.(just try it) 
            FindPath(startPosition, walkablePos, possiblePath);

            if(possiblePath.Count < bestPath.Count)
            {
                closestTile = walkable;
                bestPath = possiblePath;
            }
        }

        return closestTile;
    }
    
    // I can make a reverse search, start from targetNode and find a path to startNode including blockade tiles and -
    // select the closest walkable tile to targetNode as new targetNode and calculate a path from startNode to new targetNode -
    // to see if character can create a path to new target node so i can find a viable path later
    // if cant create a path -> create a reverse path -> traverse path until we find a blockade ->
    // -> after blockade is found, continue to search until we find a walkable tile -> 
    // -> if walkable tile found, try to create a path, if can't, use tile as new targetNode and repeat this formula.
    /*void ReverseSearchForClosestTile(TileData startNode, TileData targetNode, List<TileData> path)
    {
        Heap<TileData> openSet = new Heap<TileData>(StaticClass.tileCount);     // holds walkable tiles
        HashSet<TileData> closedSet = new HashSet<TileData>();              // holds the tiles that are already been walked through
        openSet.Add(startNode);

        bool tempBool = true;
        while (openSet.Count > 0 && tempBool)
        {
            TileData currentNode = openSet.RemoveFirst();
            
            closedSet.Add(currentNode);     // tile that we will move to
    
            if (currentNode == targetNode)  // if the next tile is the target, path is found, exit while loop
            {
                //sw.Stop();
                //print("path found: " + sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode, targetNode, path);
                return;
            }
    
            // if path is not found yet, select next tile we want to move
            foreach (TileData neighbour in currentNode.myFourNeighbours) 
            {
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                // Enters here ONLY when target is NOT a walkable tile BUT next to a walkable tile
                if(neighbour == targetNode)      // if target is not a walkable tile, and neighbour to a walkable tile
                {
                    neighbour.parentNode = currentNode;
                    targetNode = currentNode;                           // currentNode is the tile that has targetNode as neighbour
                    RetracePath(startNode, targetNode, path);
                    return;
                }
                
                if (!neighbour.walkable || closedSet.Contains(neighbour) || movementPoint <= 0) continue;     // if neighbour is not walkable or we already been on it, skip

                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parentNode = currentNode;
                    //neighbour.gameObject.transform.localScale = new Vector3(4, 4, 1);
                    //neighbour.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);             // not only does this func add it to heap, also sorts it.
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
            movementPoint--;
        }

        return;
    }*/

    // Tiles are connected to each other with parentNode, this method adds tiles to path list starting from endNode to startNode
    // through their parentNode variable.
    private void RetracePath(TileData startNode, TileData targetNode, List<TileData> path)      
    {
        TileData currentNode = targetNode;
    
        while(currentNode != startNode && !path.Contains(currentNode)) 
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();
    }

    private int GetDistance(TileData nodeA, TileData nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX- nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
    
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}