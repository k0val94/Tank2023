using UnityEngine;

public class QuicksandEffect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        TankPhysicsController tankPhysicsController = other.GetComponent<TankPhysicsController>();

        if (tankPhysicsController != null)
        {
            tankPhysicsController.SetInQuicksand(true);
            Debug.Log("Panzer ist jetzt im Quicksand, Effekt ist aktiv.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TankPhysicsController tankPhysicsController = other.GetComponent<TankPhysicsController>();

        if (tankPhysicsController != null)
        {
            tankPhysicsController.SetInQuicksand(false);
            Debug.Log("Panzer hat den Quicksand verlassen, Effekt ist deaktiviert.");
        }
    }
}