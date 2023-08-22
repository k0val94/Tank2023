using UnityEngine;
using System.Collections;

public class EnemyTankMovementController : MonoBehaviour
{


    private float forwardSpeed = 45f;
    private float reverseSpeed = 10f;
    private float torque = 10f;
    private float maxSpeed = 20f;
    private float maxAngularVelocity = 20f;
    private float drag = 2f;
    private float rotationDamping = 10f;

    private bool areBothChainsMoving = false;
    private bool isLeftChainMoving = false;
    private bool isRightChainMoving = false;

    private int remainingAmmo = 25;
    private bool isReloadingAmmo = false;

    private EnemyTankTurretController currentEnemyTankTurretController;

    private GameObject currentEnemyTankTurretInstance;
    private Rigidbody2D currentEnemyTankRigidbody2D;

    public void Initialize(GameObject currentEnemyTankTurretInstance)
    {
        currentEnemyTankTurretController = currentEnemyTankTurretInstance.GetComponent<EnemyTankTurretController>();
        currentEnemyTankRigidbody2D = currentEnemyTankTurretInstance.GetComponentInParent<Rigidbody2D>();

        StartCoroutine(AmmoReloadCoroutine());
    }

    IEnumerator AmmoReloadCoroutine()
    {
        while (true)
        {
            if (remainingAmmo < 50 && !isReloadingAmmo)
            {
                isReloadingAmmo = true;
                yield return new WaitForSeconds(0.3f);
                remainingAmmo = 50;
                isReloadingAmmo = false;
            }
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            FireProjectile();
        }

    }

    void FixedUpdate()
    {

        //currentEnemyTankTurretController.RotateTowardsTarget();
        HandleTankMovementAI();

    }


    private Transform GetClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closestPlayer = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                closestPlayer = player.transform;
            }
        }

        return closestPlayer;
    }

    private void HandleTankMovementAI()
    {
        Transform target = GetClosestPlayer();
        
        // Falls kein Spieler gefunden wurde, brechen Sie die Funktion ab.
        if (target == null)
            return;

        float move = GetAIMoveValue(target);
        float rotate = GetAIRotateValue(target);

        HandleTankMovementBasedOnInput(move, rotate);
    }

    private float GetAIMoveValue(Transform target)
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget < 5f) 
        {
            return -1f;
        }
        else
        {
            return 1f;
        }
    }

    private float GetAIRotateValue(Transform target)
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float angleDifference = Vector2.SignedAngle(transform.up, directionToTarget);

        if (angleDifference > 10f) 
        {
            return -1f;
        }
        else if (angleDifference < -10f)
        {
            return 1f;
        }
        else
        {
            return 0f;
        }
    }


    private void HandleTankMovementBasedOnInput(float move, float rotate)
    {

        float currentSpeed = currentEnemyTankRigidbody2D.velocity.magnitude;

        areBothChainsMoving = false;
        isLeftChainMoving = false;
        isRightChainMoving = false;

        if (Mathf.Abs(move) > 0.1f)
        {
            areBothChainsMoving = true;
            Vector2 force = transform.up * move * (move > 0 ? forwardSpeed : reverseSpeed);
            currentEnemyTankRigidbody2D.AddForce(force);
        }

        if (Mathf.Abs(rotate) > 0.1f)
        {
            float rotationDirection = (move >= 0) ? -1 : 1;

            if (currentSpeed < 1f)
            {
                if (rotate < -0.1f)
                {
                    isLeftChainMoving = true;
                }
                else
                {
                    isRightChainMoving = true;
                }
                float turnTorque = rotationDirection * rotate * torque * 2;
                currentEnemyTankRigidbody2D.AddTorque(turnTorque);
            }
            else
            {
                float turnTorque = rotationDirection * rotate * torque;
                currentEnemyTankRigidbody2D.AddTorque(turnTorque);
            }
        }

        currentEnemyTankRigidbody2D.drag = drag;
        currentEnemyTankRigidbody2D.velocity *= (1f - drag * Time.fixedDeltaTime);
        currentEnemyTankRigidbody2D.angularVelocity *= (1f - rotationDamping * Time.fixedDeltaTime);
        currentEnemyTankRigidbody2D.velocity = Vector2.ClampMagnitude(currentEnemyTankRigidbody2D.velocity, maxSpeed);
        currentEnemyTankRigidbody2D.angularVelocity = Mathf.Clamp(currentEnemyTankRigidbody2D.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }

    void FireProjectile()
    {
        if (!isReloadingAmmo && remainingAmmo > 0)
        {
            remainingAmmo--;
            if (currentEnemyTankTurretController != null)
            {
                currentEnemyTankTurretController.FireProjectile();
            }
        }
    }

    public bool AreBothChainsMoving()
    {
        return areBothChainsMoving;
    }

    public bool IsLeftChainMoving()
    {
        return isLeftChainMoving;
    }

    public bool IsRightChainMoving()
    {
        return isRightChainMoving;
    }
}
