using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomebaseHealthBar : MonoBehaviour
{
    // foreground UI element
    [SerializeField] Image healthBarSprite;

    public void UpdateHomebaseHealthBar(float currentHomebaseHealth, float homebaseHealth)
    {
        healthBarSprite.fillAmount = currentHomebaseHealth / homebaseHealth;
    }





}
