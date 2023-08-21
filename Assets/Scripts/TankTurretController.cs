using UnityEngine;

public class TankTurretController : MonoBehaviour
{
    [Header("Turret Settings")]
    [SerializeField] private float turretRotationSpeed = 2f; 
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;

    private float firePointHorizontalOffset = -0.25f;
    private float firePointVerticalOffset = 0.5f;
    private Transform firePoint;

    private void Awake()
    {
        CalculateFirePosition();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            FireProjectile();
        }
    }

    public void RotateTurretTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turretRotationSpeed * Time.deltaTime);
    }

    void CalculateFirePosition()
    {
        Vector3 turretTop = transform.position + transform.up * (GetComponent<SpriteRenderer>().bounds.size.y / 2);
        Vector3 calculatedFirePoint = turretTop + transform.up * firePointHorizontalOffset + Vector3.up * firePointVerticalOffset;
        if (firePoint == null)
        {
            firePoint = new GameObject("Fire Point").transform;
            firePoint.SetParent(transform);
        }

        firePoint.position = calculatedFirePoint;
        firePoint.rotation = transform.rotation;
    }

    public void FireProjectile()
    {
        if (firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D projectileRigidbody2D = projectile.GetComponent<Rigidbody2D>();
            if (projectileRigidbody2D != null)
            {
                projectileRigidbody2D.velocity = firePoint.up * projectileSpeed;
            }
        }
        else
        {
            Debug.LogWarning("FirePoint reference is missing. Please make sure to assign the fire point in the inspector.");
        }
    }
}
