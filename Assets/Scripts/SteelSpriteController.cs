using UnityEngine;

public class SteelSpriterController : MonoBehaviour
{
    [SerializeField] private SpriteHolder steelSpriteHolder; 
    [SerializeField] private SpriteHolder steelDamagedSpriteHolder;
    
    private int selectedSpriteIndex = -1;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (steelSpriteHolder != null && steelSpriteHolder.GetSpritesList().Count > 0)
        {
            selectedSpriteIndex = Random.Range(0, steelSpriteHolder.GetSpritesList().Count);
            spriteRenderer.sprite = steelSpriteHolder.GetSpritesList()[selectedSpriteIndex];
        }
    }

    public void UpdateDamagedSprite()
    {
        if (selectedSpriteIndex != -1 && steelDamagedSpriteHolder != null)
        {
            spriteRenderer.sprite = steelDamagedSpriteHolder.GetSpritesList()[selectedSpriteIndex];
        }
    }
}