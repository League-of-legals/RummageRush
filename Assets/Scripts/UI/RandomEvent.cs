using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RandomEvent : MonoBehaviour
{
    [SerializeField] List<Sprite> randomItems;
    [SerializeField] List<string> randomItemNames;
    [SerializeField] TMP_Text randomItemName;
    [SerializeField] TMP_Text randomItemDescription;
    [SerializeField] GameObject panel;

    [SerializeField] EventManagerSO eventManager;
    [SerializeField] LevelManager levelManager;

    private int randomIndex;
    public Sprite item;
    public string itemName;

    private void OnEnable()
    {
        eventManager.onRandomEvent += Randomize;
    }

    private void OnDisable()
    {
        eventManager.onRandomEvent -= Randomize;
    }
    public void Randomize()
    {
        Debug.Log($"randomizing...");
        randomIndex = Random.Range(0, randomItems.Count);
        item = randomItems[randomIndex];
        itemName = randomItemNames[randomIndex];

        panel.GetComponent<Image>().sprite = item;
        randomItemName.text = "a " + itemName;

        if (itemName == "Banana peel")
        {
            randomItemDescription.text =
                $"Slows down enemies for {levelManager.randomEventDuration} seconds!";

        }

        else if (itemName == "Cardboard box")
        {
            randomItemDescription.text =
                $"Homebase is immune to damage for {levelManager.randomEventDuration} seconds!";
        }

        else if (itemName == "Crushed can")
        {
            randomItemDescription.text =
                $"Raccoons shoot faster for {levelManager.randomEventDuration} seconds!";

        }

        else if (itemName == "Lavalamp")
        {
            randomItemDescription.text =
                $"Raccoons are distracted and can't defend for {levelManager.randomEventDuration} seconds!";

        }

        else if (itemName == "Moldy brownie")
        {
            randomItemDescription.text =
                $"Raccoons are sick and can't defend for {levelManager.randomEventDuration} seconds!";

        }

        else if (itemName == "Plastic knife")
        {
            randomItemDescription.text =
                $"For {levelManager.randomEventDuration} seconds enemies' maximum health is reduced!";


        }
    }
}
