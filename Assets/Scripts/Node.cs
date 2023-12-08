using UnityEngine;
using System.Collections.Generic;

public class Node
{
    public Vector2 Position;
    public bool IsWalkable;
    public float GCost;
    public float HCost;
    public float FCost => GCost + HCost;
    public Node Parent;
    public List<Node> Neighbors;

    public Node(Vector2 position, bool isWalkable)
    {
        Position = position;
        IsWalkable = isWalkable;
        GCost = 0;
        HCost = 0;
        Parent = null;
        Neighbors = new List<Node>();
    }
}