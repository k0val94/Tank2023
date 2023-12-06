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

    private void Awake()
    {
        fieldOfNoise = GetComponentInChildren<FieldOfNoise>();
        walkableGridManager = FindObjectOfType<WalkableGridManager>();
        if (fieldOfNoise == null)
        {
            Debug.LogError("FieldOfNoise component not found on the child objects!");
        }
        else
        {
            currentState = State.Idle;
        }
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

    private List<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        // Implement Dijkstra's Algorithm here
        // Convert start and goal to grid coordinates
        // Use walkableGridManager.GetWalkableGrid() for grid data
        // Convert the resulting path back to world coordinates
        return new List<Vector2>(); // Placeholder
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
}
