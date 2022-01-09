using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWarTilemapManager : MonoBehaviour
{
    [SerializeField] private List<Tilemap> allTileMaps = null;

    private Tilemap fogTileMap;
    public TileBase whiteTile;
    
    // Start is called before the first frame update
    void Start()
    {
        fogTileMap = GetComponent<Tilemap>();
        
        shadowColor.a = 0.7f;

        CreateFog();
        ShadowTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateFog()
    {
        for (int x = GridManager.bounds[StaticClass.xMin]; x < GridManager.bounds[StaticClass.xMax]; x++)
        {
            for (int y = GridManager.bounds[StaticClass.yMin]; y < GridManager.bounds[StaticClass.yMax]; y++)
            {
                Vector3Int localPos = (new Vector3Int(x, y, (int)GridManager.gridBase.transform.position.z));    // local positions of tiles
                
                foreach (Tilemap tileMap in allTileMaps)   // circle through tilemaps in our scene
                {
                    if(tileMap.HasTile(localPos))   // if there is no tile, there is nothing to create or change
                    {
                        fogTileMap.SetTile(new Vector3Int(x, y, 0), whiteTile);
                        fogTileMap.SetColor(new Vector3Int(x, y, 0), Color.black);
                    }
                }
            }
        }
        fogTileMap.CompressBounds();
    }

    public bool night;
    public bool clearFog;
    public void ClearFog(Vector2 tilePos, Vector2 characterPos)
    {
        Color tempColor;
        if(night)
        {
            tempColor = Color.black;
            if(Vector2.Distance(tilePos, characterPos) <= 1)
            {
                tempColor.a = .0f;
            }
            else if(Vector2.Distance(tilePos, characterPos) > 1 && Vector2.Distance(tilePos, characterPos) <= 2)
            {
                tempColor.a = .10f;
            }
            else if(Vector2.Distance(tilePos, characterPos) > 2 && Vector2.Distance(tilePos, characterPos) <= 3)
            {
                tempColor.a = .22f;
            }
            else if(Vector2.Distance(tilePos, characterPos) > 3 && Vector2.Distance(tilePos, characterPos) <= 4)
            {
                tempColor.a = .34f;
            }
            else if(Vector2.Distance(tilePos, characterPos) > 4 && Vector2.Distance(tilePos, characterPos) <= 5)
            {
                tempColor.a = .46f;
            }
            else if(Vector2.Distance(tilePos, characterPos) > 5 && Vector2.Distance(tilePos, characterPos) <= 6)
            {
                tempColor.a = .58f;
            }
            else if(Vector2.Distance(tilePos, characterPos) > 6 && Vector3.Distance(tilePos, characterPos) <= 7)
            {
                tempColor.a = .70f;
            }
            else if(clearFog)
            {
                tempColor.a = .70f;
            }
        }
        else
        {
            tempColor = Color.clear;
        }

        Vector3Int tempPos = Vector3Int.FloorToInt(tilePos);
        fogTileMap.SetColor(tempPos, tempColor);
        fogTileMap.SetTile(tempPos + new Vector3Int(0, 0, 1), null);
    }

    public List<GameObject> playerCharactcers = new List<GameObject>();
    [SerializeField] private List<Vector3Int> visibleTiles = new List<Vector3Int>();
    Color shadowColor = Color.black;
    public void ShadowTiles()
    {
        List<Vector3Int> tempList = new List<Vector3Int>();
        for(int i = 0; i < playerCharactcers.Count; i++)
        {
            foreach(BoxCollider2D col in playerCharactcers[i].GetComponentsInChildren<BoxCollider2D>())
            {
                Vector3Int tempPos = Vector3Int.FloorToInt(col.transform.position);
                visibleTiles.Remove(tempPos);
                
                //if(tempPos == )
                tempList.Add(tempPos);
            }
        }

        // Shadow over visible tiles that are not visible after character movement
        foreach(Vector3Int vec in visibleTiles)
        {
            fogTileMap.SetTile(vec, whiteTile);
            if(clearFog)
            {
                fogTileMap.SetColor(vec, shadowColor);
            }
            else
            {
                fogTileMap.SetColor(vec, Color.black);
            }
        }
        SmoothCorners(visibleTiles);
        visibleTiles = tempList;

        SmoothCorners(visibleTiles);
    }

    [Header("SMOOTH CORNER TILES")]
    public Tile DOWN_1;
    public Tile LEFT_1;
    public Tile RIGHT_1;
    public Tile UP_1;
    public Tile DOWNLEFT_2;
    public Tile LEFTUP_2;
    public Tile RIGHTDOWN_2;
    public Tile UPRIGHT_2;
    public Tile EXDOWN_3;
    public Tile EXLEFT_3;
    public Tile EXRIGHT_3;
    public Tile EXUP_3;
    private void SmoothCorners(List<Vector3Int> visibleTiles)
    {
        StartCoroutine(SmoothCornersDelayed(visibleTiles));
    }

    private IEnumerator SmoothCornersDelayed(List<Vector3Int> visibleTiles)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        foreach(Vector3Int vec in visibleTiles)
        {
            bool down = false;
            bool left = false;
            bool up = false;
            bool right = false;

            // We check 4 neighbours of black tile is A black tile
            if(fogTileMap.GetColor(new Vector3Int(vec.x, vec.y - 1, 0)).a == 1.0f)  // Down
            {
                down = true;
            }
            if(fogTileMap.GetColor(new Vector3Int(vec.x - 1, vec.y, 0)).a == 1.0f)  // Left
            {
                left = true;
            }
            if(fogTileMap.GetColor(new Vector3Int(vec.x, vec.y + 1, 0)).a == 1.0f)  // Up
            {
                up = true;
            }
            if(fogTileMap.GetColor(new Vector3Int(vec.x + 1, vec.y, 0)).a == 1.0f)  // Right
            {
                right = true;
            }

            // Not close to a black tile
            if(down == false && left == false && up == false && right == false)
                continue;

            // If next to a black tile, we check for all 12 possibilities
            if(down == true && left == false && up == false && right == false)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), DOWN_1);
            }
            else if(down == false && left == true && up == false && right == false)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), LEFT_1);
            }
            else if(down == false && left == false && up == true && right == false)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), UP_1);
            }
            else if(down == false && left == false && up == false && right == true)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), RIGHT_1);
            }
            else if(down == true && left == true && up == false && right == false)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), DOWNLEFT_2);
            }
            else if(down == false && left == true && up == true && right == false)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), LEFTUP_2);
            }
            else if(down == false && left == false && up == true && right == true)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), UPRIGHT_2);
            }
            else if(down == true && left == false && up == false && right == true)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), RIGHTDOWN_2);
            }
            else if(down == false && left == true && up == true && right == true)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), EXDOWN_3);
            }
            else if(down == true && left == false && up == true && right == true)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), EXLEFT_3);
            }
            else if(down == true && left == true && up == false && right == true)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), EXUP_3);
            }
            else if(down == true && left == true && up == true && right == false)
            {
                fogTileMap.SetTile(vec + new Vector3Int(0, 0, 1), EXRIGHT_3);
            }

            fogTileMap.SetColor(vec + new Vector3Int(0, 0, 1), Color.black);
        }
    }
}
