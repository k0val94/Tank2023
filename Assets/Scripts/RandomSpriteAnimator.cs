using UnityEngine;

public class RandomSpriteAnimator : MonoBehaviour
{
    [SerializeField] private SpriteHolder spriteHolder;
    [SerializeField] private float frameRate = 0.2f; // Zeit zwischen den Frame-Wechseln

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("UpdateSprite", 0f, frameRate);
    }

    private void UpdateSprite()
    {
        if (spriteHolder != null)
        {
            Sprite[] sprites = spriteHolder.GetSpritesList().ToArray();

            if (sprites.Length > 0)
            {
                int randomIndex = Random.Range(0, sprites.Length);
                spriteRenderer.sprite = sprites[randomIndex];
            }
        }
    }
}