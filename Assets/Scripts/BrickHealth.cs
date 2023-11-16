using UnityEngine;

public class BrickHealth : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
