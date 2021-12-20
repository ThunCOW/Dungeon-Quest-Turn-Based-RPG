using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class FieldOfViewCircle : MonoBehaviour
{
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstableMask;

    public float meshResolution;
    public int raycastCount;
    public int edgeResolveIteration;
    public float edgeDistanceThreshold;

    public List<Transform> visibleTargets = new List<Transform>();

    private Mesh mesh;
    
    // Start is called before the first frame update
    void Start()
    {
        //BoxCollider2D bo = GetComponent<BoxCollider2D>();
        //Vector2 v = bo.bounds.ClosestPoint(this.transform.position);
        StartCoroutine(FindTargetsWithDelay(0.2f));
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        //StartCoroutine(DrawFieldOfViewWithDelay(0.05f));
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear(); // this runs every frame?
        //Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        Collider2D[] targetsInViewRadius2D = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        for(int i = 0; i < targetsInViewRadius2D.Length; i++)
        {
            Transform target = targetsInViewRadius2D[i].transform;
            Vector3 dirToTarget = (new Vector3(target.position.x + 0.5f, target.position.y, target.position.z) - transform.position).normalized;
            if(Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics2D.Raycast(transform.position, dirToTarget, distanceToTarget, obstableMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    IEnumerator DrawFieldOfViewWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            DrawFieldOfView();
        }
    }

    void DrawFieldOfViewWithCornerDetection()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution); // raycast count
        raycastCount = 0;
        //raycastCount = stepCount + 1;
        float stepAngleSize = viewAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        for(int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if(i < vertexCount - 2) // so dont go out of index
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution); // raycast count
        raycastCount = 0;
        //raycastCount = stepCount + 1;
        float stepAngleSize = viewAngle / stepCount;
        
        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();
        for(int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            
            /*if(i > 0)
            {
                // We need to check if old raycast hit an obstacle but new one didn't, or new one did old one didn't,
                // then if true find an edge in between
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDistanceThreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector3.zero)
                        viewPoints.Add(edge.pointA);
                    if(edge.pointB != Vector3.zero)
                        viewPoints.Add(edge.pointB);
                }
            }*/

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if(i < vertexCount - 2) // so dont go out of index
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public Vector2[] GetColliderVertices(GameObject go)
    {
        Vector2[] vertices = new Vector2[4];
        BoxCollider2D b = go.GetComponent<BoxCollider2D>(); //retrieves the Box Collider of the GameObject called obj
        vertices[0] = (b.bounds.center + new Vector3(-b.size.x, -b.size.y) * 0.5f);
        vertices[1] = (b.bounds.center + new Vector3(-b.size.x, b.size.y) * 0.5f);
        vertices[2] = (b.bounds.center + new Vector3(b.size.x, b.size.y) * 0.5f);
        vertices[3] = (b.bounds.center + new Vector3(b.size.x, -b.size.y) * 0.5f);

        //TilemapCollider2D collider = go.GetComponent<TilemapCollider2D>();
        //float size = 1;
        //vertices[0] = (collider.bounds.center + new Vector3(-size, -size) * 0.5f);
        //vertices[1] = (collider.bounds.center + new Vector3(-size, size) * 0.5f);
        //vertices[2] = (collider.bounds.center + new Vector3(size, size) * 0.5f);
        //vertices[3] = (collider.bounds.center + new Vector3(size, -size) * 0.5f);
        return vertices;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, dir, viewRadius, obstableMask);
        raycastCount++;
        if(hit2D)
        {
            
            Vector2[] tempVec;
            tempVec = GetColliderVertices(hit2D.collider.gameObject);
            for(int i = 0; i < 4; i++)
            {
                //Debug.Log(tempVec[i]);
                Debug.DrawLine(transform.position, tempVec[i], Color.blue);
            }
            //UnityEngine.Debug.Break();
            
            return new ViewCastInfo(true, hit2D.point, hit2D.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;
        
        for(int i = 0; i < edgeResolveIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDistanceThreshold;
            if(newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
