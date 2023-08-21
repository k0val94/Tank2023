using UnityEngine;
using System.Collections;

public class PlayerTankMovementController : MonoBehaviour
{

    private float turretRotationSpeed = 50f;
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

    private float minZoom = 5f;
    private float maxZoom = 15f;
    private float zoomSpeed = 30f;

    private Transform currentPlayerTankTurretTransform;
    private TankTurretController currentPlayerTankTurretController;

    private GameObject currentPlayerTankTurretInstance;
    private Rigidbody2D currentPlayerTankRigidbody2D;

    public void Initialize(GameObject currentPlayerTankTurretInstance)
    {
        currentPlayerTankTurretController = currentPlayerTankTurretInstance.GetComponent<TankTurretController>();
        currentPlayerTankRigidbody2D = currentPlayerTankTurretInstance.GetComponentInParent<Rigidbody2D>();
        currentPlayerTankTurretTransform = currentPlayerTankTurretInstance.GetComponentInParent<Transform>();

        StartCoroutine(AmmoReloadCoroutine());
    }

    IEnumerator AmmoReloadCoroutine()
    {
        while (true)
        {
            if (remainingAmmo < 25 && !isReloadingAmmo)
            {
                isReloadingAmmo = true;
                yield return new WaitForSeconds(2f);
                remainingAmmo = 25;
                isReloadingAmmo = false;
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
        currentPlayerTankTurretController.RotateTurretTowardsMouse();

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
        float currentSpeed = currentPlayerTankRigidbody2D.velocity.magnitude;

        areBothChainsMoving = false;
        isLeftChainMoving = false;
        isRightChainMoving = false;

        if (Mathf.Abs(move) > 0.1f)
        {
            areBothChainsMoving = true;
            Vector2 force = transform.up * move * (move > 0 ? forwardSpeed : reverseSpeed);
            currentPlayerTankRigidbody2D.AddForce(force);
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
                currentPlayerTankRigidbody2D.AddTorque(turnTorque);
            }
            else
            {
                float turnTorque = rotationDirection * rotate * torque;
                currentPlayerTankRigidbody2D.AddTorque(turnTorque);
            }
        }

        currentPlayerTankRigidbody2D.drag = drag;
        currentPlayerTankRigidbody2D.velocity *= (1f - drag * Time.fixedDeltaTime);
        currentPlayerTankRigidbody2D.angularVelocity *= (1f - rotationDamping * Time.fixedDeltaTime);
        currentPlayerTankRigidbody2D.velocity = Vector2.ClampMagnitude(currentPlayerTankRigidbody2D.velocity, maxSpeed);
        currentPlayerTankRigidbody2D.angularVelocity = Mathf.Clamp(currentPlayerTankRigidbody2D.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }

    private void FireProjectile()
    {
        if (!isReloadingAmmo && remainingAmmo > 0)
        {
            remainingAmmo--;
            if (currentPlayerTankTurretController != null)
            {
                currentPlayerTankTurretController.FireProjectile();
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
