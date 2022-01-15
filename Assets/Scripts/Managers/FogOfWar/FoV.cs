using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace TileFOV
{
    // https://www.youtube.com/watch?v=r50mj8TUF_U&ab_channel=SharpAccent
    public class FoV : MonoBehaviour
    {
        public int searchDistance;
        public int shadowDistance;

        public List<Vector2Int> visibleTiles; 

        private void Start()
        {
        
        }

        public void Refresh(Vinteger pos)
        {
            for(int octant = 0; octant < 8; octant++)
            {
                RefreshOctant(pos, octant);
            }
        }

        public void RefreshOctant(Vinteger start, int octant, int maxRows = 999)
        {
            ShadowLine line = new ShadowLine();

            bool fullShadow = false;

            for(int row = 1; row < maxRows; row++)
            {
                Vinteger bounds = start.Add(TransformOctant(row, 0, octant));
                if(!InBoundsAndClose(bounds.x, bounds.y))
                {
                    break;
                }

                for(int col = 0; col <= row; col++)
                {
                    Vinteger pos = start.Add(TransformOctant(row, col, octant));
                    if(!InBoundsAndClose(pos.x, pos.y))
                    {
                        break;
                    }

                    if(fullShadow)
                    {
                        // TODO: 28.36
                        TileData td = GridManager.gridManager.GetTileDataByLocalPosition(new Vector3(pos.x, pos.y, 0));
                        if(td != null)
                        {
                            Vector3Int tempVec = new Vector3Int(pos.x, pos.y, 0);
                            td.tilemap.SetColor(tempVec, Color.gray);
                            /*if(td.visibility == 0)
                            {
                                Vector3Int tempVec = new Vector3Int(pos.x, pos.y, 0);
                                td.tilemap.SetColor(tempVec, Color.black);
                                // make it black 
                            }
                            else
                            {
                                Vector3Int tempVec = new Vector3Int(pos.x, pos.y, 0);
                                td.tilemap.SetColor(tempVec, Color.gray);
                                // make it gray
                            }*/
                        }
                    }
                    else
                    {
                        Shadow projection = ProjectTile(row, col);

                        bool visible = !line.IsInShadow(projection);

                        // TODO: 31.22
                        TileData td = GridManager.gridManager.GetTileDataByLocalPosition(new Vector3(pos.x, pos.y));

                        bool isWall = false;

                        if(td != null)
                        {
                            DoorTile doorTile = td as DoorTile;
                            if(!td.walkable || (td is DoorTile && !doorTile.doorOpen))
                            {
                                isWall = true;
                            }

                            Color color = Color.blue;
                            if(!visible)
                            {
                                color = Color.red;
                                /*if(td.visibility == 0)
                                {
                                    color = Color.black;
                                }
                                else
                                {
                                    color = Color.gray;
                                }*/
                            }
                            else
                            {
                                //td.visibility = 1;
                                Vector2Int tempVec2 = new Vector2Int(pos.x, pos.y);
                                if(!visibleTiles.Contains(tempVec2))
                                    visibleTiles.Add(tempVec2);
                            }

                            // Here all the tiles within search distance are colored, so it makes a square,
                            // go to row 154, where tiles outside of search distance are colored back to gray (delete comment slashes to see)
                            Vector3Int tempVec = new Vector3Int(pos.x, pos.y, 0);
                            td.tilemap.SetColor(tempVec, color);
                        }
                        else
                        {
                            isWall = true;
                        }

                        if(visible && isWall)   // if it is visible but is a wall/blockade
                        {
                            line.Add(projection);
                            //fullShadow = line.IsFullShadow();
                        }
                    }

                    float posDist = pos.Distance(start);
                    if(posDist > shadowDistance * StaticClass.cellSize)
                    {
                        TileData td = GridManager.gridManager.GetTileDataByLocalPosition(new Vector3(pos.x, pos.y));
                        if(td != null)
                        {
                            // We can use this to color tiles that are not within view distance
                            //                           + x tile
                            if(posDist < (shadowDistance + 0) * StaticClass.cellSize)
                            {
                                Debug.Log("Shouldn't enter when 0");
                                Vector3Int tempVec = new Vector3Int(pos.x, pos.y, 0);
                                td.tilemap.SetColor(tempVec, Color.red);
                                //if(td.visibility == 0)
                                //{
                                //    Vector3Int tempVec = new Vector3Int(pos.x, pos.y, 0);
                                //    td.tilemap.SetColor(tempVec, Color.gray);
                                //}
                                //else
                                //{
                                //    Vector3Int tempVec = new Vector3Int(pos.x, pos.y, 0);
                                //    td.tilemap.SetColor(tempVec, Color.black);
                                //}
                            }
                            else    // tiles outside of view distance
                            {
                                Vector3Int tempVec3 = new Vector3Int(pos.x, pos.y, 0);
                                td.tilemap.SetColor(tempVec3, Color.gray);
                                Vector2Int tempVec2 = new Vector2Int(pos.x, pos.y);
                                visibleTiles.Remove(tempVec2);
                            }
                        }
                    }
                }
            }
            //return line.shadows;
        }

        public bool InBoundsAndClose(int x, int y)
        {
            bool retVal = true;

            // Checks if position is within grid bounds
            if(x < GridManager.bounds[StaticClass.xMin] || y < GridManager.bounds[StaticClass.yMin] || x > GridManager.bounds[StaticClass.xMax] - 1 || y > GridManager.bounds[StaticClass.yMax] - 1)
                retVal = false;

            if(retVal)
            {
                // TODO: 24.00
                // Check if tile is within search distance
                if(Mathf.Abs(x -  transform.position.x) > searchDistance || Mathf.Abs(y - transform.position.y) > searchDistance)
                {
                    //TileData td = gridManager.GetTileDataByLocalPosition(new Vector3(x, y, 0));
                    //Vector3Int tempVec = new Vector3Int(x, y, 0);
                    //td.tilemap.SetColor(tempVec, Color.red);
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

    public class ShadowLine
    {
        public List<Shadow> shadows = new List<Shadow>();

        public bool IsInShadow(Shadow projection)
        {
            foreach (Shadow shadow in shadows)
            {
                if(shadow.Contains(projection))
                {
                    return true;    // if in shadow return true
                }
            }
            return false;   // otherwise return false
        }

        public void Add(Shadow sh)
        {
            int index = 0;
            for (; index < shadows.Count; index++)
            {
                if(shadows[index].start >= sh.start)
                    break;
            }

            Shadow overlapping_previous = null;
            if(index > 0 && shadows[index - 1].end > sh.start)
            {
                overlapping_previous = shadows[index - 1];
            }

            Shadow overlapping_next = null;
            if(index < shadows.Count && shadows[index].start < sh.end)
            {
                overlapping_next = shadows[index];
            }

            if(overlapping_next != null)
            {
                if(overlapping_previous != null)
                {
                    overlapping_previous.end = overlapping_next.end;
                    overlapping_previous.endPos = overlapping_next.endPos;
                    shadows.RemoveAt(index);
                }
                else
                {
                    overlapping_next.start = sh.start;
                    overlapping_next.startPos = sh.startPos;
                }
            }
            else
            {
                if(overlapping_previous != null)
                {
                    overlapping_previous.end = sh.end;
                    overlapping_previous.endPos = sh.endPos;
                }
                else
                {
                    shadows.Insert(index, sh);
                }
            }
        }

        public bool IsFullShadow()
        {
            return shadows.Count == 1 && shadows[0].start == 0 && shadows[0].end == 1;
        }
    }

    public class Shadow
    {
        public float start;
        public float end;
        public Vinteger startPos;
        public Vinteger endPos;

        public Shadow(float shadowStart, float shadowEnd, Vinteger startPosition, Vinteger endPosition)
        {
            start = shadowStart;
            end = shadowEnd;
            startPos = startPosition;
            endPos = endPosition;
        }

        public bool Contains(Shadow other)
        {
            return start <= other.start && end >= other.end;
        }
    }

    public class Vinteger
    {
        public int x;
        public int y;

        public Vinteger(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vinteger Add(Vinteger other)
        {
            return new Vinteger(x + other.x, y + other.y);
        }

        public float Distance(Vinteger other)
        {
            return Mathf.Sqrt(Mathf.Pow(this.x - other.x, 2) + Mathf.Pow(this.y - other.y, 2));
        }
    }
}