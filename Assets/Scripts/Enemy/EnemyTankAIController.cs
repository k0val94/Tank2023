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
            target = fieldOfNoise.audibleTargets.Count > 0 ? fieldOfNoise.audibleTargets[0] : null;
        }
    }

    private void Update()
    {
        if (currentState == State.Following)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        Debug.Log("Attempting to Follow Target");

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
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
