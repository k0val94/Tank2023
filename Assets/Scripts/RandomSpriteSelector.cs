using UnityEngine;

public class RandomSpriteSelector : MonoBehaviour
{
    public SpriteManager spriteManager; // Ziehe das SpriteManagerPrefab hierhin

    private void Start()
    {
        if (spriteManager != null && spriteManager.spritesList.Count > 0)
        {
            int randomIndex = Random.Range(0, spriteManager.spritesList.Count);
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = spriteManager.spritesList[randomIndex];
            }
        }
    }
}