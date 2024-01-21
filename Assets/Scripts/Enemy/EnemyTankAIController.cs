using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyTankAIController : MonoBehaviour
{
    private float rotateSpeed;
    private float speedIdentifier;
    private Transform target;
    private TankPhysicsController tankPhysicsController;
    private FieldOfNoise fieldOfNoise;
    private WalkableGridManager walkableGridManager;
    private List<Vector2> pathToFollow;
    private int currentPathIndex;
    private Pathfinder pathfinder;
    private Grid grid;
    private bool isTurning = false;

    public enum State
    {
        Idle,
        Following
    }

    public State currentState;

    private float pathUpdateCooldown = 2.0f;
    private float lastPathUpdateTime = 0.0f;

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

        grid = new Grid(MapData.Instance.GetWalkableGrid().GetLength(0), 
                        MapData.Instance.GetWalkableGrid().GetLength(1), 
                        MapData.Instance.GetWalkableGrid());
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

        if (Time.time - lastPathUpdateTime > pathUpdateCooldown)
        {
            UpdateTarget();
            lastPathUpdateTime = Time.time;
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
                currentPathIndex = pathToFollow.Count > 3 ? 2 : 0;
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
            // Stop the tank if it's within a certain range of the target
            if (pathToFollow.Count - currentPathIndex <= 4)
            {
                currentState = State.Idle;
                return;
            }

            Vector2 startNode = pathToFollow[0]; // Start node from the path as grid position
            Vector2 nextNode = pathToFollow[currentPathIndex]; // Next point in the path as grid position

            MoveTowardsNextNode(startNode, nextNode); // Pass both start and next nodes

            // Check if the tank has reached the next node
            /*Vector2 currentWorldPosition = new Vector2(transform.position.x, transform.position.y);
            if (Vector2.Distance(currentWorldPosition, grid.GetWorldPositionFromNode(nextGridPosition)) < 1.0f) // Adjust the threshold as needed
            {
                currentPathIndex++;
            }*/
        }
        else
        {
            currentState = State.Idle; // End of the path reached
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

    private void MoveTowardsNextNode(Vector2 startNode, Vector2 nextNode)
    {
        Vector2 direction = nextNode - startNode;
        float angleRad = Mathf.Atan2(direction.y, direction.x);
        float angleDeg = (angleRad * Mathf.Rad2Deg + 360) % 360;

        float currentAngle = (transform.eulerAngles.z + 90) % 360;

        Debug.Log($"StartNode: {startNode}, NextNode: {nextNode}, Direction: {direction}, " + 
                $"Angle to Target in Degrees: {angleDeg}, Current Tank Angle (adjusted): {currentAngle}");


        Debug.Log(isTurning);                

        // Wenn der Panzer nicht im Drehmodus ist und der Winkel nicht in die gewünschte Richtung zeigt, starten Sie das Drehen
        if (!isTurning)
        {
            float angleDifference = Mathf.DeltaAngle(currentAngle, angleDeg);

            if (angleDifference < 0)
            {
                rotateSpeed = 0.5f; // oder ein anderer Wert für die Drehgeschwindigkeit
                isTurning = true;
            }
            else if (angleDifference > 0)
            {
                rotateSpeed = -0.5f; // oder ein anderer Wert für die Drehgeschwindigkeit
                isTurning = true;
            }
            else if (angleDifference == 0)
            {
                rotateSpeed = 0;
            }

        }
        else
        {
            // Der Panzer ist im Drehmodus

            tankPhysicsController.MoveTank(0, rotateSpeed);
            Debug.Log(Mathf.Abs(angleDeg - currentAngle));
            
            // Überprüfen Sie, ob die Drehung abgeschlossen ist (kein Winkelunterschied mehr)
            if (Mathf.Abs(angleDeg - currentAngle) < 20.0f) // Hier den Schwellenwert anpassen, ab dem die Drehung als abgeschlossen gilt
            {
                isTurning = false; // Drehen ist abgeschlossen
                // Hier können Sie den Panzer in den Bewegungsmodus versetzen
                tankPhysicsController.MoveTank(1, 0);
            }
        }
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {

        if (MapData.Instance.walkableGrid == null)
        {
            Debug.Log("walkableGrid is null.");
            return;
        }

        for (int x = 0; x < MapData.Instance.width; x++)
        {
            for (int y = 0; y < MapData.Instance.height; y++)
            {
                Vector3 pos = new Vector3(
                    x * (MapData.Instance.tileSize / 100.0f), 
                    y * (MapData.Instance.tileSize / 100.0f), 
                    0) - MapData.Instance.mapCenter;

                // Set color based on walkability: Green for walkable, Red for non-walkable
                Color gizmoColor = MapData.Instance.walkableGrid[x, y] ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
                Gizmos.color = gizmoColor;

                float scaleFactor = 0.9f; 
                Vector3 cubeSize = new Vector3(MapData.Instance.tileSize / 100.0f * scaleFactor, MapData.Instance.tileSize / 100.0f * scaleFactor, 1f);
                Gizmos.DrawWireCube(pos, cubeSize);

                #if UNITY_EDITOR
                Handles.Label(pos, $"({x},{y})");
                #endif
            }
        }
            
        if (pathfinder != null && pathfinder.LastPath != null && pathfinder.LastPath.Count > 0)
        {
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
        /*

        Gizmos.color = Color.cyan; // Color for the facing direction field of view

        float viewAngle = 10f; // The same angle as used in IsTankFacingDirection
        float viewDistance = 5f; // Adjust the distance as needed

        // Draw the left boundary of the field of view
        Quaternion leftBoundaryRotation = Quaternion.AngleAxis(-viewAngle, Vector3.forward);
        Vector3 leftBoundaryDirection = leftBoundaryRotation * transform.up;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundaryDirection * viewDistance);

        // Draw the right boundary of the field of view
        Quaternion rightBoundaryRotation = Quaternion.AngleAxis(viewAngle, Vector3.forward);
        Vector3 rightBoundaryDirection = rightBoundaryRotation * transform.up;
        Gizmos.DrawLine(transform.position, transform.position + rightBoundaryDirection * viewDistance);

        // Optional: Draw more lines to fill the arc for better visualization
        int steps = 10; // Increase or decrease for more or fewer lines
        for (int i = 0; i <= steps; i++)
        {
            float stepAngle = viewAngle * 2 / steps * i - viewAngle;
            Quaternion rotation = Quaternion.AngleAxis(stepAngle, Vector3.forward);
            Vector3 direction = rotation * transform.up;
            Gizmos.DrawLine(transform.position, transform.position + direction * viewDistance);
        }*/


        // Draw a special gizmo for the current node being followed
        if (currentPathIndex < pathToFollow.Count)
        {
            Vector2 currentNodePosition = pathToFollow[currentPathIndex];
            Vector3 currentNodeWorldPos = new Vector3(
                currentNodePosition.x * MapData.Instance.tileSize / 100.0f, 
                currentNodePosition.y * MapData.Instance.tileSize / 100.0f, 
                0) - MapData.Instance.mapCenter;

            Gizmos.color = Color.magenta; // Magenta color for the current node
            Vector3 currentNodeSize = new Vector3(MapData.Instance.tileSize / 100.0f, MapData.Instance.tileSize / 100.0f, 1f) * 0.5f; // Slightly larger size
            Gizmos.DrawCube(currentNodeWorldPos, currentNodeSize);
        }

    }
    #endif

}