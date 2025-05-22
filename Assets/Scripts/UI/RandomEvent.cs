using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RandomEvent : MonoBehaviour
{
    [SerializeField] List<Sprite> randomItems;
    [SerializeField] List<string> randomItemNames;

    [Header("UI Panels")]
    [SerializeField] public GameObject offerPanel;
    [SerializeField] public GameObject itemRevealPanel;

    [Header("UI Elements")]
    [SerializeField] Image showItemImage;
    [SerializeField] TMP_Text randomItemName;
    [SerializeField] TMP_Text randomItemDescription;
    [SerializeField] Button acceptButton;
    [SerializeField] Button denyButton;

    [SerializeField] EventManagerSO eventManager;
    [SerializeField] LevelManager levelManager;

    private int randomIndex;
    public Sprite item;
    public string itemName;

    private void Awake()
    {
        offerPanel.SetActive(false);
        itemRevealPanel.SetActive(false);
    }

    private void OnEnable()
    {
        eventManager.onRandomEvent += ShowOffer;
        acceptButton.onClick.AddListener(OnAccept);
        denyButton.onClick.AddListener(OnDeny);
    }

    private void OnDisable()
    {
        eventManager.onRandomEvent -= ShowOffer;
        acceptButton.onClick.RemoveListener(OnAccept);
        denyButton.onClick.RemoveListener(OnDeny);
        itemRevealPanel?.SetActive(false);
    }

    public void ShowOffer()
    {
        Debug.Log("Showing random item offer...");

        if (randomItems.Count == 0 || randomItemNames.Count == 0)
        {
            Debug.LogError("Random items list is empty!");
            return;
        }

        if (randomItems.Count != randomItemNames.Count)
        {
            Debug.LogError("randomItems and randomItemNames must be the same length!");
            return;
        }

        randomIndex = Random.Range(0, randomItems.Count);
        item = randomItems[randomIndex];
        itemName = randomItemNames[randomIndex];

        showItemImage.sprite = item;
        Debug.Log(offerPanel);
        offerPanel.gameObject.SetActive(true);
    }

    public void OnAccept()
    {
        Debug.Log($"Accepted item: {itemName}");
        offerPanel.SetActive(false);
        ShowItemDescription();
        Debug.Log(itemRevealPanel);
        itemRevealPanel.SetActive(true);
    }

    public void OnDeny()
    {
        Debug.Log($"Denied item: {itemName}");
        item = null;
        itemName = null;
        offerPanel.SetActive(false);
        itemRevealPanel.SetActive(false);
        eventManager.ResumeGame(); // Resume gameplay
    }

    private void ShowItemDescription()
    {
        randomItemName.text = itemName;

        string description = itemName switch
        {
            "Banana peel" => $"Slows down enemies for {levelManager.randomEventDuration} seconds!",
            "Cardboard box" => $"Homebase is immune to damage for {levelManager.randomEventDuration} seconds!",
            "Crushed can" => $"Raccoons shoot faster for {levelManager.randomEventDuration} seconds!",
            "Lavalamp" => $"Raccoons are distracted and can't defend for {levelManager.randomEventDuration} seconds!",
            "Moldy brownie" => $"Raccoons are sick and can't defend for {levelManager.randomEventDuration} seconds!",
            "Plastic knife" => $"Enemies' maximum health is reduced for {levelManager.randomEventDuration} seconds!",
            _ => "No description available."
        };

        randomItemDescription.text = description;
    }
}
