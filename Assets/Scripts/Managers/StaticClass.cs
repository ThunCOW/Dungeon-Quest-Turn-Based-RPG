using UnityEngine;

public static class StaticClass{
    
    // Static variables for GridManager
    public const int xMin = 0;
    public const int xMax = 1;
    public static int yMin = 2;
    public static int yMax = 3;
    public static int xSize = 4;
    public static int ySize = 5;
    
    public static int tileCount;
    public static float cellSize = 1.0f;

    // Static variables for Game State
    public static int gameTurn;
    public static bool OnUI = false;
    public static bool enemyTurn = false;

    public static Character inspectedCharacter;

    public static float MasterVolume;
    public static float BackgroundVolume;
    public static float SoundEffectVolume;
}