using UnityEngine;

public class RandomSpriteSelector : MonoBehaviour
{
    [SerializeField] private SpriteManager spriteManager; 

    private void Start()
    {
        if (spriteManager != null && spriteManager.GetSpritesList().Count > 0)
        {
            int randomIndex = Random.Range(0, spriteManager.GetSpritesList().Count);
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = spriteManager.GetSpritesList()[randomIndex];
            }
        }
    }
}