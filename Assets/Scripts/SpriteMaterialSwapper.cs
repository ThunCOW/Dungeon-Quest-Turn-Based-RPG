using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering;

public class SpriteMaterialSwapper : MonoBehaviour
{
    [SerializeField] Material spriteDefault;
    [SerializeField] Material spriteShader;

    public bool swapToDefault = false;
    public bool swapToShader = false;


    void OnValidate()
    {
        if(swapToDefault)
        {
            swapToDefault = false;
            if(spriteDefault != null)
            {
                GameObject[] _rootGameObjectsOfSpecificScene = SceneManager.GetSceneByName("Dungeon1Light").GetRootGameObjects();
                foreach(GameObject go in _rootGameObjectsOfSpecificScene)
                {
                    foreach(SpriteRenderer sr in go.GetComponentsInChildren<SpriteRenderer>(true))
                    {
                        if(sr != null)
                        {
                            sr.material = spriteDefault;
                        }
                    }
                    foreach(TilemapRenderer tr in go.GetComponentsInChildren<TilemapRenderer>(true))
                    {
                        if(tr != null)
                        {
                            tr.material = spriteDefault;
                        }
                    }

                    
                }
            }
        }
        if(swapToShader)
        {
            swapToShader = false;
            if(spriteShader != null)
            {
                GameObject[] _rootGameObjectsOfSpecificScene = SceneManager.GetSceneByName("Dungeon1Light").GetRootGameObjects();
                foreach(GameObject go in _rootGameObjectsOfSpecificScene)
                {
                    foreach(SpriteRenderer sr in go.GetComponentsInChildren<SpriteRenderer>(true))
                    {
                        if(sr != null)
                        {
                            sr.material = spriteShader;
                        }
                    }
                    foreach(TilemapRenderer tr in go.GetComponentsInChildren<TilemapRenderer>(true))
                    {
                        if(tr != null)
                        {
                            tr.material = spriteShader;
                        }
                    }
                }
            }
        }
    }
}
