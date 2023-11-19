using UnityEngine;

public class RandomSpriteSelector : MonoBehaviour
{
    [SerializeField] private SpriteHolder spriteHolder; 

    private void Start()
    {
        if (spriteHolder != null && spriteHolder.GetSpritesList().Count > 0)
        {
            int randomIndex = Random.Range(0, spriteHolder.GetSpritesList().Count);
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = spriteHolder.GetSpritesList()[randomIndex];
            }
        }
    }
}