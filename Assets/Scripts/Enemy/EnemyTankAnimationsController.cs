using UnityEngine;

public class EnemyTankAnimationsController : MonoBehaviour
{
    private Animator enemyTankAnimator;
    private TankPhysicsController tankPhysicsController;

    private void Start()
    {
        tankPhysicsController = GetComponent<TankPhysicsController>();
        enemyTankAnimator = GetComponent<Animator>();
        Debug.Log("EnemyTankAnimationsController initialized.");
    }

    private void Update()
    {
        bool areBothChainsMoving = tankPhysicsController.AreBothChainsMoving();
        bool isLeftChainMoving = tankPhysicsController.IsLeftChainMoving();
        bool isRightChainMoving = tankPhysicsController.IsRightChainMoving();

        enemyTankAnimator.SetBool("IsMovingBothChains", areBothChainsMoving);
        enemyTankAnimator.SetBool("IsMovingLeftChain", isLeftChainMoving);
        enemyTankAnimator.SetBool("IsMovingRightChain", isRightChainMoving);
    }
}
