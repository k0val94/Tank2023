using UnityEngine;

public class BrickSpriterController : MonoBehaviour
{
    [SerializeField] private SpriteHolder brickSpriteHolder; 
    [SerializeField] private SpriteHolder brickDamagedSpriteHolder;
    
    private int selectedSpriteIndex = -1;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (brickSpriteHolder != null && brickSpriteHolder.GetSpritesList().Count > 0)
        {
            selectedSpriteIndex = Random.Range(0, brickSpriteHolder.GetSpritesList().Count);
            spriteRenderer.sprite = brickSpriteHolder.GetSpritesList()[selectedSpriteIndex];
        }
    }

    public void UpdateDamagedSprite()
    {
        if (selectedSpriteIndex != -1 && brickDamagedSpriteHolder != null)
        {
            spriteRenderer.sprite = brickDamagedSpriteHolder.GetSpritesList()[selectedSpriteIndex];
        }
    }
}