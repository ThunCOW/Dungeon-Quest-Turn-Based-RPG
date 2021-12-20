using UnityEngine;

public static class StaticClass{
    
    // Static variables for GridManager
    public static int xMin = 0;
    public static int xMax = 1;
    public static int yMin = 2;
    public static int yMax = 3;
    public static int xSize = 4;
    public static int ySize = 5;
    
    public static int tileCount;
    public static float cellSize = 1.0f;

    public static GridManager gridManager = null;
    public static Grid gridBase = null;

    // Static variables for Game State
    public static int gameTurn;
    public static bool OnUI = false;
    public static bool enemyTurn = false;

    public static Character inspectedCharacter;
}

public enum TileType{
    walkable,
    unwalkable,
    door
}