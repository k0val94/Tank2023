using UnityEngine;
using System.Collections.Generic;

public class EnemyTankAIController : MonoBehaviour
{
    private float rotateSpeed = 0.5f;
    private Transform target;
    private TankPhysicsController tankPhysicsController;
    private FieldOfNoise fieldOfNoise;
    private WalkableGridManager walkableGridManager;
    private List<Vector2> pathToFollow;
    private int currentPathIndex;

    public enum State
    {
        Idle,
        Following
    }

    public State currentState;

    private Grid grid;

    private void Awake()
    {
        fieldOfNoise = GetComponentInChildren<FieldOfNoise>();
        walkableGridManager = FindObjectOfType<WalkableGridManager>();

        if (walkableGridManager == null)
        {
            Debug.LogError("WalkableGridManager wurde nicht gefunden!");
            return;
        }

        if (fieldOfNoise == null)
        {
            Debug.LogError("FieldOfNoise component not found on the child objects!");
            return;
        }

        InitializeGrid();
    }

    private void Start()
    {
        tankPhysicsController = GetComponent<TankPhysicsController>();
        pathToFollow = new List<Vector2>();
    }

    private void Update()
    {
        target = fieldOfNoise.audibleTargets.Count > 0 ? fieldOfNoise.audibleTargets[0] : null;
        

        if (target != null)
        {
            currentState = State.Following;
            pathToFollow = FindPath(transform.position, target.position); // Implementieren Sie FindPath für den Dijkstra-Algorithmus
            currentPathIndex = 0;
        }
        else if (pathToFollow.Count == 0)
        {
            currentState = State.Idle;
        }

        switch (currentState)
        {
            case State.Following:
                if (currentPathIndex < pathToFollow.Count)
                {
                    Vector2 nextPoint = pathToFollow[currentPathIndex];
                    MoveTowardsPoint(nextPoint);
                    if (Vector2.Distance(transform.position, nextPoint) < 1.0f) // Prüfen, ob der Panzer nah am nächsten Punkt ist
                    {
                        currentPathIndex++;
                    }
                }
                else
                {
                    currentState = State.Idle; // Ende des Pfades erreicht
                }
                break;
            case State.Idle:
                // Optional: Logik für den Idle-Zustand
                break;
        }
    }

    private void InitializeGrid()
    {
        grid = new Grid();
        grid.Width = walkableGridManager.GetWalkableGrid().GetLength(0);
        grid.Height = walkableGridManager.GetWalkableGrid().GetLength(1);
        grid.Nodes = new Node[grid.Width, grid.Height];

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                grid.Nodes[x, y] = new Node
                {
                    Position = new Vector2(x, y),
                    Neighbors = GetNeighbors(x, y)
                };
            }
        }
    }

    private List<Node> GetNeighbors(int x, int y)
    {
        List<Node> neighbors = new List<Node>();

        // Beispiel für orthogonale Nachbarn
        int[] dx = { 1, 0, -1, 0 };
        int[] dy = { 0, 1, 0, -1 };
        
        for (int i = 0; i < 4; i++)
        {
            int newX = x + dx[i], newY = y + dy[i];
            if (newX >= 0 && newX < grid.Width && newY >= 0 && newY < grid.Height)
            {
                neighbors.Add(grid.Nodes[newX, newY]);
            }
        }

        return neighbors;
    }

    private List<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        // Convert start and goal to grid coordinates
        Node startNode = GetNodeFromWorldPosition(start);
        Node goalNode = GetNodeFromWorldPosition(goal);

        if (startNode == null || goalNode == null)
        {
            return new List<Vector2>();
        }

        // Initialize Dijkstra's algorithm
        var openSet = new List<Node>();
        var closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        startNode.Distance = 0;

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            foreach (Node node in openSet)
            {
                if (node.Distance < currentNode.Distance)
                    currentNode = node;
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == goalNode)
            {
                // Path has been found
                return RetracePath(startNode, goalNode);
            }

            foreach (Node neighbor in currentNode.Neighbors)
            {
                if (!closedSet.Contains(neighbor))
                {
                    float newDistanceToNeighbor = currentNode.Distance + Vector2.Distance(currentNode.Position, neighbor.Position);
                    if (newDistanceToNeighbor < neighbor.Distance)
                    {
                        neighbor.Distance = newDistanceToNeighbor;
                        neighbor.Previous = currentNode;
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
        }

        return new List<Vector2>();
    }

    private Node GetNodeFromWorldPosition(Vector2 position)
    {
        // Konvertierung der Weltkoordinaten in Gitterkoordinaten
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);

        // Überprüfen, ob die Koordinaten innerhalb des Gitters liegen
        if (x >= 0 && x < walkableGridManager.GetWalkableGrid().GetLength(0) &&
            y >= 0 && y < walkableGridManager.GetWalkableGrid().GetLength(1))
        {
            // Rückgabe des Knotens an den Gitterkoordinaten
            // Hier wird angenommen, dass Sie eine Methode oder ein Array haben, um die Node-Instanz zu erhalten
            return grid.Nodes[x, y];
        }

        return null; // Rückgabe von null, wenn die Position außerhalb des Gitters liegt
    }

    private List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Previous;
        }
        path.Reverse();
        return path;
    }

    private void MoveTowardsPoint(Vector2 point)
    {
        // Überprüfen, ob der Spieler innerhalb des Hörbereichs ist
        if (target != null && Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(target.position.x, target.position.y)) <= fieldOfNoise.hearingRadius)
        {
            // Berechnung der Richtung zum nächsten Punkt
            Vector2 directionToNextPoint = point - new Vector2(transform.position.x, transform.position.y);
            directionToNextPoint.Normalize();

            // Drehen des Panzers in Richtung des nächsten Punktes
            RotateTankTowardsDirection(directionToNextPoint);

            // Bewegung des Panzers vorwärts, wenn er ungefähr in Richtung des nächsten Punktes zeigt
            if (IsTankFacingDirection(directionToNextPoint))
            {
                tankPhysicsController.MoveTank(1.0f, 0); // Vorwärtsbewegung mit voller Geschwindigkeit
            }
        }
        else
        {
            // Stoppen der Bewegung, wenn der Spieler außerhalb des Hörbereichs ist
            currentState = State.Idle;
        }
    }

    private void RotateTankTowardsDirection(Vector2 direction)
    {
        float angleToTarget = Vector2.SignedAngle(transform.up, direction);
        if (Mathf.Abs(angleToTarget) > 1f) // Threshold to avoid jittering
        {
            float turnAmount = -Mathf.Sign(angleToTarget) * Mathf.Min(Mathf.Abs(angleToTarget) / 180, rotateSpeed);
            tankPhysicsController.MoveTank(0, turnAmount);
        }
    }

    private bool IsTankFacingDirection(Vector2 direction)
    {
        float angleToTarget = Vector2.SignedAngle(transform.up, direction);
        return Mathf.Abs(angleToTarget) < 10f; // Threshold angle to consider the tank as 'facing' the direction
    }

    public class Node
    {
        public Vector2 Position { get; set; }
        public float Distance { get; set; } = Mathf.Infinity;
        public Node Previous { get; set; }
        public List<Node> Neighbors { get; set; } = new List<Node>();
    }

    public class Grid
    {
        public Node[,] Nodes { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        // Initialization and setup methods for the grid will go here
    }
}
