using UnityEngine;
using UnityEngine.AI;

public class EnemyTankAIController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private TankPhysicsController tankPhysicsController;
    private FieldOfView fov; // Angenommen, du hast eine FieldOfView-Komponente, die das Sichtfeld repräsentiert

    public enum State
    {
        Patrolling,
        Attacking,
        Evading,
        SeekingCover
    }

    public State currentState;
    public Transform[] waypoints; // Wegpunkte für das Patrouillieren
    private int waypointIndex = 0;

    public LayerMask coverLayerMask; // Layer für potenzielle Deckung
    public float attackRange = 10.0f;
    public float evasionDistance = 5.0f;
    public float coverCheckRadius = 15.0f;

    private void Start()
    {
        tankPhysicsController = GetComponent<TankPhysicsController>();
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        currentState = State.Patrolling;
        player = GameObject.FindGameObjectWithTag("Player").transform; // Achte darauf, dass der Spieler dieses Tag hat
    }

    private void Update()
    {
        // Verwalte den Zustand der KI basierend auf verschiedenen Bedingungen
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                if (fov.visibleTargets.Contains(player))
                {
                    ChangeState(State.Attacking);
                }
                break;
            case State.Attacking:
                if (!fov.visibleTargets.Contains(player))
                {
                    ChangeState(State.Patrolling);
                }
                else if (Vector3.Distance(transform.position, player.position) > attackRange)
                {
                    ApproachPlayer();
                }
                else
                {
                    AttackPlayer();
                    if (ShouldEvade())
                    {
                        ChangeState(State.Evading);
                    }
                }
                break;
            case State.Evading:
                Evade();
                if (IsSafe())
                {
                    ChangeState(State.SeekingCover);
                }
                break;
            case State.SeekingCover:
                SeekCover();
                if (IsCovered())
                {
                    ChangeState(State.Attacking);
                }
                break;
        }
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[waypointIndex];
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 1f)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length; // Zum nächsten Wegpunkt übergehen
        }
        Vector3 dirToWaypoint = (targetWaypoint.position - transform.position).normalized;
        float angleToWaypoint = Vector3.SignedAngle(transform.up, dirToWaypoint, Vector3.forward);
        float rotate = angleToWaypoint > 0 ? 1f : -1f;
        tankPhysicsController.MoveTank(1f, rotate);

        if (Mathf.Abs(angleToWaypoint) > 5f) 
        {
            tankPhysicsController.MoveTank(0f, rotate);
        }
    }


    private void ApproachPlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Implementiere Angriffslogik hier
    }

    private bool ShouldEvade()
    {
        // Bestimme, ob die KI ausweichen sollte
        return Vector3.Distance(transform.position, player.position) < evasionDistance;
    }

    private void Evade()
    {
        // Implementiere Ausweichlogik hier
    }

    private bool IsSafe()
    {
        // Überprüfe, ob die KI in Sicherheit ist
        return !fov.visibleTargets.Contains(player);
    }

    private void SeekCover()
    {
        // Finde die nächstgelegene Deckung und bewege die KI dorthin
    }

    private bool IsCovered()
    {
        // Überprüfe, ob die KI in Deckung ist
        Collider[] hits = Physics.OverlapSphere(transform.position, coverCheckRadius, coverLayerMask);
        foreach (var hit in hits)
        {
            if (hit.transform != transform)
            {
                return true;
            }
        }
        return false;
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
    }

    // Weitere Methoden für das Schießen und andere Aktionen
}
