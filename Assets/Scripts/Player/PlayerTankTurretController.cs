using UnityEngine;
using System.Collections;

public class PlayerTankTurretController : MonoBehaviour
{
    [Header("Turret Settings")]

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int remainingAmmo = 25;
    
    private float turretRotationSpeed = 90f; 
    private float projectileSpeed = 10f;
    private float firePointVerticalOffset = 0.5f;
    private Transform firePoint;


    private bool isReloadingAmmo = false;

    private void Awake()
    {
        CreateFirePoint();
    }

    private void Start(){
        StartCoroutine(AmmoReloadCoroutine());
    }

    private void Update()
    {
        RotateTurretTowardsMouse();

        if (Input.GetMouseButtonDown(1))
        {
            FireProjectile();
        }
    }

    IEnumerator AmmoReloadCoroutine()
    {
        while (true)
        {
            if (remainingAmmo > 0 && !isReloadingAmmo)
            {
                isReloadingAmmo = true;
                yield return new WaitForSeconds(0.3f);
                remainingAmmo--;
                isReloadingAmmo = false;
            }
            yield return null;
        }
    }


    private void RotateTurretTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turretRotationSpeed * Time.deltaTime);
    }

    private void CreateFirePoint()
    {
        if (firePoint == null)
        {
            firePoint = new GameObject("Fire Point").transform;
            firePoint.SetParent(transform);
        }

        Vector3 firePointOffset = transform.up * firePointVerticalOffset; // Adjust this offset as needed
        firePoint.localPosition = firePointOffset;
        firePoint.localRotation = Quaternion.identity;
    }

    private void FireProjectile()
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
