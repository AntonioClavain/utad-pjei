using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    public Health healthComponent;

    public Image fillbar;

    void Start()
    {
        if(healthComponent != null)
        {
            healthComponent.healthDelegate += RefreshHealthBar;
        }
    }

    public void RefreshHealthBar(float newHealth)
    {
        fillbar.fillAmount = newHealth;
    }
}
