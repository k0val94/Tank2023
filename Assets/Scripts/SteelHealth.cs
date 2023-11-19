using UnityEngine;

public class SteelHealth : MonoBehaviour
{
    [SerializeField] private float health = 130f;
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
            GetComponent<SteelSpriterController>().UpdateDamagedSprite();
        }
    }
}