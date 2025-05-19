using System.Collections.Generic;
using TMPro;
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
    [SerializeField] Button acceptButton;
    [SerializeField] Button denyButton;

    [SerializeField] EventManagerSO eventManager;
    [SerializeField] LevelManager levelManager;

    private int randomIndex;
    public Sprite item;
    public string itemName;

    private void OnEnable()
    {
        eventManager.onRandomEvent += Randomize;
        acceptButton.onClick.AddListener(AcceptEvent);
        denyButton.onClick.AddListener(DenyEvent);
    }

    private void OnDisable()
    {
        eventManager.onRandomEvent -= Randomize;
        acceptButton.onClick.RemoveListener(AcceptEvent);
        denyButton.onClick.RemoveListener(DenyEvent);
    }

    public void Randomize()
    {
        Debug.Log("randomizing...");
        randomIndex = Random.Range(0, randomItems.Count);
        item = randomItems[randomIndex];
        itemName = randomItemNames[randomIndex];

        panel.GetComponent<Image>().sprite = item;
        randomItemName.text = "a " + itemName;

        string description = itemName switch
        {
            "Banana peel" => $"Slows down enemies for {levelManager.randomEventDuration} seconds!",
            "Cardboard box" => $"Homebase is immune to damage for {levelManager.randomEventDuration} seconds!",
            "Crushed can" => $"Raccoons shoot faster for {levelManager.randomEventDuration} seconds!",
            "Lavalamp" => $"Raccoons are distracted and can't defend for {levelManager.randomEventDuration} seconds!",
            "Moldy brownie" => $"Raccoons are sick and can't defend for {levelManager.randomEventDuration} seconds!",
            "Plastic knife" => $"Enemies' maximum health is reduced for {levelManager.randomEventDuration} seconds!",
            _ => ""
        };

        randomItemDescription.text = description;
        panel.SetActive(true);
    }

    public void AcceptEvent()
    {
        Debug.Log($"Accepted event: {itemName}");
        // Apply effects here
        panel.SetActive(false);
    }

    public void DenyEvent()
    {
        Debug.Log($"Denied event: {itemName}");
        panel.SetActive(false);
    }

}
