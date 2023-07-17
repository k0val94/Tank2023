using UnityEngine;
using System.Collections;

public class TankController : MonoBehaviour
{
    private Rigidbody2D tankRigidbody;
    private float speed = 45f;
    private float reverseSpeed = 10f;
    private float torque = 10f;
    private float maxSpeed = 20f;
    private float maxAngularVelocity = 20f;
    private float drag = 2f; // Die Drag-Kraft
    private float rotationDamping = 10f; // Dämpfung für die Rotation
    public GameObject projectilePrefab; // Das Prefab für das Projektil
    public float projectileSpeed = 10f; // Die Anfangsgeschwindigkeit des Projektils
    public bool canShoot = true; // Ob der Panzer schießen kann

    void Start()
    {
        tankRigidbody = GetComponent<Rigidbody2D>();

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
            // Warte, bis die Leertaste gedrückt wird und der Panzer schießen kann
            while (!Input.GetKey(KeyCode.Space) || !canShoot)
            {
                yield return null;
            }

            // Verhindere, dass der Panzer sofort wieder schießen kann
            canShoot = false;

            // Berechne die Position, an der das Projektil erscheinen soll
            Vector3 spawnPosition = transform.position + transform.up;

            // Erstelle eine Instanz des Projektils an der berechneten Position und mit der gleichen Ausrichtung
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, transform.rotation);

            // Füge dem Projektil eine Anfangsgeschwindigkeit hinzu
            Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
            if (projectileRigidbody != null)
            {
                projectileRigidbody.velocity = transform.up * projectileSpeed;
            }

            // Setze den TankController für das Projektil
            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            if (projectileController != null)
            {
                projectileController.tankController = this;
            }
        }
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Vertical");
        float rotate = Input.GetAxis("Horizontal");

        // Apply drag
        tankRigidbody.drag = drag;

        if (move > 0.1f) // Vorwärts
        {
            Vector2 force = transform.up * move * speed;
            tankRigidbody.AddForce(force);
        }
        else if (move < -0.1f && tankRigidbody.velocity.magnitude < 0.1f) // Rückwärts, nur wenn der Panzer gestoppt ist
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
            // Set angular velocity to 0 when not rotating
            tankRigidbody.angularVelocity = 0f;
        }

        // Dämpfung der Geschwindigkeit
        tankRigidbody.velocity *= (1f - drag * Time.fixedDeltaTime);

        // Dämpfung der Rotation
        tankRigidbody.angularVelocity *= (1f - rotationDamping * Time.fixedDeltaTime);

        tankRigidbody.velocity = Vector2.ClampMagnitude(tankRigidbody.velocity, maxSpeed);
        tankRigidbody.angularVelocity = Mathf.Clamp(tankRigidbody.angularVelocity, -maxAngularVelocity, maxAngularVelocity);

        Debug.Log("Velocity: " + tankRigidbody.velocity);
        Debug.Log("Angular velocity: " + tankRigidbody.angularVelocity);
    }
}
