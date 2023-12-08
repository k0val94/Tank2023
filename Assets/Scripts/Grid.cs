using UnityEngine;
using System.Collections.Generic;

public class Grid
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Node[,] Nodes { get; private set; }

    public Grid(int width, int height, bool[,] walkableMap)
    {
        Width = width;
        Height = height;
        Nodes = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x, y);
                Nodes[x, y] = new Node(position, walkableMap[x, y]);
            }
        }
    }

    public void InitializeNeighbors()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Node node = Nodes[x, y];
                node.Neighbors = GetNeighbors(x, y);
            }
        }
    }

    private List<Node> GetNeighbors(int x, int y)
    {
        List<Node> neighbors = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int checkX = x + dx;
                int checkY = y + dy;

                if (checkX >= 0 && checkX < Width && checkY >= 0 && checkY < Height)
                {
                    neighbors.Add(Nodes[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public Node GetNodeFromWorldPosition(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - MapData.Instance.mapCenter.x) / (MapData.Instance.tileSize / 100.0f)) + MapData.Instance.width;
        int y = Mathf.FloorToInt((worldPosition.y - MapData.Instance.mapCenter.y) / (MapData.Instance.tileSize / 100.0f)) + MapData.Instance.height;

        x = Mathf.Clamp(x, 0, Width - 1);
        y = Mathf.Clamp(y, 0, Height - 1);

        return Nodes[x, y];
    }
}
