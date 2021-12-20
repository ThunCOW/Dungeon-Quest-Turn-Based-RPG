using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TileFOV;

public class PlayerMovement : CharacterMovement
{
    public GameObject playerCamera;

    protected override void Start()
    {
        base.Start();
        playerCamera.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -10);
    }

    public override void MovementAction(float movementPoint)
    {
        MouseInputs((int)movementPoint);
        //MobileInputs();
        if(Input.GetKeyDown(KeyCode.Space))
            UnityEngine.Debug.Break();
    }

    bool readyToCalculatePath = true;
    private void MouseInputs(int movementPoint)
    {
        /*if(Input.GetMouseButtonDown(1)) // right click
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            //print("pressed");
            if(path != null)
                path.Clear();

            if(walkableTilesPositions != null)
                walkableTilesPositions.Clear();
                
            CalculateWalkableTiles(movementPoint);
            for(int i = 0; i < walkableTilesPositions.Count; i++)
                pathFinding.FindPath(seekerTransform.position, walkableTilesPositions[i], path, movementPoint);
            
            //sw.Stop();
            //print("path found: " + sw.ElapsedMilliseconds + " ms");
        }*/
        //if(Input.GetMouseButton(0)){
            if(!MouseOverUILayerObject.IsPointerOverUIObject() && !StaticClass.OnUI)      // calculate a path if mouse is not over UI
            {
                if(readyToCalculatePath)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // mouse world position
                    Vector3Int localPos = StaticClass.gridBase.WorldToCell(mousePos);   // mouse world to tile local position
                    Vector3 worldPos = StaticClass.gridBase.CellToWorld(localPos);  // tile to world position

                    if(targetLocalPos != localPos)
                    {
                        //print(localPos);
                        targetLocalPos = localPos;
                        if(path != null) path.Clear();
                        targetTransform.position = new Vector3(worldPos.x + StaticClass.cellSize/2, worldPos.y + StaticClass.cellSize/2, 0);

                        pathFinding.FindPath(seekerTransform.position, targetTransform.position, path);
                    }
                }
            }
            else
            {
                path.Clear();
            }
        //}
        if(Input.GetMouseButtonDown(0) && !StaticClass.OnUI)
        {
            //print("pressed");
            if(path.Count > 0)
            {
                StartCoroutine(PreMovement());
                readyToCalculatePath = false;
            }
        }
    }

    private IEnumerator PreMovement()
    {
        yield return new WaitForSeconds(timeToMoveOneTile);

        Vector3Int targetTilePos = new Vector3Int((int)path[0].worldX, (int)path[0].worldY, 0);
        if(path.Count > 1)
        {
            MoveOneTile(targetTilePos, false);
        }
        else
        {
            MoveOneTile(targetTilePos, true);
        }

        StartCoroutine(PlayAction());
    }

    private IEnumerator PlayAction()
    {
        yield return new WaitForSeconds(timeToMoveOneTile);
        fogOfWarTilemapManager.ShadowTiles();
        fov.EnableColliders();
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();

        if(path.Count > 0)
        {
            Vector3Int targetTilePos = new Vector3Int((int)path[0].worldX, (int)path[0].worldY, 0);
        
            if(path.Count > 1)
            {
                MoveOneTile(targetTilePos, false);
            }
            else
            {
                MoveOneTile(targetTilePos, true);
            }

            StartCoroutine(PlayAction()); 
        }
        else
        {
            //yield return new WaitForSeconds(timeToMoveOneTile);
            StaticClass.enemyTurn = true; 
            readyToCalculatePath = true;
        }
        
        //playerCamera.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -10);
        
        // we calculate next visible tiles by sending our target position that we want to move, not current(transform.position) position
        //Vector3Int localPos = StaticClass.gridBase.WorldToCell(targetPos);
        //Vinteger v = new Vinteger(targetTilePos.x, targetTilePos.y);
        //fov.Refresh(v);   // when we do it here, we clear tiles before character moves, some tiles look wrong then
    }

    Touch touchToMove;
    private bool isTouchedToMove;
    void MobileInputs(){
        if(Input.touchCount > 0){
            foreach(Touch t in Input.touches)
            {
                if(!isTouchedToMove){
                    Vector2 tempVec = Camera.main.ScreenToWorldPoint(t.position);
                    isTouchedToMove = true;
                    touchToMove = t;
                }else if(t.fingerId == touchToMove.fingerId){
                    touchToMove = t;
                }
            }
        }
        if(isTouchedToMove)
        {
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touchToMove.position);
            Vector3Int localPos = StaticClass.gridBase.WorldToCell(touchPos);
            Vector3 worldPos = StaticClass.gridBase.CellToWorld(localPos);

            if(targetLocalPos != localPos)
            {
                //print(localPos);
                targetLocalPos = localPos;
                if(path != null) path.Clear();
                targetTransform.position = new Vector3(worldPos.x + StaticClass.cellSize/2, worldPos.y + StaticClass.cellSize/2, 0);
                pathFinding.FindPath(seekerTransform.position, targetTransform.position, path);
            }

            
            Vector3Int targetTilePos = StaticClass.gridBase.WorldToCell(new Vector3(path[0].worldX, path[0].worldY, 0));
            switch (touchToMove.phase)
            {
                //When a touch has first been detected, change the message and record the starting position
                case TouchPhase.Began:
                    
                    break;

                //Determine if the touch is a moving touch
                case TouchPhase.Moved:
                    
                    break;

                case TouchPhase.Stationary:
                    
                    break;

                case TouchPhase.Ended:
                    if(path.Count > 0)
                    {
                        StartCoroutine(PreMovement());
                        isTouchedToMove = false;
                        touchToMove.fingerId = -1;
                    }
                    /*MoveOneTile(targetTilePos);
                    isTouchedToMove = false;
                    touchToMove.fingerId = -1;*/
                    break;
            }
        }
    }
}

public static class MouseOverUILayerObject
{
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == 5) //5 = UI layer
            {
                return true;
            }
        }

        return false;
    }
}
