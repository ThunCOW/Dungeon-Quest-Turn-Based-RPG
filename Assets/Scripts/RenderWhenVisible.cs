using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;

public class RenderWhenVisible : MonoBehaviour
{
    public bool disableObjects;

    [Space]
    [SerializeField] List<GameObject> roomObjects = new List<GameObject>();

    public void OnValidate()
    {
        if(disableObjects)
        {
            foreach(GameObject go in roomObjects)
                go.SetActive(false);
        }
        else
        {
            foreach(GameObject go in roomObjects)
                go.SetActive(true);
        }
    }

    private void Start()
    {
        foreach(GameObject go in roomObjects)
            go.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("MainCamera"))
        {
            foreach(GameObject go in roomObjects)
            {
                if(go.GetComponentInChildren<Light2D>() != null)
                    continue;
                    
                go.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("MainCamera"))
        {
            foreach(GameObject go in roomObjects)
                go.SetActive(false);
        }
    }

    public void DetectObjectsInsideRoom(ContactFilter2D contactFilter2D)
    {
        if(!disableObjects)
        {
            Collider2D selfCol = GetComponent<Collider2D>();

            List<Collider2D> collider2Ds = new List<Collider2D>();
            Physics2D.OverlapCollider(selfCol, contactFilter2D, collider2Ds);
            foreach(var col in collider2Ds)
            {
                if(!col)
                    continue;
                    
                if(!roomObjects.Contains(col.gameObject))
                    roomObjects.Add(col.gameObject);
            }
        }
    }

    public void ClearRoom()
    {
        if(!disableObjects)
            roomObjects.Clear();
    }
}