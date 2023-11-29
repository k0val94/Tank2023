using UnityEngine;
using UnityEngine.AI;

public class EnemyTankAIController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    private TankPhysicsController tankPhysicsController;
    private FieldOfNoise fieldOfNoise;

    public enum State
    {
        Following
    }

    public State currentState;

    private void Start()
    {
        tankPhysicsController = GetComponent<TankPhysicsController>();
        agent = GetComponent<NavMeshAgent>();
        fieldOfNoise = GetComponentInChildren<FieldOfNoise>();

        if (fieldOfNoise == null)
        {
            Debug.LogError("FieldOfNoise component not found on the child objects!");
        }
        else
        {
            currentState = State.Following;
        }
    }

    private void Update()
    {
        Debug.Log($"Hearing Radius: {fieldOfNoise.hearingRadius}");
        UpdateTarget();

        if (currentState == State.Following && target != null)
        {
            FollowTarget();
        }
    }

    private void UpdateTarget()
    {
        if (fieldOfNoise.audibleTargets.Count > 0)
        {
            // Assume the closest target is the one to follow
            target = fieldOfNoise.audibleTargets[0];
            for (int i = 1; i < fieldOfNoise.audibleTargets.Count; i++)
            {
                if (Vector2.Distance(transform.position, fieldOfNoise.audibleTargets[i].position) <
                    Vector2.Distance(transform.position, target.position))
                {
                    target = fieldOfNoise.audibleTargets[i];
                }
            }
        }
        else
        {
            target = null;
        }
    }

    private void FollowTarget()
    {
        Debug.Log("Attempting to Follow Target");

        if (target != null)
        {
            // Convert to Vector2 for 2D distance calculation
            Vector2 enemyPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 targetPosition = new Vector2(target.position.x, target.position.y);

            float distanceToTarget = Vector2.Distance(enemyPosition, targetPosition);
            Debug.Log($"Target Detected. Distance to Target: {distanceToTarget}");

            if (distanceToTarget <= fieldOfNoise.hearingRadius) // Use hearing radius from FieldOfNoise
            {
                Debug.Log("Target within hearing range. Initiating follow.");

                // Determine movement direction (1 for forward, -1 for reverse)
                float moveDirection = 1; // Assuming always moving forward

                // Calculate the rotation needed to face the target
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                float angleToTarget = Vector3.SignedAngle(transform.up, directionToTarget, Vector3.forward);
                float rotateAmount = Mathf.Clamp(angleToTarget / 180, -1f, 1f); // Normalize to range [-1, 1]

                Debug.Log($"Moving towards target. Move Direction: {moveDirection}, Rotate Amount: {rotateAmount}");

                tankPhysicsController.MoveTank(moveDirection, rotateAmount);
            }
            else
            {
                Debug.Log("Target out of hearing range. Adjusting behavior.");
                // Logic for when the target is not in hearing range or not detected
                // Example: Patrol or idle
            }
        }
        else
        {
            Debug.Log("No target to follow.");
        }
    }
}
