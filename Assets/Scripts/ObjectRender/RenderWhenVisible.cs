using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Experimental.Rendering.Universal;

public class RenderWhenVisible : MonoBehaviour
{
    public bool getObjects;
    
    [Space]
    public bool disableObjects;

    [Space]
    public bool canPlayerSee = false;

    [Space]
    [SerializeField] List<GameObject> roomObjects = new List<GameObject>();

    [HideInInspector]
    public GetObjectsAll getObjectsAll;
    public void OnValidate()
    {
        if(getObjectsAll == null)
            getObjectsAll = FindObjectOfType<GetObjectsAll>();
        
        contactFilterForFOV = getObjectsAll.contactFilterForFOV;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            GetRoomObjects();
        }; 
        

        if(disableObjects)
        {
            bool isDirty = false;
            foreach(GameObject go in roomObjects)
            {
                if(go != null)
                    go.SetActive(false);
                else
                    isDirty = true;
            }
            if(isDirty)
            {
                for(int i = roomObjects.Count - 1; i > 0; i--)
                {
                    if(roomObjects[i].gameObject == null)
                        roomObjects.RemoveAt(i);
                }
            }
        }
        else
        {
            foreach(GameObject go in roomObjects)
            {
                if(go != null)
                    go.SetActive(true);
            }
        }
#endif
    }

    private void Start()
    {
        if(getObjectsAll == null)
            getObjectsAll = FindObjectOfType<GetObjectsAll>();
        
        contactFilterForFOV = getObjectsAll.contactFilterForFOV;
        
        PlayerMovement.current.onMovementStepComplete += DetectPlayerInside;
        DetectPlayerInside();

        foreach(GameObject go in roomObjects)
            go.SetActive(false);
    }

    private bool cameraRendering = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        //if(canPlayerSee)
        //{
            if(other.CompareTag("MainCamera"))
            {
                cameraRendering = true;
                /*foreach(GameObject go in roomObjects)
                {
                    if(go.GetComponentInChildren<Light2D>() != null)
                        continue;
                        
                    go.SetActive(true);
                }*/
            }
        //}
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("MainCamera"))
        {
            cameraRendering = false;
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
    
    [HideInInspector]
    [SerializeField] private ContactFilter2D contactFilterForFOV;
    private void DetectPlayerInside()
    {
        canPlayerSee = false;
        Collider2D selfCol = GetComponent<Collider2D>();

        List<Collider2D> collider2Ds = new List<Collider2D>();
        Physics2D.OverlapCollider(selfCol, contactFilterForFOV, collider2Ds);
        foreach(var col in collider2Ds)
        {
            if(!col || !col.CompareTag("Player"))
                continue;
                
            canPlayerSee = true;
            break;
        }

        if(!canPlayerSee)
        {
            foreach(GameObject go in roomObjects)
            {
                if(!go.activeSelf)
                    break;

                go.SetActive(false);
            }
        }
        else
        {
            if(cameraRendering)
            {
                foreach(GameObject go in roomObjects)
                {
                    if(go.GetComponentInChildren<Light2D>() != null)
                        continue;
                        
                    go.SetActive(true);
                }
            }
        }
    }

    #if (UNITY_EDITOR)
    private void GetRoomObjects()
    {
        if(getObjects)
        {
            getObjects = false;

            ClearRoom();

            // For all child under this transform ( child's child is not included )
            foreach(Transform child in getObjectsAll.gameObject.transform)
            {
                // Get circle collider 
                CircleCollider2D circleCol = GetComponent<CircleCollider2D>();
                // if doesn't have one create a one
                if(circleCol == null)
                {
                    circleCol = child.gameObject.AddComponent<CircleCollider2D>();
                    circleCol.radius = 0.22f;
                    
                    DetectObjectsInsideRoom(getObjectsAll.contactFilter2D);

                    DestroyImmediate(circleCol);
                }
                else if(circleCol != null)
                {
                    DetectObjectsInsideRoom(getObjectsAll.contactFilter2D);
                }
            }
        }
    }
    #endif
}