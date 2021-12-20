using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVariationManager : MonoBehaviour
{
    Tilemap tilemap;

    public List<Tile> tileVariations = new List<Tile>();
    public bool randomizeTiles = false;
    private void OnValidate()
    {
        if(tilemap == null)
            tilemap = GetComponent<Tilemap>();

        if(randomizeTiles)
        {
            if(tileVariations.Count > 0)
            {
                for(int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
                {
                    for(int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
                    {
                        Vector3Int tilePos = new Vector3Int(x, y, 0);
                        if(tilemap.HasTile(tilePos))
                        {
                            int randomNumber = UnityEngine.Random.Range(0, tileVariations.Count - 1);
                            tilemap.SetTile(tilePos, tileVariations[randomNumber]);
                        }
                    }
                }
            }
            randomizeTiles = false;
        }
    }
}
