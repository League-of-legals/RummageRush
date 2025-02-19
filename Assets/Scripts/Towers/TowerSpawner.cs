using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TowerSpawner : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField] LayerMask groundLayers;
    private Tower towerIndicator;

    [Header("Towers user can buy:")]
    [SerializeField] Tower towerDefault;
    [SerializeField] Tower towerHeavy;
    [SerializeField] Tower towerFast;

    [Header("Grid:")]
    [SerializeField] Tilemap tilemap;
    private Vector3Int cellPosition;

    private bool spawnerIsActive;
    private Vector3 mousePosition;

    [Header("References:")]
    [SerializeField] GameSettingsSO gameSettings;
    [SerializeField] HUDmanager hudManager;
    [SerializeField] TMP_Text status;

    [Header("Buttons:")]
    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Button button3;

    [Header("List of towers:")]
    [SerializeField] public List<Tower> towersInScene;

    [Header("Placing Effect:")]
    [SerializeField] private GameObject TowerPlacingEffect;

    [Header("Placement Zone:")]
    [SerializeField] private GameObject placementZoneEffect;
    [SerializeField] private float gridSize = 1f; // Grid size for snapping

    private void Awake()
    {
        spawnerIsActive = false;
        towerIndicator = null;

        if (placementZoneEffect != null)
        {
            placementZoneEffect.SetActive(false); // Hide zone at start
        }

    }

    private void Update()
    {
        if (gameSettings.currentGameState == GameStates.inGame)
        {
            HandleTowerPlacement();
        }

    }

    private void HandleTowerPlacement()
    {
        if (spawnerIsActive)
        {
            if (!hudManager.randomEventScreen.activeInHierarchy)
            {
                Vector3 newPosition = GetMousePosition();
                newPosition = SnapToGrid(newPosition);

                towerIndicator.transform.position = newPosition;

                if (placementZoneEffect != null)
                {
                    placementZoneEffect.SetActive(true);
                   
                }

                if (Input.GetMouseButtonDown(0))
                {
                    towerIndicator.activateTower();
                    towersInScene.Add(towerIndicator);
                    towerIndicator = null;
                    spawnerIsActive = false;
                    placementZoneEffect.SetActive(false);
                    TowerPlacingEffect.SetActive(false);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    CancelTowerPlacement();
                }
            }
            else
            {
                CancelTowerPlacement();
            }
        }
    }

    private void CancelTowerPlacement()
    {
        gameSettings.money += towerIndicator.towerCost;
        hudManager.UpdateMoneyText();
        Destroy(towerIndicator.gameObject);
        spawnerIsActive = false;
        placementZoneEffect.SetActive(false);
        TowerPlacingEffect.SetActive(false);
    }

    private void OnEnable()
    {
        button1.onClick.AddListener(() => StartTowerPlacement(towerDefault));
        button2.onClick.AddListener(() => StartTowerPlacement(towerFast));
        button3.onClick.AddListener(() => StartTowerPlacement(towerHeavy));
    }

    private void OnDisable()
    {
        button1.onClick.RemoveListener(() => StartTowerPlacement(towerDefault));
        button2.onClick.RemoveListener(() => StartTowerPlacement(towerFast));
        button3.onClick.RemoveListener(() => StartTowerPlacement(towerHeavy));
    }

    private Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200f, groundLayers))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);

            Vector3 worldPosition = SnapToGrid(hit.point);

            if (TowerPlacingEffect != null)
            {
                TowerPlacingEffect.SetActive(true);
            }

            if (placementZoneEffect != null)
            {
                placementZoneEffect.SetActive(true);
            }

            return worldPosition;
        }

        if (TowerPlacingEffect != null)
        {
            TowerPlacingEffect.SetActive(false);
        }

        if (placementZoneEffect != null)
        {
            placementZoneEffect.SetActive(false);
        }

        return Vector3.zero;
    }

    private void StartTowerPlacement(Tower newTower)
    {
        if (!spawnerIsActive && gameSettings.money >= newTower.towerCost)
        {
            Debug.Log("Purchased tower");
            mousePosition = GetMousePosition();
            mousePosition = SnapToGrid(mousePosition);
            towerIndicator = Instantiate(newTower, mousePosition, Quaternion.identity);
            towerIndicator.towerSpawner = this;
            spawnerIsActive = true;
            gameSettings.money -= newTower.towerCost;
            hudManager.UpdateMoneyText();

            if (placementZoneEffect != null)
            {
                placementZoneEffect.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Not enough money!");
            StartCoroutine(StatusUpdate());
        }
    }

    IEnumerator StatusUpdate()
    {
        status.text = "Not enough money!";
        yield return new WaitForSeconds(3);
        status.text = "";
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Floor(position.x / gridSize) * gridSize + gridSize / 2f;
        float z = Mathf.Floor(position.z / gridSize) * gridSize + gridSize / 2f;
        return new Vector3(x, position.y, z);
    }


}
