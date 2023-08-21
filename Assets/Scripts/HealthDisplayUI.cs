using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayUI : MonoBehaviour
{
    [SerializeField] private Transform HeartOne; 
    [SerializeField] private Transform HeartTwo; 
    [SerializeField] private Transform HeartThree;

    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite halfHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    private HealthManager healthManager;

    public void Initialize(int maxLives)
    {
        UpdateHealthDisplay(maxLives); // Initialisieren mit sechs vollen Herzen
        Debug.Log("HealthDisplayUI initialized.");
    }

    public void UpdateHealthDisplay(int currentHealth)
    {
        int fullHearts = currentHealth / 2; // Volle Herzen
        int halfHeart = currentHealth % 2;   // Ein halbes Herz, falls notwendig

        SetHeartSprite(HeartOne, fullHearts >= 1, halfHeart > 0);
        SetHeartSprite(HeartTwo, fullHearts >= 2, halfHeart > 1);
        SetHeartSprite(HeartThree, fullHearts >= 3, halfHeart > 2);
    }

    private void SetHeartSprite(Transform heartTransform, bool isFull, bool isHalf)
    {
        Image heartImage = heartTransform.GetComponent<Image>();
        
        if (isFull)
        {
            heartImage.sprite = fullHeartSprite;
        }
        else if (isHalf)
        {
            heartImage.sprite = halfHeartSprite;
        }
        else
        {
            heartImage.sprite = emptyHeartSprite;
        }
    }
}
