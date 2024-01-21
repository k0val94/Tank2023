using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapData : MonoBehaviour
{
    public static MapData Instance;
    public List<string[]> mapLayers;
    
    public float tileSize { get; private set; } = 64f;
    public int width { get; set; }
    public int height { get; set; }
    public Vector3 mapCenter { get; set; }
    private bool[,] walkableGrid;

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
            return walkableGrid[x, y];
        }
        return false;
    }

    public void SetTileWalkable(int x, int y, bool isWalkable)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            walkableGrid[x, y] = isWalkable;
        }
    }

    public bool[,] GetWalkableGrid()
    {
        return walkableGrid;
    }

    void OnDrawGizmos()
    {
        if (walkableGrid == null)
        {
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(
                    x * (tileSize / 100.0f), 
                    y * (tileSize / 100.0f), 
                    0) - mapCenter;

                // Set color based on walkability: Green for walkable, Red for non-walkable
                Color gizmoColor = walkableGrid[x, y] ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
                Gizmos.color = gizmoColor;

                Vector3 cubeSize = new Vector3(tileSize / 100.0f, tileSize / 100.0f, 1f);
                Gizmos.DrawWireCube(pos, cubeSize);

                #if UNITY_EDITOR
                Handles.Label(pos, $"({x},{y})");
                #endif
            }
        }
    }
}
