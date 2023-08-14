using UnityEngine;

public class TankAnimationsController : MonoBehaviour
{
    private Animator animator;
    private TankController tankController; // Referenz auf den TankController

    private void Start()
    {
        animator = GetComponent<Animator>();
        tankController = GetComponent<TankController>();
    }

    private void Update()
    {

        // Aktualisiere die Animator-Parameter basierend auf den Bewegungen des TankControllers
        bool bothChainsMoving = tankController.IsBothChainsMoving();
        bool leftChainMoving = tankController.IsLeftChainMoving();
        bool rightChainMoving = tankController.IsRightChainMoving();

        // Setze die Animator-Parameter entsprechend den Zuständen der Bewegungen
        animator.SetBool("IsMovingBothChains", bothChainsMoving);
        animator.SetBool("IsMovingLeftChain", leftChainMoving);
        animator.SetBool("IsMovingRightChain", rightChainMoving);

    }
}
