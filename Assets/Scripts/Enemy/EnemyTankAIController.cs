using UnityEngine;
using UnityEngine.AI;

public class EnemyTankAIController : MonoBehaviour
{
    private float rotateSpeed = 0.5f;
    private Transform target;
    private TankPhysicsController tankPhysicsController;
    private FieldOfNoise fieldOfNoise;

    public enum State
    {
        Idle,
        Following
    }

    public State currentState;

    private void Awake()
    {
        fieldOfNoise = GetComponentInChildren<FieldOfNoise>();
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
    }

    private void Update()
    {
        target = fieldOfNoise.audibleTargets.Count > 0 ? fieldOfNoise.audibleTargets[0] : null;

        if (target != null)
        {
            currentState = State.Following;
        }
        else
        {
            currentState = State.Idle;
        }

        switch (currentState)
        {
            case State.Following:
                RotateTankTowardsTarget(target);
                FollowTarget(target);
                break;
            case State.Idle:
                break;
        }
    }

    private void RotateTankTowardsTarget(Transform target)
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float angleToTarget = Vector2.SignedAngle(transform.up, directionToTarget);

        if (Mathf.Abs(angleToTarget) > 5) 
        {
            float turnAmount = -Mathf.Sign(angleToTarget) * Mathf.Min(Mathf.Abs(angleToTarget) / 180, rotateSpeed);
            tankPhysicsController.MoveTank(0, turnAmount);
        }
    }

    private void FollowTarget(Transform target)
    {
        Vector2 enemyPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetPosition = new Vector2(target.position.x, target.position.y);

        float distanceToTarget = Vector2.Distance(enemyPosition, targetPosition);

        if (distanceToTarget <= fieldOfNoise.hearingRadius)
        {
            float angleToTarget = Vector2.SignedAngle(transform.up, targetPosition - enemyPosition);
            
            if (Mathf.Abs(angleToTarget) < 20) 
            {

                float stoppingDistance = 5.0f; 
                float gasAmount = Mathf.Clamp((distanceToTarget / stoppingDistance), 0f, 1f);

                tankPhysicsController.MoveTank(gasAmount / 2, 0);
            }
        }
        else
        {
            currentState = State.Idle;
        }
    }
}