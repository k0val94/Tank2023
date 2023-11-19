using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    public Gradient gradient;
    public Image fill;

    void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("Slider component not found on HealthBar GameObject in Awake().");
            return; 
        }

        if (gradient == null)
        {
            Debug.LogError("Gradient has not been assigned in the HealthBar script.");
            return; 
        }

        if (fill == null)
        {
            Debug.LogError("Fill Image has not been assigned in the HealthBar script.");
            return;
        }
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform component not found on the HealthBar GameObject.");
            return; 
        }
        
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);
        rectTransform.anchoredPosition = new Vector2(10, 10); 
        rectTransform.sizeDelta = new Vector2(200, 30); 
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
	{
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}