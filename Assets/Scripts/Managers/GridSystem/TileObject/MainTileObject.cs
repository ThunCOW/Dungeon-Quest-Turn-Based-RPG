using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/Main Tile")]
public class MainTileObject : ScriptableObject
{
    /// <summary>
    /// The footstep audio which will play when character moves a tile, 
    /// </summary>
    public AudioClip tileSteppingSound;

    [Range(1, 10)]
    public int footstepVolumeMultiplier = 1;
}
