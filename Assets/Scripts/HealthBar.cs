using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;

 	void Start()
    {

        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("HealthBar script is not attached to an object with a Slider component.");
        }

		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2(0, 0);
		rectTransform.anchorMax = new Vector2(0, 0);
		rectTransform.pivot = new Vector2(0, 0);
		rectTransform.anchoredPosition = new Vector2(10, 10); // Anpassen, wie benötigt
		rectTransform.sizeDelta = new Vector2(200, 30); // Setzen Sie die gewünschte Größe

    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

}
