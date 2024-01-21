using UnityEngine;
using System.Collections.Generic;

public class MapData : MonoBehaviour
{
    public static MapData Instance;
    public List<string[]> mapLayers;
    
    public float tileSize { get; private set; } = 64f;
    public int width { get; set; }
    public int height { get; set; }
    public Vector3 mapCenter { get; set; }
    public bool[,] walkableGrid;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeWalkableGrid()
    {
        walkableGrid = new bool[width, height];
    }

    public bool IsTileWalkable(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            bool isWalkable = walkableGrid[x, y];
            if (!isWalkable)
            {
                Debug.Log($"First Unwalkable Tile Found at ({x}, {y})");
            }
            return isWalkable;
        }
        return false;
    }

    public void SetTileWalkable(int x, int y, bool isWalkable)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            // Die Koordinaten anpassen, um zum neuen Ursprung (0,0) zu gelangen
            int adjustedY = height - 1 - y;

            walkableGrid[x, adjustedY] = isWalkable;
        }
    }

    public bool[,] GetWalkableGrid()
    {
        return walkableGrid;
    }

}
