using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Door Tile")]
public class DoorTileObject : MainTileObject
{
    /// <summary>
    /// The door sound which will play when character opens a door.
    /// </summary>
    public AudioClip doorOpeningSound;
    
    [Range(1, 10)]
    public int doorOpeningVolumeMultiplier = 1;
    
    /// <summary>
    /// The door sound which will play when character closes a door.
    /// </summary>
    public AudioClip doorClosingSound;

    [Range(1, 10)]
    public int doorClosingVolumeMultiplier = 1;

    /// <summary>
    /// The tile which will shown when the door is open.
    /// </summary>
    public Tile doorOpenTile;
    
    /// <summary>
    /// The tile which will shown when the door is closed or locked.
    /// </summary>
    public Tile doorClosedTile;
}
