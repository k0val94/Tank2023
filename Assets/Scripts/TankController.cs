using UnityEngine;
using System.Collections;

public class TankController : MonoBehaviour
{
    public Transform tankTurret; // Der Turm des Panzers
    private Rigidbody2D tankRigidbody;
    private float turretRotationSpeed = 50f; // Geschwindigkeit der Turmdrehung
    private float speed = 45f;
    private float reverseSpeed = 10f;
    private float torque = 10f;
    private float maxSpeed = 20f;
    private float maxAngularVelocity = 20f;
    private float drag = 2f;
    private float rotationDamping = 10f;

    private bool isBothChainsMoving = false;
    private bool isLeftChainMoving = false;
    private bool isRightChainMoving = false;

    private TankTurretController turretController; // Referenz auf den Turm-Controller

    private int remainingShots = 25; // Verbleibende Schüsse
    private bool isReloading = false; // Wird gerade nachgeladen

    private float minZoom = 5f;
    private float maxZoom = 15f;
    private float zoomSpeed = 30f;

    void Start()
    {
        tankRigidbody = GetComponent<Rigidbody2D>();

        if (tankRigidbody == null)
        {
            Debug.LogWarning("No Rigidbody2D component found on " + gameObject.name + ". TankController has been disabled.");
            enabled = false;
            return;
        }

        if (tankTurret == null)
        {
            Debug.LogWarning("TankTurret reference is not set. Please attach the turret to the TankController.");
        }

        turretController = tankTurret.GetComponent<TankTurretController>(); // Turm-Controller holen

        StartCoroutine(ShotReloadCoroutine());
    }

    IEnumerator ShotReloadCoroutine()
    {
        while (true)
        {
            if (remainingShots < 25 && !isReloading)
            {
                isReloading = true;
                yield return new WaitForSeconds(2f); // Nachladen dauert 2 Sekunden
                remainingShots = 25;
                isReloading = false;
            }
            yield return null;
        }
    }

    void Update()
    {
        HandleCameraZoom();

    }

    void FixedUpdate()
    {
        HandleTankMovement();
        RotateTurretTowardsMouse();

        if (Input.GetMouseButtonDown(1)) // Rechtsklick (Mouse Button 1) fürs Schießen
        {
            FireProjectile();
        }
    }

    private void HandleCameraZoom()
    {
        if (Camera.main != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            Camera.main.orthographicSize -= scrollInput * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }
    }

    private void HandleTankMovement()
    {
        float move = Input.GetAxis("Vertical");
        float rotate = Input.GetAxis("Horizontal");

        float currentSpeed = tankRigidbody.velocity.magnitude;

        // Standardmäßig setzen wir alle Kettenbewegungs-Flags zurück.
        isBothChainsMoving = false;
        isLeftChainMoving = false;
        isRightChainMoving = false;

        if (Mathf.Abs(move) > 0.1f)
        {
            // Der Panzer bewegt sich vorwärts oder rückwärts.
            isBothChainsMoving = true;
            Vector2 force = transform.up * move * (move > 0 ? speed : reverseSpeed);
            tankRigidbody.AddForce(force);
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
                tankRigidbody.AddTorque(turnTorque);
            }
            else
            {
                float turnTorque = rotationDirection * rotate * torque;
                tankRigidbody.AddTorque(turnTorque);
            }
        }

        // Geschwindigkeits-/Rotationsgrenzen und Dämpfung
        tankRigidbody.drag = drag;
        tankRigidbody.velocity *= (1f - drag * Time.fixedDeltaTime);
        tankRigidbody.angularVelocity *= (1f - rotationDamping * Time.fixedDeltaTime);
        tankRigidbody.velocity = Vector2.ClampMagnitude(tankRigidbody.velocity, maxSpeed);
        tankRigidbody.angularVelocity = Mathf.Clamp(tankRigidbody.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }

    private void RotateTurretTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - tankTurret.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float rotationZ = Mathf.LerpAngle(tankTurret.rotation.eulerAngles.z, angle - 90f, turretRotationSpeed * Time.deltaTime); // 90 Grad Korrektur
        tankTurret.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

    private void FireProjectile()
    {
        if (!isReloading && remainingShots > 0)
        {
            remainingShots--;
            if (turretController != null)
            {
                turretController.FireProjectile(); // Schießvorgang im Turm-Controller aufrufen
            }
        }
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
