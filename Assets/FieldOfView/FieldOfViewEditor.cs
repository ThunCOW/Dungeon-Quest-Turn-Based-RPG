#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(FieldOfViewCircle))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfViewCircle fow = (FieldOfViewCircle)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.up, 360, fow.viewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle/2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle/2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        foreach(Transform visibleTargets in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTargets.position);
        }
    }
}
#endif