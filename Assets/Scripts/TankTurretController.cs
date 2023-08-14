using UnityEngine;

public class TankTurretController : MonoBehaviour
{
    public float turretRotationSpeed = 90f; // Grad pro Sekunde
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    private float firePointOffset = -0.25f; // Abstand von der Turmspitze, wo das Projektil abgefeuert wird
    private float firePointHeight = 0.5f; // Höhe des Fire Points über der Turmspitze

    private Transform firePoint; // Der Transform, von dem aus die Projektile geschossen werden

    private void Awake()
    {
        CalculateFirePoint(); // Berechne den Fire Point beim Start
    }

    void Update()
    {
        RotateTurretTowardsMouse();

        if (Input.GetMouseButtonDown(1)) // Rechte Maustaste
        {
            FireProjectile();
        }
    }

    void RotateTurretTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turretRotationSpeed * Time.deltaTime);
    }

    void CalculateFirePoint()
    {
        // Annahme: Der Turm ist oben auf dem GameObject und zeigt nach oben.
        Vector3 turretTop = transform.position + transform.up * (GetComponent<SpriteRenderer>().bounds.size.y / 2);

        // Berechne den firePoint basierend auf den Offset- und Höhenwerten
        Vector3 calculatedFirePoint = turretTop + transform.up * firePointOffset + Vector3.up * firePointHeight;

        // Erstelle ein leeres GameObject als Fire Point, wenn noch keines existiert
        if (firePoint == null)
        {
            firePoint = new GameObject("Fire Point").transform;
            firePoint.SetParent(transform); // Setze den Turm als Elternteil des Fire Points
        }

        firePoint.position = calculatedFirePoint;
        firePoint.rotation = transform.rotation;
    }

    public void FireProjectile()
    {
        if (firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
            if (projectileRigidbody != null)
            {
                projectileRigidbody.velocity = firePoint.up * projectileSpeed;
            }
        }
        else
        {
            Debug.LogWarning("FirePoint reference is missing. Please make sure to assign the fire point in the inspector.");
        }
    }
}
