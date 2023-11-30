using UnityEngine;

public class WalkableGridManager : MonoBehaviour
{
    private bool[,] walkableGrid;

    public enum TileType
    {
        Dirt = 'D',
        Quicksand = 'Q',
        Brick = 'B',
        Steel = 'S',
        Water = 'W'
    }

    // Method to initialize the walkable grid
    public void InitializeGrid(int width, int height)
    {
        walkableGrid = new bool[width, height];
    }

    // Method to update the walkable status of a tile
    public void SetTileWalkable(int x, int y, bool isWalkable)
    {
        if (x >= 0 && x < walkableGrid.GetLength(0) && y >= 0 && y < walkableGrid.GetLength(1))
        {
            walkableGrid[x, y] = isWalkable;
        }
    }

    // Method to check if a tile is walkable
    public bool IsTileWalkable(char tileTypeChar)
    {
        return tileTypeChar != (char)TileType.Brick &&
               tileTypeChar != (char)TileType.Steel &&
               tileTypeChar != (char)TileType.Water; // Include water as unwalkable
    }

    // Method to get the walkable grid
    public bool[,] GetWalkableGrid()
    {
        return walkableGrid;
    }

    // Optional: Gizmos for debugging the walkable grid
    void OnDrawGizmos()
    {
        if (walkableGrid == null)
            return;

        for (int x = 0; x < MapData.Instance.width; x++)
        {
            for (int y = 0; y < MapData.Instance.height; y++)
            {
                Vector3 pos = new Vector3(
                    x * MapData.Instance.tileSize / 100.0f, 
                    (MapData.Instance.height - y - 1) * MapData.Instance.tileSize / 100.0f, 
                    0) - MapData.Instance.mapCenter;

                // Choose color based on walkability and make it semi-transparent
                Color gizmoColor = walkableGrid[x, y] ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
                Gizmos.color = gizmoColor;

                // Draw a wireframe cube for better visibility over the map
                Vector3 cubeSize = new Vector3(MapData.Instance.tileSize / 100.0f, MapData.Instance.tileSize / 100.0f, 1f) * 0.9f;
                Gizmos.DrawWireCube(pos, cubeSize);
            }
        }
    }
}
