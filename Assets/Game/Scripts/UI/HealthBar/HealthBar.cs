using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public float showDelayFactor = 0.5f;

    private float lastValue;

    private int showDelayAmmount = 0;

    private void Awake()
    {
        transform.localScale = new Vector2(0, 0);

        lastValue = slider.value;
    }

    private void Update()
    {
        if (slider.value > 0)
        {
            if (lastValue != slider.value)
            {
                lastValue = slider.value;

                StartCoroutine(ShowDelay());
            }
            else
            {
                if (showDelayAmmount <= 0)
                {
                    transform.localScale = new Vector2(0, 0);
                }
            }

        } else
        {
            transform.localScale = new Vector2(0, 0);
        }
    }

    public void SetValue(float value)
    { 
        slider.value = value;
    }

    public IEnumerator ShowDelay()
    {
        showDelayAmmount++;

        transform.localScale = new Vector2(1, 1);

        float value = 1f;

        while (value > 0)
        {
            value -= Time.deltaTime * showDelayFactor;

            yield return null;
        }

        showDelayAmmount--;
    }

    public void SetMaxValue(float maxValue)
    {
        slider.maxValue = maxValue;
    }
}
