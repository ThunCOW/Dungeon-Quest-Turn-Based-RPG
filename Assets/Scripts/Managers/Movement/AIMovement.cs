using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class AIMovement : CharacterMovement
{
    public List<Vector3> wanderingTiles = new List<Vector3>();

    protected override void Awake()
    {
        base.Awake();

        //isSelectedOnEditor = false;
    }

    //private bool test = true;
    public override void MovementAction(float movementPoint) 
    {
        WanderingMode();
    }

    private void EnemyAI()
    {
        /*if(StaticClass.enemyTurn)                                                                       // if enemy turn
        {
            if(test)                                                                                    // if enemy sees player
            {
                test = false;
                targetTransform = FindObjectOfType<PlayerMovement>().transform;                                          // Enemy targets a player character here
                pathFinding.FindPath(seekerTransform.position, targetTransform.position, path);           // Calculate a path to target
                StartCoroutine(PlayAction());
            }
            else
            {
                // do nothing for now
            }
        }*/
    }

    bool readyToCalculatePath = true;
    // AI character wanders around with no purpose
    protected virtual void WanderingMode()
    {
        if(readyToCalculatePath)
        {
            readyToCalculatePath = false;
            StartCoroutine(WanderingModeWithDelay());
        }
        // 1. Cast a circle, choose a random point, check if inside wandering path, if in move, choose a different point and repeat
        // 2. Randomly pick a movement point in wandering path.
        // 2.5 Pick a random point in wandering path, calculate distance, move if distance is lower than a certain movement point, move

        // 3. I want to have characters that patrol two point with time too.
    }

    protected virtual IEnumerator WanderingModeWithDelay()
    {
        if(path != null) path.Clear();
        
        while(path.Count == 0)   // Find a valid move-able path
        {
            int value;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomNumber = new byte[4];//4 for int32
                rng.GetBytes(randomNumber);
                value = BitConverter.ToInt32(randomNumber, 0);
            }

            value = Mathf.Abs(value) % (wanderingTiles.Count - 1);
            Vector2 nextDestination = wanderingTiles[(int)value];
            
            pathFinding.FindPath(seekerTransform.position, nextDestination, path);
        }
        
        float delay = UnityEngine.Random.Range(5, 25);
        
        yield return new WaitForSeconds(delay);
        
        StartCoroutine(PreMovement());
    }

    private IEnumerator PreMovement()
    {
        yield return new WaitForSeconds(timeToMoveOneTile);

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
            readyToCalculatePath = true;
        }
    }

    private IEnumerator PlayAction()
    {
        yield return new WaitForSeconds(timeToMoveOneTile);
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
            //StaticClass.enemyTurn = false; 
            readyToCalculatePath = true;
        }
        /*
        Vector3Int targetTilePos = StaticClass.gridBase.WorldToCell(new Vector3(path[0].worldX, path[0].worldY, 0));
        MoveOneTile(targetTilePos);

        if(path.Count > 0)
            StartCoroutine(PlayAction());
        else
        {
            StaticClass.enemyTurn = false;
            test = true;
        }*/
    }

#if UNITY_EDITOR
    // Gizmos for WanderingPath creator editor
    [HideInInspector] public bool canDrawGizmo = false;
    [HideInInspector] private bool _isSelectedOnEditor = false;
    [HideInInspector] public bool isSelectedOnEditor{
        get{return _isSelectedOnEditor;}
        set
        {
            _isSelectedOnEditor = value;
        }
    }
    [HideInInspector] public Color gizmosColor = Color.green;
    [ExecuteInEditMode]
    protected override void OnDrawGizmos() 
    {
        base.OnDrawGizmos();

        if(isSelectedOnEditor)
        {
            Gizmos.color = Color.cyan;

            Vector3 pos = transform.position + (Vector3.one * StaticClass.cellSize) / 2;
            Gizmos.DrawCube(pos, Vector3.one * 0.70f);
            
            if(wanderingTiles.Count > 0)
            {
                foreach (Vector3 tempPos in wanderingTiles)
                {
                    Gizmos.color = gizmosColor;

                    pos = tempPos + (Vector3.one * StaticClass.cellSize) / 2;

                    Gizmos.DrawCube(pos, Vector3.one * 0.35f);
                }
            }
        }

        if(canDrawGizmo)
        {
            if(wanderingTiles.Count > 0)
            {
                foreach (Vector3 tempPos in wanderingTiles)
                {
                    Gizmos.color = gizmosColor;

                    Vector3 pos = tempPos + (Vector3.one * StaticClass.cellSize) / 2;

                    Gizmos.DrawCube(pos, Vector3.one * 0.35f);
                }
            }
        }

    }

    private void OnApplicationQuit() 
    {
        isSelectedOnEditor = false;    
    }
#endif
}
