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
            // Log current position and target position in world coordinates
            Debug.Log($"Enemy Position (World): {transform.position}, Target Position (World): {target.position}");

            Node enemyNode = GetNodeFromWorldPosition(transform.position);
            Node targetNode = GetNodeFromWorldPosition(target.position);

            // Überprüfen, ob die Knoten gültig sind, bevor Sie auf ihre Eigenschaften zugreifen
            if (enemyNode != null && targetNode != null)
            {
                Debug.Log($"Enemy Node Position (Grid): {enemyNode.Position}, Target Node Position (Grid): {targetNode.Position}");
            }
            else
            {
                Debug.LogError("Einer der Knoten ist null. Überprüfen Sie die GetNodeFromWorldPosition-Methode.");
                return; // Beenden Sie die Methode hier, um weitere Fehler zu verhindern
            }

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

    private Node GetNodeFromWorldPosition(Vector2 worldPosition)
    {
        // Transformiere Weltkoordinaten in lokale Gitterkoordinaten
        Vector2 gridPosition = (worldPosition - (Vector2)MapData.Instance.mapCenter) / (MapData.Instance.tileSize / 100.0f);

        // Justiere Koordinaten, um vom unteren linken Eckpunkt zu starten
        int x = Mathf.FloorToInt(gridPosition.x + MapData.Instance.width / 2f);
        int y = Mathf.FloorToInt(gridPosition.y + MapData.Instance.height / 2f);
        Debug.Log($"Grid-Check: {grid != null}, Nodes-Check: {grid?.Nodes != null}, X: {x}, Y: {y}");

        // Überprüfe, ob die Koordinaten innerhalb der Gittergrenzen liegen
        if (x >= 0 && x < grid.Width && y >= 0 && y < grid.Height)
        {
            return grid.Nodes[x, y];
             Debug.LogError($"GetNodeFromWorldPosition: Grid coordinates out of bounds. X: {x}, Y: {y}, World Position: {worldPosition}");
        }
        else
        {
            return null; // Gib null zurück, wenn die Position außerhalb des Gitters liegt
        }
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

    void OnDrawGizmos()
    {
        if (grid == null || grid.Nodes == null)
        {
            return;
        }

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Vector3 pos = new Vector3(
                    x * (MapData.Instance.tileSize / 100.0f), 
                    y * (MapData.Instance.tileSize / 100.0f), 
                    0) - (Vector3)MapData.Instance.mapCenter;

                Color gizmoColor = new Color(0, 1, 0, 0.3f);
                Gizmos.color = gizmoColor;

                Vector3 cubeSize = new Vector3(MapData.Instance.tileSize / 100.0f, MapData.Instance.tileSize / 100.0f, 1f);
                Gizmos.DrawWireCube(pos, cubeSize);
            }
        }
    }

}
