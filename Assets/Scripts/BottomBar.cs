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

    private RectTransform rectTransform;
    private RectTransform heartsPanelRectTransform;
    private Image[] heartImages;

    private Vector2[] heartPositions = new Vector2[3];

    [SerializeField] private int maxLives = 6;

    private void Start()
    {
        InitializeUIElements();
        UpdateHeartsState(maxLives); // Assuming you have defined maxLives somewhere
    }

    private void InitializeUIElements()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateSizeAndPosition();
        InitializeHeartPositions();
        CreateHeartsPanel();
        InitializeHeartImages();
    }

    private void UpdateSizeAndPosition()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float barHeight = (coveragePercentage / 100f) * screenHeight;
        float barWidth = screenWidth * 0.15f; // Adjust as needed

        rectTransform.sizeDelta = new Vector2(barWidth, barHeight);

        float barXPosition = (screenWidth * 0.5f) - (barWidth * 0.5f);
        float barYPosition = -(screenHeight * 0.5f) + (barHeight * 0.5f);
        rectTransform.anchoredPosition = new Vector2(barXPosition, barYPosition);
    }

    private void InitializeHeartPositions()
    {
        float screenWidth = Screen.width;
        float heartPlaceholderWidth = (heartPlaceholderWidthPercentage / 100f) * screenWidth;
        float heartSpacing = heartPlaceholderWidth * 0.3f;

        for (int i = 0; i < heartPositions.Length; i++)
        {
            heartPositions[i] = new Vector2(i * heartSpacing, 0f);
        }
    }

    private void CreateHeartsPanel()
    {
        GameObject panelObject = new GameObject("HeartsPanel");
        panelObject.transform.SetParent(transform);
        heartsPanelRectTransform = panelObject.AddComponent<RectTransform>();
        heartsPanelRectTransform.anchorMin = new Vector2(0f, 0.5f);
        heartsPanelRectTransform.anchorMax = new Vector2(0f, 0.5f);
        heartsPanelRectTransform.pivot = new Vector2(0f, 0.5f);
        heartsPanelRectTransform.anchoredPosition = Vector2.zero;
    }

    private void InitializeHeartImages()
    {
        heartImages = new Image[3];
        for (int i = 0; i < heartImages.Length; i++)
        {
            GameObject heartObject = new GameObject("Heart" + (i + 1));
            heartObject.transform.SetParent(heartsPanelRectTransform);
            RectTransform heartRectTransform = heartObject.AddComponent<RectTransform>();
            heartRectTransform.anchoredPosition = heartPositions[i];
            heartImages[i] = heartObject.AddComponent<Image>();
            heartImages[i].rectTransform.sizeDelta = new Vector2(50f, 50f);
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
