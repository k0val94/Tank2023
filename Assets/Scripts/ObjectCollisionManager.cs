using UnityEngine;

public class ObjectCollisionManager : MonoBehaviour
{
    void Start()
    {
        int projectileLayer = LayerMask.NameToLayer("Projectile");
        int waterLayer = LayerMask.NameToLayer("Water");
        Physics2D.IgnoreLayerCollision(projectileLayer, waterLayer, true);
    }
}