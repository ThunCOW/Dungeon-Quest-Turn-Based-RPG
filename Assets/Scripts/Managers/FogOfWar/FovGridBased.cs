using UnityEngine;
using System.Collections.Generic;

namespace TileFOV
{
    public class FovGridBased : MonoBehaviour
    {
        public int searchDistance;
        public int viewDistance;

        public GameObject fovTiles_Parent = null;
        [SerializeField] GameObject fov_tile_prefab = null;
        private GameObject[,] nodes;

        GridManager gridManager;
        private void Start()
        {
            // Needs to run before CharacterMovement Start func
            gridManager = StaticClass.gridManager;
            
            CreateFov();
        }

        private void CreateFov()
        {
            Vector3Int localPos = StaticClass.gridBase.WorldToCell(transform.position);
            Vinteger v = new Vinteger(localPos.x, localPos.y);

            nodes = new GameObject[50,50];

            //Vector3Int charPos = Vector3Int.FloorToInt(transform.position);
            GameObject go = (GameObject)Instantiate(fov_tile_prefab, Vector3.zero, Quaternion.Euler(0, 0, 0));
            go.transform.parent = fovTiles_Parent.transform;
            go.transform.localPosition = Vector3.zero;

            // setting it inside box detector creates error even if we call it on start/awake function when
            // there is a collision right after it is being created.
            go.GetComponent<BoxDetector>().characterDetection = GetComponent<CharacterDetection>();

            //nodes[charPos.x + viewDistance, charPos.y + viewDistance] = go;

            for(int octant = 0; octant < 8; octant++)
            {
                CreateFovTiles(v, octant);
            }
            viewDistanceCurrent = viewDistance;
            //fovTiles_Parent.transform.parent = null;
        }

        private int viewDistanceCurrent;
        public void Refresh(Vinteger pos)
        {
            // if character sight changed, destroy old fov tiles and create new ones
            if(viewDistanceCurrent != viewDistance)
            {
                foreach(Transform child in fovTiles_Parent.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                CreateFov();
            }

            for(int octant = 0; octant < 8; octant++)
            {
                RefreshOctant(pos, octant);
            }
            fovTiles_Parent.transform.position = transform.position;    // every time updated, change position back to character's position
            // during movement action, we set parent to null so boxes do not move/lerp, then change parent back to character
            fovTiles_Parent.transform.parent = transform;
        }

        private void RefreshOctant(Vinteger start, int octant, int maxRows = 999)
        {
            ShadowLine line = new ShadowLine();

            for(int row = 1; row < maxRows; row++)
            {
                Vinteger bounds = start.Add(TransformOctant(row, 0, octant));
                if(!InBoundsAndClose(bounds.x, bounds.y, start))
                {
                    break;
                }

                for(int col = 0; col <= row; col++)
                {
                    Vinteger pos = start.Add(TransformOctant(row, col, octant));
                    if(!InBoundsAndClose(pos.x, pos.y, start))
                    {
                        break;
                    }

                    float posDist = pos.Distance(start);
                    if(posDist <= viewDistance * StaticClass.cellSize)
                    {
                        Shadow projection = ProjectTile(row, col);

                        bool visible = !line.IsInShadow(projection);

                        TileData td = gridManager.GetTileDataByLocalPosition(new Vector3(pos.x, pos.y));

                        bool isWall = false;

                        if(td != null)
                        {
                            Vector2Int fov_tilePosition = new Vector2Int((pos.x - start.x), (pos.y - start.y));
                            Color color = Color.blue;
                            
                            if((td.tileType == TileType.unwalkable && !td.walkable) || (td.tileType == TileType.door && !td.doorOpen))
                            {
                                isWall = true;
                            }

                            if(!visible)
                            {
                                color = Color.red;
                                
                                GameObject fovTile = nodes[fov_tilePosition.x + viewDistance, fov_tilePosition.y + viewDistance];
                                if(fovTile.activeSelf)
                                {
                                    fovTile.GetComponent<BoxDetector>().DisableBox();
                                }
                            }
                            else if(visible)
                            {
                                //if(!isWall)
                                //{
                                GameObject fovTile = nodes[fov_tilePosition.x + viewDistance, fov_tilePosition.y + viewDistance];
                                    if(!fovTile.activeSelf)
                                    {
                                        fovTile.SetActive(true);
                                    }
                                /*}
                                else
                                {
                                    color = Color.red;

                                    if(fovTile.activeSelf)
                                    {
                                        fovTile.GetComponent<BoxDetector>().DisableBox();
                                    }
                                }*/

                                /*if(gameObject.CompareTag("Player"))
                                {
                                    Debug.Log(fovTile.transform.position);
                                    // Clear fog tile
                                    FogOfWarTilemapManager fogOfWarTilemapManager = FindObjectOfType<FogOfWarTilemapManager>().GetComponent<FogOfWarTilemapManager>();

                                    if(fogOfWarTilemapManager != null)
                                    {
                                        fogOfWarTilemapManager.ClearFog(fovTile.transform.position, transform.position);
                                    }
                                }*/
                            }

                            //Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);
                            //td.tilemap.SetColor(tilePos, color);
                        }
                        else
                        {
                            Vector2Int fov_tilePosition = new Vector2Int((pos.x - start.x), (pos.y - start.y));

                            Color color = Color.red;
                                
                            GameObject fovTile = nodes[fov_tilePosition.x + viewDistance, fov_tilePosition.y + viewDistance];
                            if(fovTile.activeSelf)
                            {
                                fovTile.GetComponent<BoxDetector>().DisableBox();
                            }

                            isWall = true;
                        }

                        if(visible && isWall)   // if it is visible but is a wall/door/etc
                        {
                            line.Add(projection);
                        }
                    }
                }
            }
        }

        private void CreateFovTiles(Vinteger start, int octant, int maxRows = 999)
        {
            for(int row = 1; row < maxRows; row++)
            {
                Vinteger bounds = start.Add(TransformOctant(row, 0, octant));
                if(!InBoundsAndClose(bounds.x, bounds.y, start))
                {
                    break;
                }
                
                for(int col = 0; col <= row; col++)
                {
                    Vinteger pos = start.Add(TransformOctant(row, col, octant));
                    if(!InBoundsAndClose(pos.x, pos.y, start))
                    {
                        break;
                    }

                    float posDist = pos.Distance(start);
                    if(posDist <= viewDistance * StaticClass.cellSize)
                    {
                        Vector2Int fov_tilePosition = new Vector2Int((start.x - pos.x), (start.y - pos.y));
                        if(nodes[fov_tilePosition.x + viewDistance, fov_tilePosition.y + viewDistance] == null)
                        {
                            GameObject go = (GameObject)Instantiate(fov_tile_prefab, Vector3.zero, Quaternion.Euler(0, 0, 0));
                            go.transform.parent = fovTiles_Parent.transform;
                            go.transform.localPosition = ((Vector3Int)fov_tilePosition);

                            // setting it inside box detector creates error even if we call it on start/awake function when
                            // there is a collision right after it is being created.
                            go.GetComponent<BoxDetector>().characterDetection = GetComponent<CharacterDetection>();

                            nodes[fov_tilePosition.x + viewDistance, fov_tilePosition.y + viewDistance] = go;
                        }
                    }
                }
            }
        }

        public void DisableColliders()
        {
            foreach(BoxCollider2D col in GetComponentsInChildren<BoxCollider2D>())
            {
                col.enabled = false;
            }
        }

        public void EnableColliders()
        {
            foreach(BoxCollider2D col in GetComponentsInChildren<BoxCollider2D>())
            {
                col.enabled = true;
            }
        }

        // start is the position our character is standing / will soon stand.
        public bool InBoundsAndClose(int x, int y, Vinteger start)
        {
            if(start == null) 
                start = new Vinteger(x, y);

            bool retVal = true;

            // Checks if position is within grid bounds
            if(x < gridManager.bounds[StaticClass.xMin] || y < gridManager.bounds[StaticClass.yMin] || x > gridManager.bounds[StaticClass.xMax] - 1 || y > gridManager.bounds[StaticClass.yMax] - 1)
                retVal = false;

            if(retVal)
            {
                // Check if tile is within search distance
                if(Mathf.Abs(x -  start.x) > searchDistance || Mathf.Abs(y - start.y) > searchDistance)
                {
                    retVal = false;
                }
            }

            return retVal;
        }

        Vinteger TransformOctant(int row, int col, int octant)
        {
            switch(octant)
            {
                case 0:
                    return new Vinteger(col, -row);
                case 1:
                    return new Vinteger(row, -col);
                case 2:
                    return new Vinteger(row, col);
                case 3:
                    return new Vinteger(col, row);
                case 4:
                    return new Vinteger(-col, row);
                case 5:
                    return new Vinteger(-row, col);
                case 6:
                    return new Vinteger(-row, -col);
                case 7:
                    return new Vinteger(-col, -row);
                default:
                    return new Vinteger(0,0);
            }
        }

        Shadow ProjectTile(int row, int col)
        {
            float topLeft = (float)col / (row + 2);
            float bottomRight = (float)(col + 1) / (row + 1);

            return new Shadow(topLeft, bottomRight, new Vinteger(col, row + 2), new Vinteger(col + 1, row + 1));
        }
    }
}
