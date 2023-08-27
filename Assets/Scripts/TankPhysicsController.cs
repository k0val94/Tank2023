using UnityEngine;

public class TankPhysicsController : MonoBehaviour
{
    private Rigidbody2D tankRigidbody;

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

    private void Start()
    {
        tankRigidbody = GetComponent<Rigidbody2D>();
    }

    public void MoveTank(float move, float rotate)
    {
    float currentSpeed = tankRigidbody.velocity.magnitude;

        areBothChainsMoving = false;
        isLeftChainMoving = false;
        isRightChainMoving = false;

        if (Mathf.Abs(move) > 0.1f)
        {
            areBothChainsMoving = true;
            Vector2 force = transform.up * move * (move > 0 ? forwardSpeed : reverseSpeed);
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

        tankRigidbody.drag = drag;
        tankRigidbody.velocity *= (1f - drag * Time.fixedDeltaTime);
        tankRigidbody.angularVelocity *= (1f - rotationDamping * Time.fixedDeltaTime);
        tankRigidbody.velocity = Vector2.ClampMagnitude(tankRigidbody.velocity, maxSpeed);
        tankRigidbody.angularVelocity = Mathf.Clamp(tankRigidbody.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
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