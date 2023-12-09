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
    private Pathfinder pathfinder;
    private Grid grid;

    public enum State
    {
        Idle,
        Following
    }

    public State currentState;

    private void Awake()
    {
        fieldOfNoise = GetComponentInChildren<FieldOfNoise>();
        walkableGridManager = FindObjectOfType<WalkableGridManager>();

        if (walkableGridManager == null)
        {
            Debug.LogError("WalkableGridManager was not found!");
            return;
        }

        if (fieldOfNoise == null)
        {
            Debug.LogError("FieldOfNoise component not found on the child objects!");
            return;
        }

        grid = new Grid(walkableGridManager.GetWalkableGrid().GetLength(0), 
                        walkableGridManager.GetWalkableGrid().GetLength(1), 
                        walkableGridManager.GetWalkableGrid());
    }

    private void Start()
    {
        tankPhysicsController = GetComponent<TankPhysicsController>();
        pathToFollow = new List<Vector2>();
        pathfinder = new Pathfinder(grid);
        grid.InitializeNeighbors();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            TestWorldPosition(new Vector2(-22.40f, 21.76f), new Vector2(0f, 69f)); // Oben links
            TestWorldPosition(new Vector2(21.76f, -22.40f), new Vector2(69f, 0f)); // Unten rechts
            TestWorldPosition(new Vector2(0.00f, 0.00f), new Vector2(35f, 35f));    // Zentrum
            TestWorldPosition(new Vector2(21.76f, 21.76f), new Vector2(69f, 69f));  // Oben rechts
            TestWorldPosition(new Vector2(-22.40f, -22.40f), new Vector2(0f, 0f)); // Unten links
        }

        UpdateTarget();

        switch (currentState)
        {
            case State.Following:
                FollowPath();
                break;
            case State.Idle:
                // Optional: Logik für den Idle-Zustand
                break;
        }
    }
    
    private void TestWorldPosition(Vector2 worldPosition, Vector2 expectedGridPosition)
    {
        // Berechnung der Gitterkoordinaten
        Node node = grid.GetNodeFromWorldPosition(worldPosition);
        if (node != null)
        {
            Debug.Log($"Test-Weltkoordinate: {worldPosition}, Gitterkoordinate: {node.Position}, Erwartet: {expectedGridPosition}");
        }
        else
        {
            Debug.LogError($"Test-Weltkoordinate: {worldPosition}, Gitterkoordinate konnte nicht gefunden werden, Erwartet: {expectedGridPosition}");
        }
    }
   private void UpdateTarget()
    {

        if (fieldOfNoise.audibleTargets.Count > 0)
        {
            target = fieldOfNoise.audibleTargets[0];
            Node enemyNode = grid.GetNodeFromWorldPosition(transform.position);
            Node targetNode = grid.GetNodeFromWorldPosition(target.position);

            if (enemyNode != null && targetNode != null)
            {
                currentState = State.Following;
                pathToFollow = FindPath(transform.position, target.position);
                currentPathIndex = 0;
            }
        }
        else
        {
            currentState = State.Idle;
        }
    }


    private void FollowPath()
    {
        if (currentPathIndex < pathToFollow.Count)
        {
            Vector2 nextPoint = pathToFollow[currentPathIndex];
            MoveTowardsPoint(nextPoint);

            if (Vector2.Distance(transform.position, nextPoint) < 1.0f)
            {
                currentPathIndex++;
            }
        }
        else
        {
            currentState = State.Idle; // Ende des Pfades erreicht
        }
    }

    private List<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        Node startNode = grid.GetNodeFromWorldPosition(start);
        Node goalNode = grid.GetNodeFromWorldPosition(goal);

        Debug.Log($"StartNode: {startNode.Position}, GoalNode: {goalNode.Position}");

        var pathNodes = pathfinder.GetShortestPathAstar(startNode, goalNode);

        if (pathNodes == null || pathNodes.Count == 0)
        {
            Debug.Log("Kein Pfad gefunden.");
            return new List<Vector2>();
        }

        List<Vector2> path = new List<Vector2>();
        Debug.Log($"Gefundener Pfadlänge: {pathNodes.Count}");

        foreach (var node in pathNodes)
        {
            path.Add(node.Position);
        }

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

    // Optional: Weitere Methoden und Logik

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        /*if (grid == null || grid.Nodes == null)
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
        }*/   
        
        if (pathfinder != null && pathfinder.LastPath != null && pathfinder.LastPath.Count > 0)
        {
            // Draw the path in blue
            Gizmos.color = Color.blue;

            for (int i = 0; i < pathfinder.LastPath.Count; i++)
            {
                Node node = pathfinder.LastPath[i];
                Vector3 pos = new Vector3(
                        node.Position.x * MapData.Instance.tileSize / 100.0f, 
                        node.Position.y * MapData.Instance.tileSize / 100.0f, 
                        0) - MapData.Instance.mapCenter;

                Vector3 cubeSize = new Vector3(MapData.Instance.tileSize / 100.0f, MapData.Instance.tileSize / 100.0f, 1f) * 0.3f;

                // Mark the start node in green
                if (i == 0)
                {
                    Gizmos.color = Color.green;
                }
                // Mark the end node in red
                else if (i == pathfinder.LastPath.Count - 1)
                {
                    Gizmos.color = Color.red;
                }
                // The rest of the path in blue
                else
                {
                    Gizmos.color = Color.blue;
                }

                Gizmos.DrawCube(pos, cubeSize);
            }
        }
    }
    #endif
}
