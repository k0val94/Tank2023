using UnityEngine;

public class BrickHealth : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    private bool isDamaged = false;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
        else if (health <= 50f && !isDamaged)
        {
            isDamaged = true;
            GetComponent<BrickSpriterController>().UpdateDamagedSprite();
        }
    }
}