using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }
}