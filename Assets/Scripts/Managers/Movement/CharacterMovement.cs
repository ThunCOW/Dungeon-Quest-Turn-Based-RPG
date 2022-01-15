using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using TileFOV;

public abstract class CharacterMovement : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    

    protected SortingGroup sortingGroup;
    protected Vector3 currentPos, newPos;   // sending it to GridManager to block the new tile we are standing on, and unblocking the old tile 
    
    protected List<Vector3> walkableTilesPositions = null; // right click

    //public int movementPoint = 3;
    protected Transform seekerTransform;
    [Space]
    public Transform targetTransform;
    protected Vector3 targetLocalPos;

    protected PathFinding pathFinding;

    [SerializeField] protected List<TileData> path = new List<TileData>();
    
    protected FovGridBased fov;

    protected FogOfWarTilemapManager fogOfWarTilemapManager;
    protected virtual void OnValidate()
    {
        if(fogOfWarTilemapManager == null)
            fogOfWarTilemapManager = FindObjectOfType<FogOfWarTilemapManager>();
    }

    protected virtual void Awake()
    {
        
    }
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Get References
        fogOfWarTilemapManager = FindObjectOfType<FogOfWarTilemapManager>();
        sortingGroup = GetComponent<SortingGroup>();
        //CalculateWalkableTiles();
        pathFinding = new PathFinding();

        fov = GetComponent<FovGridBased>();
        Vector3Int localPos = GridManager.gridBase.WorldToCell(transform.position);
        Vinteger v = new Vinteger(localPos.x, localPos.y);
        fov.Refresh(v);

        // dumb variables
        seekerTransform = transform;
        currentPos = transform.position;
        newPos = transform.position;

        //updates
        GridManager.gridManager.BlockTile(transform.position);
    }

    
    private bool isLastTile;
    public float ViggleDegree;
    private float ViggleDegreePerMove;

    protected bool isLerping;
    bool initLerp;
    float speedActual;
    public float timeToMoveOneTile;
    float t;
    public float curveScale = .07f;
    public AnimationCurve moveCurve;

    private Vector3Int targetPos;
    private void Update() 
    {
        if(isLerping)
        {
            LerpToPos(transform.position, targetPos);
        }
    }

    public abstract void MovementAction(float movementPoint);

    protected void MoveOneTile(Vector3Int targetPos, bool lastTile)
    {
        if(path.Count > 0 && path[0].walkable)
        {
            this.isLastTile = lastTile;
            this.targetPos = targetPos; // we either do this or set target position from inherited class, which is a nuisaince since easily forgettable

            if(path[0] is DoorTile)
            {
                DoorTile doorTile = path[0] as DoorTile;
                if(path.Count == 1)
                {
                    //print("door clicked");
                    // player clicked over the door, does player want to move on door tile or want to close the door?
                    // player can't close door if standing on it
                    // player can close door if next to it
                    // game should ask if player want to close door when next to it, or want to move over door
                }
                if(doorTile.doorLocked == false && doorTile.doorOpen == false)    // if door is NOT open and NOT locked
                {
                    DoorTileObject tempObj = doorTile.tileObject as DoorTileObject;
                    audioSource.PlayOneShot(tempObj.doorOpeningSound, tempObj.doorOpeningVolumeMultiplier);
                    
                    doorTile.tilemap.SetTile(new Vector3Int(doorTile.gridX, doorTile.gridY, 0), tempObj.doorOpenTile);
                    doorTile.doorOpen = true;
                }
                else    // if door IS open
                {
                    newPos = targetPos;
                    //path.RemoveAt(0);
                    
                    isLerping = true;
                    fov.DisableColliders();
                    //transform.position = newPos;

                    // increase turn after our 1 turn of action is over
                    GridManager.gridManager.UnBlockTile(currentPos);
                    GridManager.gridManager.BlockTile(newPos);
                    currentPos = newPos;
                    StaticClass.gameTurn++;
                }
            }
            else
            {
                newPos = targetPos;

                TileData currentTD = GridManager.gridManager.GetTileDataByLocalPosition(currentPos);
                TileData nextTD = GridManager.gridManager.GetTileDataByLocalPosition(newPos);
                if(currentTD.instantSortingOrderTransitionBool)
                {
                    sortingGroup.sortingOrder = nextTD.characterSortingOrder;
                }
                //path.RemoveAt(0);
                
                isLerping = true;
                fov.DisableColliders();
                //transform.position = newPos;

                // increase turn after our 1 turn of action is over
                GridManager.gridManager.UnBlockTile(currentPos);
                GridManager.gridManager.BlockTile(newPos);
                currentPos = newPos;
                StaticClass.gameTurn++;
            }
        }
    }

    public void ClearPath()
    {
        path.Clear();
    }

    /// <summary>
    /// A bool to check if player has taken half of the way from one tile to another
    /// </summary>
    private bool halfTimePassed = false;

    protected void LerpToPos(Vector3 startPos, Vector3Int targetPos)
    {
        if(!initLerp)
        {
            fov.fovTiles_Parent.transform.parent = null; // we set it to null so boxes do not move with character

            Vector3 scale = Vector3.one;
            if(startPos.x > targetPos.x)
                scale.x = -1;
            else
                scale.x = 1;
            //transform.localScale = scale;
            
            t = 0;
            
            //float dist = Vector3.Distance(startPos, targetPos);
            // x = v / t;
            // x = 1
            // v / t = 1;
            // v ? , t we will set
            // t = 1 v = 1 | t = 2 v = 0.5 | t = 10 v = 0.1 | t = 0.1 v = 10 | t = 0.2 v = 5
            // v = (10 / t) / 10
            speedActual = (10 / timeToMoveOneTile) / 10; // / dist


            initLerp = true;
        }

        t += Time.deltaTime * speedActual;
        if(t >= 0.5f && !halfTimePassed)   // half the time passed
        {
            halfTimePassed = true;
            TileData td = GridManager.gridManager.GetTileDataByLocalPosition(targetPos);
            sortingGroup.sortingOrder = td.characterSortingOrder;
        }
        if(t >= 1)                          // character landed on a tile
        {
            halfTimePassed = false;

            t = 1;
            isLerping = false;
            initLerp = false;
            ViggleDegree = -ViggleDegree;
            ViggleDegreePerMove = ViggleDegree - transform.rotation.z;
            
            Vinteger v = new Vinteger(targetPos.x, targetPos.y);
            fov.Refresh(v);

            audioSource.PlayOneShot(path[0].tileObject.tileSteppingSound, path[0].tileObject.footstepVolumeMultiplier);

            path.RemoveAt(0);
        }

        float y = moveCurve.Evaluate(t);
        y *= curveScale;

        Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
        tp.y += y;
        transform.position = tp;

        if(isLastTile)
        {
            ViggleDegree = Mathf.Abs(ViggleDegree);
            ViggleDegreePerMove = ViggleDegree;
            transform.GetChild(0).eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.GetChild(0).eulerAngles = new Vector3(0, 0, ViggleDegreePerMove * t);
        }
    }

    protected void CalculateWalkableTiles(int movementPoint){
        for(int x = 0; x <= movementPoint; x++)
        {
            for(int y = (movementPoint - x); y >= 0; y--)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }
                walkableTilesPositions.Add(new Vector3(transform.position.x + x, transform.position.y + y, 0));
                if(y != 0)
                {
                    walkableTilesPositions.Add(new Vector3(transform.position.x + x, transform.position.y - y, 0));
                }

                if(x != 0)
                {
                    walkableTilesPositions.Add(new Vector3(transform.position.x - x, transform.position.y + y, 0));
                    if(y != 0)
                    {
                        walkableTilesPositions.Add(new Vector3(transform.position.x - x, transform.position.y - y, 0));
                    }
                }
            }
        }
    }

    protected virtual void OnDrawGizmos() 
    {
        if(path != null){
            foreach (TileData tempTileData in path)
            {
                Gizmos.color = (tempTileData.walkable)?Color.white:Color.red;
                
                Vector3 tempWorldPos = new Vector3(tempTileData.worldX  + StaticClass.cellSize / 2, tempTileData.worldY  + StaticClass.cellSize / 2, 0);
                Gizmos.DrawCube(tempWorldPos, Vector3.one * 0.35f);
            }
        }
    }
}
