using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float damage = 40f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            BrickHealth brickHealth = collision.gameObject.GetComponent<BrickHealth>();
            if (brickHealth != null)
            {
                brickHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Steel"))
        {
            SteelHealth steelHealth = collision.gameObject.GetComponent<SteelHealth>();
            if (steelHealth != null)
            {
                steelHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
