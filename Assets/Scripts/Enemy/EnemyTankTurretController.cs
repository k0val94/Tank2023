using UnityEngine;
using System.Collections;

public class EnemyTankTurretController : MonoBehaviour
{
    [Header("Turret Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float turretRotationSpeed = 90f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float firePointVerticalOffset = 0.5f;
    private Transform firePoint;
    private FieldOfView fieldOfView; 
    private bool isReloadingAmmo = false;
    private float reloadTime = 1.5f;
    private float timeSinceLastShot = 1.5f;

    private void Awake()
    {
          fieldOfView = GetComponent<FieldOfView>();
        CreateFirePoint();
    }

    private void Update()
    {
        Transform target = fieldOfView.visibleTargets.Count > 0 ? fieldOfView.visibleTargets[0] : null;
        if (target != null)
        {
            RotateTurretTowardsTarget(target);
            timeSinceLastShot += Time.deltaTime;

            if (!isReloadingAmmo && timeSinceLastShot >= reloadTime)
            {
                FireProjectile();
                timeSinceLastShot = 0f;
                isReloadingAmmo = true;
                StartCoroutine(ReloadAmmo());
            }
        }
    }

    private void RotateTurretTowardsTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.Normalize();
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

        Vector3 firePointOffset = transform.up * firePointVerticalOffset;
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

    private IEnumerator ReloadAmmo()
    {
        yield return new WaitForSeconds(reloadTime);
        isReloadingAmmo = false;
    }
}
