using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private Grid grid;
    public List<Node> LastPath { get; private set; }

    public Pathfinder(Grid grid)
    {
        this.grid = grid;
    }

    public List<Node> GetShortestPathAstar(Node start, Node end)
    {
        var openSet = new PriorityQueue<Node, float>();
        var closedSet = new HashSet<Node>();

        foreach (var node in grid.Nodes)
        {
            node.GCost = Mathf.Infinity;
            node.HCost = GetHeuristicCost(node, end);
            node.Parent = null;
        }

        start.GCost = 0;
        openSet.Enqueue(start, start.FCost);

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();

            if (current == end)
            {
                LastPath = RetracePath(start, end);
                return LastPath;
            }

            closedSet.Add(current);

            foreach (var neighbor in current.Neighbors)
            {
                if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
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
                        openSet.Enqueue(neighbor, neighbor.FCost);
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

public class PriorityQueue<T, TPriority> where TPriority : IComparable<TPriority>
{
     private SortedDictionary<TPriority, Queue<T>> elements = new SortedDictionary<TPriority, Queue<T>>();

    public int Count { get; private set; } = 0;

    public void Enqueue(T item, TPriority priority)
    {
        if (!elements.ContainsKey(priority))
        {
            elements[priority] = new Queue<T>();
        }

        elements[priority].Enqueue(item);
        Count++;
    }

    public T Dequeue()
    {
        // Würde einen Fehler werfen, wenn die Warteschlange leer ist, also sicherstellen, dass sie nicht leer ist
        if (Count == 0)
            throw new InvalidOperationException("Die Warteschlange ist leer.");

        // Findet das erste Element mit der niedrigsten Priorität
        var pair = elements.GetEnumerator();
        pair.MoveNext();
        var items = pair.Current.Value;
        var item = items.Dequeue();
        if (items.Count == 0) // Wenn die Warteschlange für diese Priorität leer ist, entfernen Sie sie
        {
            elements.Remove(pair.Current.Key);
        }
        Count--;

        return item;
    }

    public bool Contains(T item)
    {
        foreach (var queue in elements.Values)
        {
            if (queue.Contains(item))
            {
                return true;
            }
        }
        return false;
    }
}
