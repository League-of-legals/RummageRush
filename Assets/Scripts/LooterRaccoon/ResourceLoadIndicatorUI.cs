using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceLoadIndicatorUI : MonoBehaviour
{
    [SerializeField] TMP_Text resourceLoadText;
    [SerializeField] LooterRaccoon looterRaccoon;

    private void Start()
    {
        looterRaccoon = GetComponentInParent<LooterRaccoon>();
    }

    public void UpdateLoadText(string moneyText)
    {
        resourceLoadText.text = moneyText;
    }
}
