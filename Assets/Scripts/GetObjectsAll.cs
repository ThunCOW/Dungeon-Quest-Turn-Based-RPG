using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using UnityEditor;


[ExecuteInEditMode]
public class GetObjectsAll : MonoBehaviour
{
    public bool Trigger = false;
    
    [Space]
    public bool disableAllObjects = false;

    [Space]
    public ContactFilter2D contactFilter2D;

    [Space]
    [SerializeField] GameObject roomParent = null;

    List<RenderWhenVisible> _rooms;

    private void OnValidate()
    {
        if(roomParent == null)
        {
            roomParent = FindObjectOfType<RenderWhenVisible>().transform.parent.gameObject;
        }

        
    }

    private void Update() 
    {

#if (UNITY_EDITOR)
        /*if(disableAllObjects)
        {
            int i = 0;
            foreach(RenderWhenVisible room in roomParent.GetComponentsInChildren<RenderWhenVisible>())
            {
                room.disableObjects = true;
                room.OnValidate();
                room.gameObject.name = "Room_" + i.ToString();
                i++;
            }
        }
        else
        {
            int i = 0;
            foreach(RenderWhenVisible room in roomParent.GetComponentsInChildren<RenderWhenVisible>())
            {
                room.disableObjects = false;
                room.OnValidate();
                room.gameObject.name = "Room_" + i.ToString();
                i++;
            }
        }*/
        
        if(Trigger)
        {
            Trigger = false;
            
            _rooms = new List<RenderWhenVisible>();
            int i = 0;
            foreach(RenderWhenVisible room in roomParent.GetComponentsInChildren<RenderWhenVisible>())
            {
                room.gameObject.name = "Room_" + i.ToString();
                i++;
                _rooms.Add(room);

                room.ClearRoom();
            }

            // For all child under this transform ( child's child is not included )
            foreach(Transform child in transform)
            {
                // Get circle collider 
                CircleCollider2D circleCol = GetComponent<CircleCollider2D>();
                // if doesn't have one create a one
                if(circleCol == null)
                {
                    circleCol = child.gameObject.AddComponent<CircleCollider2D>();
                    circleCol.radius = 0.22f;
                    
                    foreach(RenderWhenVisible room in _rooms)
                    {
                        room.DetectObjectsInsideRoom(contactFilter2D);
                    }

                    DestroyImmediate(circleCol);
                }
                else if(circleCol != null)
                {
                    foreach(RenderWhenVisible room in _rooms)
                    {
                        room.DetectObjectsInsideRoom(contactFilter2D);
                    }
                }
            }
        }
        /*if(Trigger)
        {
            Trigger = false;
            
            _rooms = new List<RenderWhenVisible>();
            foreach(RenderWhenVisible room in roomParent.GetComponentsInChildren<RenderWhenVisible>())
            {
                _rooms.Add(room);

                room.ClearRoom();
            }

            // For all child under this transform ( child's child is not included )
            foreach(Transform child in transform)
            {
                // Get circle collider 
                CircleCollider2D circleCol = GetComponent<CircleCollider2D>();
                // if doesn't have one create a one
                if(circleCol == null)
                {
                    circleCol = gameObject.AddComponent<CircleCollider2D>();
                    
                    foreach(RenderWhenVisible room in _rooms)
                    {
                        room.DetectObjectsInsideRoom(contactFilter2D);
                    }

                    
                }
                else if(circleCol != null)
                {
                    foreach(RenderWhenVisible room in _rooms)
                    {
                        room.DetectObjectsInsideRoom(contactFilter2D);
                    }
                }
            }
        }*/
#endif

    }

    /*IEnumerator PrintEachSecond(CircleCollider2D circleCol)
    {
        var waitForOneSecond = new EditorWaitForSeconds(1.0f);

        yield return waitForOneSecond;
        
        DestroyImmediate(circleCol);
    }*/
}
