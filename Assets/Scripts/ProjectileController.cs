using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public TankController tankController;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
        }

        tankController.canShoot = true;
        Destroy(gameObject);
    }
}