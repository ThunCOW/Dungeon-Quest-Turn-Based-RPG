using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateSoftShadowCornersForTiles : MonoBehaviour
{
    Tilemap tilemap;

    public GameObject shadowSpritePrefab;
    private GameObject shadowCornerParent;
    public bool createShadow = false;
    public bool removeShadow = false;
    private void OnValidate()
    {
        if(tilemap == null)
            tilemap = GetComponent<Tilemap>();

        if(createShadow)
        {
            if(shadowSpritePrefab != null)
            {
                shadowCornerParent = new GameObject();
                shadowCornerParent.name = "Shadow Corners Parent";
                shadowCornerParent.transform.parent = transform.parent.gameObject.transform;
                for(int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
                {
                    for(int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
                    {
                        Vector3Int tilePos = new Vector3Int(x, y, 0);
                        if(tilemap.HasTile(tilePos))    // Check if there is a tile on current position
                        {
                            if(!tilemap.HasTile(tilePos + Vector3Int.right)) // If there is no tile on its right
                            {
                                GameObject go = Instantiate(shadowSpritePrefab, new Vector3(x + 1,y,0), Quaternion.Euler(new Vector3(0, 0, 90)));
                                go.transform.parent = shadowCornerParent.transform;
                            }
                            if(!tilemap.HasTile(tilePos + Vector3Int.left)) // If there is no tile on its left
                            {
                                GameObject go = Instantiate(shadowSpritePrefab, new Vector3(x,y + 1,0), Quaternion.Euler(new Vector3(0, 0, 270)));
                                go.transform.parent = shadowCornerParent.transform;
                            }
                            if(!tilemap.HasTile(tilePos + Vector3Int.up)) // If there is no tile on its up
                            {
                                GameObject go = Instantiate(shadowSpritePrefab, new Vector3(x,y + 1,0), Quaternion.identity);
                                go.transform.parent = shadowCornerParent.transform;
                            }
                            if(!tilemap.HasTile(tilePos + Vector3Int.down)) // If there is no tile on its down
                            {
                                GameObject go = Instantiate(shadowSpritePrefab, new Vector3(x + 1,y,0), Quaternion.Euler(new Vector3(0, 0, 180)));
                                go.transform.parent = shadowCornerParent.transform;
                            }
                        }
                    }
                }
            }
            createShadow = false;
        }

        if(removeShadow)
        {
            if(shadowCornerParent != null)
                DestroyImmediate(shadowCornerParent);
            removeShadow = false;
        }
    }

    [ExecuteInEditMode]
    void Execute()
    {

    }
}
