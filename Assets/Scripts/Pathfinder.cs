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
        foreach (var node in grid.Nodes)
        {
            node.GCost = Mathf.Infinity;
            node.HCost = GetHeuristicCost(node, end);
            node.Parent = null;
        }

        start.GCost = 0;
        var openSet = new List<Node> { start };

        while (openSet.Count > 0)
        {
            Node current = openSet.OrderBy(n => n.FCost).First();

            if (current == end)
            {
                return RetracePath(start, end);
            }

            openSet.Remove(current);

            foreach (var neighbor in current.Neighbors)
            {
                if (!neighbor.IsWalkable)
                {
                    continue;
                }

                float newMovementCostToNeighbor = current.GCost + GetDistance(current, neighbor);
                if (newMovementCostToNeighbor < neighbor.GCost)
                {
                    neighbor.GCost = newMovementCostToNeighbor;
                    neighbor.Parent = current;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

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
            currentNode = currentNode.Parent;
        }
        path.Add(startNode);
        path.Reverse();

        return path;
    }
}
