using UnityEngine;

public class PlayerTankAnimationsController : MonoBehaviour
{
    private Animator animator;
    private PlayerTankMovementController playerTankMovementController;

    public void Initialize(PlayerTankMovementController playerTankMovementController)
    {
        this.playerTankMovementController = playerTankMovementController;
        animator = GetComponent<Animator>();
        Debug.Log("PlayerTankAnimationsController initialized.");

    }

    /*private void Update()
    {
        Debug.Log("playerTankMovementController.AreBothChainsMoving()." + playerTankMovementController.AreBothChainsMoving());
        bool areBothChainsMoving = playerTankMovementController.AreBothChainsMoving();
        bool isLeftChainMoving = playerTankMovementController.IsLeftChainMoving();
        bool isRightChainMoving = playerTankMovementController.IsRightChainMoving();

        animator.SetBool("IsMovingBothChains", areBothChainsMoving);
        animator.SetBool("IsMovingLeftChain", isLeftChainMoving);
        animator.SetBool("IsMovingRightChain", isRightChainMoving);
    }*/
}
