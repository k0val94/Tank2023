using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayUI : MonoBehaviour
{
    public Image emptyHeartImage;
    public Image halfHeartImage;
    public Image fullHeartImage;

    public int maxHealth = 6; // Maximaler Lebenswert deines Spielers
    private int currentHealth = 6; // Aktueller Lebenswert deines Spielers

    private void Start()
    {
        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay()
    {
        int fullHearts = currentHealth / 2; // Volle Herzen
        int halfHearts = currentHealth % 2; // Halbvolle Herzen

        emptyHeartImage.gameObject.SetActive(false);
        halfHeartImage.gameObject.SetActive(false);
        fullHeartImage.gameObject.SetActive(false);

        for (int i = 0; i < fullHearts; i++)
        {
            fullHeartImage.gameObject.SetActive(true);
        }

        for (int i = 0; i < halfHearts; i++)
        {
            halfHeartImage.gameObject.SetActive(true);
        }
    }
}
