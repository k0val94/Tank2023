using UnityEngine;

public class QuicksandEffect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        TankPhysicsController tankPhysicsController = other.GetComponent<TankPhysicsController>();

        if (tankPhysicsController != null)
        {
            tankPhysicsController.SetInQuicksand(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TankPhysicsController tankPhysicsController = other.GetComponent<TankPhysicsController>();

        if (tankPhysicsController != null)
        {
            tankPhysicsController.SetInQuicksand(false);
        }
    }
}