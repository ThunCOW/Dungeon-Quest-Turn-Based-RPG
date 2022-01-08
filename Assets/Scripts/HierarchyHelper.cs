#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HierarchyHelper : MonoBehaviour
{
    public List<GameObject> childObjects;

    [Space]
    public bool setChild = false;

    void Update()
    {
        MakeChilds();
    }

    void MakeChilds()
    {
        if(setChild)
        {
            setChild = false;
            foreach(GameObject go in childObjects)
            {
                go.transform.parent = this.gameObject.transform;
            }
            childObjects.Clear();
        }
    }
}
#endif