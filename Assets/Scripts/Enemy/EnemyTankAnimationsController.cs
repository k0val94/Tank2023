using UnityEngine;

public class EnemyTankAnimationsController : MonoBehaviour
{
    private Animator currentEnemyTankAnimator;
    private EnemyTankMovementController enemyTankMovementController;


    public void Initialize(EnemyTankMovementController enemyTankMovementController, GameObject currentEnemyTankTurretInstance)
    {
       
        this.enemyTankMovementController = enemyTankMovementController;
        currentEnemyTankAnimator = currentEnemyTankTurretInstance.GetComponentInParent<Animator>();
        Debug.Log("EnemyTankAnimationsController initialized.");

    }

    private void Update()
    {
        bool areBothChainsMoving = enemyTankMovementController.AreBothChainsMoving();
        bool isLeftChainMoving = enemyTankMovementController.IsLeftChainMoving();
        bool isRightChainMoving = enemyTankMovementController.IsRightChainMoving();

        currentEnemyTankAnimator.SetBool("IsMovingBothChains", areBothChainsMoving);
        currentEnemyTankAnimator.SetBool("IsMovingLeftChain", isLeftChainMoving);
        currentEnemyTankAnimator.SetBool("IsMovingRightChain", isRightChainMoving);
    }
}
