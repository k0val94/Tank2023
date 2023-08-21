using UnityEngine;
using UnityEngine.UI;

public class BottomBar : MonoBehaviour
{
    [Range(0, 100)]
    public float coveragePercentage = 17f;
    public float heartPlaceholderWidthPercentage = 10f; 

    [SerializeField] private Sprite heartFullSprite;  
    [SerializeField] private Sprite heartHalfSprite;  
    [SerializeField] private Sprite heartEmptySprite;

    public Vector2 FirstHeartPosition { get; private set; }
    public Vector2 SecondHeartPosition { get; private set; }
    public Vector2 ThirdHeartPosition { get; private set; }

    private RectTransform rectTransform;
    private RectTransform heartsPanelRectTransform; 
    private Image[] heartImages; 
    private Text playerNameText; 

    [SerializeField] private int maxLives = 6;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateSizeAndPosition();
        InitializeHeartPositions();
        InitializeUIElements();
        UpdateHeartsState(maxLives);
    }

    private void UpdateSizeAndPosition()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float barWidth = screenWidth;
        float barHeight = (coveragePercentage / 100f) * screenHeight;

        rectTransform.sizeDelta = new Vector2(barWidth, barHeight);

        float barYPosition = -(screenHeight * 0.5f) + (barHeight * 0.5f);
        rectTransform.anchoredPosition = new Vector2(0f, barYPosition);
    }

    private void InitializeHeartPositions()
    {
        float screenWidth = Screen.width;
        float heartPlaceholderWidth = (heartPlaceholderWidthPercentage / 100f) * screenWidth;
        float heartSpacing = heartPlaceholderWidth * 0.3f;

        FirstHeartPosition = new Vector2(0f, 0f);
        SecondHeartPosition = new Vector2(heartSpacing, 0f);
        ThirdHeartPosition = new Vector2(heartSpacing * 2, 0f);
    }

    private void InitializeUIElements()
    {
        CreateHeartsPanel();
        InitializeHeartImages();
    }

    private void CreateHeartsPanel()
    {
        GameObject panelObject = new GameObject("HeartsPanel");
        panelObject.transform.SetParent(transform);
        heartsPanelRectTransform = panelObject.AddComponent<RectTransform>();
        heartsPanelRectTransform.anchorMin = new Vector2(0f, 0.5f);
        heartsPanelRectTransform.anchorMax = new Vector2(0f, 0.5f);
        heartsPanelRectTransform.pivot = new Vector2(0f, 0.5f);
        heartsPanelRectTransform.anchoredPosition = new Vector2(0f, 0f);
    }

    private void InitializeHeartImages()
    {
        heartImages = new Image[3];
        for (int i = 0; i < heartImages.Length; i++)
        {
            GameObject heartObject = new GameObject("Heart" + (i + 1));
            heartObject.transform.SetParent(heartsPanelRectTransform);
            RectTransform heartRectTransform = heartObject.AddComponent<RectTransform>();
            heartRectTransform.anchoredPosition = GetHeartPosition(i);
            heartImages[i] = heartObject.AddComponent<Image>();
            heartImages[i].rectTransform.sizeDelta = new Vector2(50f, 50f); 
        }
    }

    private Vector2 GetHeartPosition(int index)
    {
        switch (index)
        {
            case 0:
                return FirstHeartPosition;
            case 1:
                return SecondHeartPosition;
            case 2:
                return ThirdHeartPosition;
            default:
                return Vector2.zero;
        }
    }

    public void UpdateHeartsState(int currentHealth)
    {
        int fullHearts = currentHealth / 2; 
        int halfHeart = currentHealth % 2;  

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (fullHearts > 0)
            {
                heartImages[i].sprite = heartFullSprite;
                fullHearts--;
            }
            else if (halfHeart > 0)
            {
                heartImages[i].sprite = heartHalfSprite;
                halfHeart--;
            }
            else
            {
                heartImages[i].sprite = heartEmptySprite;
            }
        }
    }
}
