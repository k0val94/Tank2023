using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
    private Grid grid;

    public Pathfinder(Grid grid)
    {
        this.grid = grid;
    }

    public List<Node> GetShortestPathAstar(Node start, Node end)
    {
        Debug.Log($"A*-Start: StartNode: {start.Position}, EndNode: {end.Position}");

        foreach (var node in grid.Nodes)
        {
            node.GCost = Mathf.Infinity;
            node.HCost = GetHeuristicCost(node, end);
            node.Parent = null;
        }

        start.GCost = 0;
        var openSet = new List<Node> { start };

        Debug.Log($"openSet: {openSet.Count}");

        while (openSet.Count > 0)
        {
            Node current = openSet.OrderBy(n => n.FCost).First();
            Debug.Log($"A*-Schleife: Aktueller Knoten: {current.Position}");

            if (current == end)
            {
                Debug.Log("A*-Ende: Ziel erreicht.");
                return RetracePath(start, end);
            }

            openSet.Remove(current);
            
            Debug.Log($"Number of neighbors for currentNode: {current.Neighbors.Count}");

            foreach (var neighbor in current.Neighbors)
            {
                Debug.Log($"IsWalkable: {neighbor.IsWalkable}");
                if (!neighbor.IsWalkable)
                {
                    continue;
                }

                float newMovementCostToNeighbor = current.GCost + GetDistance(current, neighbor);
                if (newMovementCostToNeighbor < neighbor.GCost)
                {
                    neighbor.GCost = newMovementCostToNeighbor;
                    neighbor.Parent = current;
                    Debug.Log($"A*-Schleife: Aktualisiere Nachbarn: {neighbor.Position}, Neuer G-Kostenwert: {neighbor.GCost}");

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                        Debug.Log($"A*-Schleife: Füge Nachbarn hinzu: {neighbor.Position}");
                    }
                }
            }
        }

        Debug.Log("A*-Ende: Kein Pfad gefunden.");
        return new List<Node>(); // Kein Pfad gefunden
    }

    private float GetHeuristicCost(Node node, Node target)
    {
        return Vector2.Distance(node.Position, target.Position);
    }

    private float GetDistance(Node nodeA, Node nodeB)
    {
        return Vector2.Distance(nodeA.Position, nodeB.Position);
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            Debug.Log($"A*-Retrace: Knoten im Pfad: {currentNode.Position}");
            currentNode = currentNode.Parent;
        }
        path.Add(startNode);
        path.Reverse();

        Debug.Log($"A*-Retrace: Gesamtlänge des Pfades: {path.Count}");
        return path;
    }
}
