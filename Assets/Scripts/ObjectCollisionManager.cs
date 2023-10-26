using UnityEngine;

public class ObjectCollisionManager : MonoBehaviour
{
    void Start()
    {
        int enemyProjectileLayer = LayerMask.NameToLayer("EnemyProjectile");
        int playerProjectileLayer = LayerMask.NameToLayer("PlayerProjectile");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");
        int waterLayer = LayerMask.NameToLayer("Water");
        int groundLayer = LayerMask.NameToLayer("GroundLayer");

        Physics2D.IgnoreLayerCollision(enemyProjectileLayer, waterLayer, true);
        Physics2D.IgnoreLayerCollision(playerProjectileLayer, waterLayer, true);
        Physics2D.IgnoreLayerCollision(enemyProjectileLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(playerProjectileLayer, playerLayer, true);
        Physics2D.IgnoreLayerCollision(enemyProjectileLayer, groundLayer, true);
        Physics2D.IgnoreLayerCollision(playerProjectileLayer, groundLayer, true);
        Physics2D.IgnoreLayerCollision(enemyLayer, groundLayer, true);
        Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
    }
}