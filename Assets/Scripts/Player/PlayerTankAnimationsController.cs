using UnityEngine;

public class PlayerTankAnimationsController : MonoBehaviour
{
    private Animator playerTankAnimator;
    private PlayerTankMovementController playerTankMovementController;

    private void Start()
    {
       
        playerTankMovementController = GetComponent<PlayerTankMovementController>();
        playerTankAnimator = GetComponent<Animator>();
        Debug.Log("PlayerTankAnimationsController initialized.");

    }

    private void Update()
    {
        bool areBothChainsMoving = playerTankMovementController.AreBothChainsMoving();
        bool isLeftChainMoving = playerTankMovementController.IsLeftChainMoving();
        bool isRightChainMoving = playerTankMovementController.IsRightChainMoving();

        playerTankAnimator.SetBool("IsMovingBothChains", areBothChainsMoving);
        playerTankAnimator.SetBool("IsMovingLeftChain", isLeftChainMoving);
        playerTankAnimator.SetBool("IsMovingRightChain", isRightChainMoving);
    }
}
