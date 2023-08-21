using UnityEngine;

public class PlayerTankAnimationsController : MonoBehaviour
{
    private Animator animator;
    private PlayerTankMovementController movementController;

    private void Start()
    {
        animator = GetComponent<Animator>();
        movementController = GetComponent<PlayerTankMovementController>();
    }

    private void Update()
    {
        bool areBothChainsMoving = movementController.AreBothChainsMoving();
        bool isLeftChainMoving = movementController.IsLeftChainMoving();
        bool isRightChainMoving = movementController.IsRightChainMoving();

        animator.SetBool("IsMovingBothChains", areBothChainsMoving);
        animator.SetBool("IsMovingLeftChain", isLeftChainMoving);
        animator.SetBool("IsMovingRightChain", isRightChainMoving);
    }
}
