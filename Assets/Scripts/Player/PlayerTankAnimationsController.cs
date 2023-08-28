using UnityEngine;

public class PlayerTankAnimationsController : MonoBehaviour
{
    private Animator playerTankAnimator;
    private TankPhysicsController tankPhysicsController;

    private void Start()
    {
        tankPhysicsController = GetComponent<TankPhysicsController>();
        playerTankAnimator = GetComponent<Animator>();
        Debug.Log("PlayerTankAnimationsController initialized.");
    }

    private void Update()
    {
        bool areBothChainsMoving = tankPhysicsController.AreBothChainsMoving();
        bool isLeftChainMoving = tankPhysicsController.IsLeftChainMoving();
        bool isRightChainMoving = tankPhysicsController.IsRightChainMoving();

        playerTankAnimator.SetBool("IsMovingBothChains", areBothChainsMoving);
        playerTankAnimator.SetBool("IsMovingLeftChain", isLeftChainMoving);
        playerTankAnimator.SetBool("IsMovingRightChain", isRightChainMoving);
    }
}
