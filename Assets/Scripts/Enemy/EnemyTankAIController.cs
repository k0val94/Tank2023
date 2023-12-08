using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        Debug.Log($"Map Center: {MapData.Instance.mapCenter}");
        Debug.Log($"Tile Size: {MapData.Instance.tileSize}");
        Debug.Log($"Grid Width: {grid.Width}");
        Debug.Log($"Grid Height: {grid.Height}");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // T-Taste für den Test
        {
                TestWorldPosition(new Vector2(-22.40f, 21.76f), new Vector2(0f, 69f)); // Oben links
                TestWorldPosition(new Vector2(21.76f, -22.40f), new Vector2(69f, 0f)); // Unten rechts
                TestWorldPosition(new Vector2(0.00f, 0.00f), new Vector2(35f, 35f));    // Zentrum
                TestWorldPosition(new Vector2(21.76f, 21.76f), new Vector2(69f, 69f));  // Oben rechts
                TestWorldPosition(new Vector2(-22.40f, -22.40f), new Vector2(0f, 0f));// Unten links
        }


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


    // Test mit verschiedenen Weltkoordinaten
    private void TestWorldPosition(Vector2 worldPosition, Vector2 expectedGridPosition)
    {
        // Berechnung der Gitterkoordinaten
        Node node = GetNodeFromWorldPosition(worldPosition);
        if (node != null)
        {
            Debug.Log($"Test-Weltkoordinate: {worldPosition}, Gitterkoordinate: {node.Position}, Erwartet: {expectedGridPosition}");
        }
        else
        {
            Debug.LogError($"Test-Weltkoordinate: {worldPosition}, Gitterkoordinate konnte nicht gefunden werden, Erwartet: {expectedGridPosition}");
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
    Node startNode = GetNodeFromWorldPosition(start);
    Node goalNode = GetNodeFromWorldPosition(goal);

    if (startNode == null || goalNode == null)
    {
        Debug.LogError("Start or goal node is null. Unable to find a path.");
        return new List<Vector2>();
    }

    List<Node> openSet = new List<Node>();
    HashSet<Node> closedSet = new HashSet<Node>();

    openSet.Add(startNode);

    while (openSet.Count > 0)
    {
        Node currentNode = openSet[0];
        for (int i = 1; i < openSet.Count; i++)
        {
            if (openSet[i].Distance < currentNode.Distance || 
                (openSet[i].Distance == currentNode.Distance && Random.value < 0.5f))
            {
                currentNode = openSet[i];
            }
        }

        openSet.Remove(currentNode);
        closedSet.Add(currentNode);

        if (currentNode == goalNode)
        {
            // Path found, retrace steps
            return RetracePath(startNode, goalNode);
        }

        foreach (Node neighbor in currentNode.Neighbors)
        {
            if (neighbor == null || !neighbor.Neighbors.Contains(currentNode) || closedSet.Contains(neighbor))
            {
                continue;
            }

            float newCostToNeighbor = currentNode.Distance + Vector2.Distance(currentNode.Position, neighbor.Position);
            if (newCostToNeighbor < neighbor.Distance || !openSet.Contains(neighbor))
            {
                neighbor.Distance = newCostToNeighbor;
                neighbor.Previous = currentNode;

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
            }
        }
    }

    Debug.LogError("No valid path found.");
    return new List<Vector2>();
}



    private float GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(Mathf.FloorToInt(nodeA.Position.x) - Mathf.FloorToInt(nodeB.Position.x));
        int dstY = Mathf.Abs(Mathf.FloorToInt(nodeA.Position.y) - Mathf.FloorToInt(nodeB.Position.y));

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


    private Node GetNodeFromWorldPosition(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - MapData.Instance.mapCenter.x) / (MapData.Instance.tileSize / 100.0f)) + MapData.Instance.width;
        int y = Mathf.FloorToInt((worldPosition.y - MapData.Instance.mapCenter.y) / (MapData.Instance.tileSize / 100.0f)) + MapData.Instance.height;

        // Stelle sicher, dass x und y innerhalb der Grenzen des Gitters liegen
        x = Mathf.Clamp(x, 0, MapData.Instance.width - 1);
        y = Mathf.Clamp(y, 0, MapData.Instance.height - 1);

        // Gib den entsprechenden Knoten zurück
        return grid.Nodes[x, y];
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

                Color gizmoColor = new Color(0, 1, 0, 0.3f); // Grün und halbtransparent
                Gizmos.color = gizmoColor;

                Vector3 cubeSize = new Vector3(MapData.Instance.tileSize / 100.0f, MapData.Instance.tileSize / 100.0f, 1f);
                Gizmos.DrawWireCube(pos, cubeSize);

                #if UNITY_EDITOR
                Handles.Label(pos, $"({x},{y})");
                #endif
            }
        }
    }

}
