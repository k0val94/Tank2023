using UnityEngine;

public class EnemyTankMovementController : MonoBehaviour
{
    private Transform player;
    private TankPhysicsController tankPhysicsController;

    public float movementThreshold = 4.0f;

    private void Start()
    {
        tankPhysicsController = GetComponent<TankPhysicsController>();
    }

    private void FixedUpdate()
    {
        HandleTankMovement();
    }

    private void HandleTankMovement()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > movementThreshold)
        {
            float angleToPlayer = Vector3.SignedAngle(transform.up, toPlayer, Vector3.forward);
            float rotate = Mathf.Clamp(angleToPlayer / 180.0f, -1.0f, 1.0f);
            float move = Mathf.Sign(Vector3.Dot(transform.up, toPlayer.normalized));

            tankPhysicsController.MoveTank(move, rotate);
        }
        else
        {
            tankPhysicsController.MoveTank(0.0f, 0.0f);
        }
    }
}