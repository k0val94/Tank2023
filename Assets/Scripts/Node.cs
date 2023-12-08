using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2 Position { get; private set; }
    public bool IsWalkable { get; private set; }

    // Kosten von Startknoten zu diesem Knoten
    public float GCost { get; set; }

    // Geschätzte Kosten vom aktuellen Knoten zum Zielknoten
    public float HCost { get; set; }

    // Gesamtkosten (GCost + HCost)
    public float FCost => GCost + HCost;

    // Zur Rückverfolgung des Pfades
    public Node Parent { get; set; }

    // Nachbarknoten
    public List<Node> Neighbors { get; set; }

    public Node(Vector2 position, bool isWalkable)
    {
        Position = position;
        IsWalkable = isWalkable;
        Neighbors = new List<Node>();
    }
}
