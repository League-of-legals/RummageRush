using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // foreground UI element
    [SerializeField] Image healthBarSprite;

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBarSprite.fillAmount = currentHealth / maxHealth;
    }





}
