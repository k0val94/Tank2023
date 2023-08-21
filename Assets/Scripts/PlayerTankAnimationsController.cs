using UnityEngine;

public class PlayerTankAnimationsController : MonoBehaviour
{
    private Animator currentPlayerTankAnimator;
    private PlayerTankMovementController playerTankMovementController;


    public void Initialize(PlayerTankMovementController playerTankMovementController, GameObject currentPlayerTankTurretInstance)
    {
       
        this.playerTankMovementController = playerTankMovementController;
        currentPlayerTankAnimator = currentPlayerTankTurretInstance.GetComponentInParent<Animator>();
        Debug.Log("PlayerTankAnimationsController initialized.");

    }

    private void Update()
    {
        bool areBothChainsMoving = playerTankMovementController.AreBothChainsMoving();
        bool isLeftChainMoving = playerTankMovementController.IsLeftChainMoving();
        bool isRightChainMoving = playerTankMovementController.IsRightChainMoving();

        currentPlayerTankAnimator.SetBool("IsMovingBothChains", areBothChainsMoving);
        currentPlayerTankAnimator.SetBool("IsMovingLeftChain", isLeftChainMoving);
        currentPlayerTankAnimator.SetBool("IsMovingRightChain", isRightChainMoving);
    }
}
