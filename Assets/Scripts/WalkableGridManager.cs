using UnityEngine;

public class WalkableGridManager : MonoBehaviour
{
    public enum TileType
    {
        Dirt = 'D',
        Quicksand = 'Q',
        Brick = 'B',
        Steel = 'S',
        Water = 'W'
    }

    // Method to check if a tile is walkable
    public bool IsTileWalkable(char tileTypeChar)
    {
        return tileTypeChar != (char)TileType.Brick &&
               tileTypeChar != (char)TileType.Steel &&
               tileTypeChar != (char)TileType.Water; // Include water as unwalkable
    }
}
