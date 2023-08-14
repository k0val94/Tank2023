using UnityEngine;
using System.Collections;

public class TankController : MonoBehaviour
{
    private Rigidbody2D tankRigidbody;
    private Animator tankAnimator;
    private float speed = 45f;
    private float reverseSpeed = 10f;
    private float torque = 10f;
    private float maxSpeed = 20f;
    private float maxAngularVelocity = 20f;
    private float drag = 2f;
    private float rotationDamping = 10f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public bool canShoot = true;

    private bool isBothChainsMoving = false;
    private bool isLeftChainMoving = false;
    private bool isRightChainMoving = false;

    void Start()
    {
        tankRigidbody = GetComponent<Rigidbody2D>();
        tankAnimator = GetComponent<Animator>();

        if (tankRigidbody == null)
        {
            Debug.LogWarning("No Rigidbody2D component found on " + gameObject.name + ". TankController has been disabled.");
            enabled = false;
            return;
        }

        PhysicsMaterial2D highFrictionMaterial = new PhysicsMaterial2D();
        highFrictionMaterial.name = "HighFriction";
        highFrictionMaterial.friction = 1.0f;

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.sharedMaterial = highFrictionMaterial;
        }
        StartCoroutine(ShootCoroutine());
    }

    IEnumerator ShootCoroutine()
    {
        while (true)
        {
            while (!Input.GetKeyDown(KeyCode.Space) || !canShoot)
            {
                yield return null;
            }

            canShoot = false;

            Vector3 spawnPosition = transform.position + transform.up;

            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, transform.rotation);

            Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
            if (projectileRigidbody != null)
            {
                projectileRigidbody.velocity = transform.up * projectileSpeed;
            }

            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            if (projectileController != null)
            {
                projectileController.tankController = this;
            }

            yield return new WaitForSeconds(1f);

            canShoot = true;
        }
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Vertical");
        float rotate = Input.GetAxis("Horizontal");

        isBothChainsMoving = Mathf.Abs(move) > 0.1f;
        isLeftChainMoving = rotate < -0.1f;
        isRightChainMoving = rotate > 0.1f;


        tankRigidbody.drag = drag;

        if (move > 0.1f)
        {
            Vector2 force = transform.up * move * speed;
            tankRigidbody.AddForce(force);
        }
        else if (move < -0.1f && tankRigidbody.velocity.magnitude < 0.1f)
        {
            Vector2 force = transform.up * move * reverseSpeed;
            tankRigidbody.AddForce(force);
        }

        if (Mathf.Abs(rotate) > 0.1f)
        {
            float turnTorque = -rotate * torque;
            tankRigidbody.AddTorque(turnTorque);
        }
        else
        {
            tankRigidbody.angularVelocity = 0f;
        }

        tankRigidbody.velocity *= (1f - drag * Time.fixedDeltaTime);
        tankRigidbody.angularVelocity *= (1f - rotationDamping * Time.fixedDeltaTime);

        tankRigidbody.velocity = Vector2.ClampMagnitude(tankRigidbody.velocity, maxSpeed);
        tankRigidbody.angularVelocity = Mathf.Clamp(tankRigidbody.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }

    public bool IsBothChainsMoving()
    {
        return isBothChainsMoving;
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
