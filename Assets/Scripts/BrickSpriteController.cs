using UnityEngine;

public class BrickSpriterController : MonoBehaviour
{
    [SerializeField] private SpriteManager brickSpriteManager; 
    [SerializeField] private SpriteManager brickDamagedSpriteManager;
    
    private int selectedSpriteIndex = -1;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (brickSpriteManager != null && brickSpriteManager.GetSpritesList().Count > 0)
        {
            selectedSpriteIndex = Random.Range(0, brickSpriteManager.GetSpritesList().Count);
            spriteRenderer.sprite = brickSpriteManager.GetSpritesList()[selectedSpriteIndex];
        }
    }

    public void UpdateDamagedSprite()
    {
        if (selectedSpriteIndex != -1 && brickDamagedSpriteManager != null)
        {
            spriteRenderer.sprite = brickDamagedSpriteManager.GetSpritesList()[selectedSpriteIndex];
        }
    }
}