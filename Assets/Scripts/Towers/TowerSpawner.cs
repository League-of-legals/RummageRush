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
    // Types of towers
    [SerializeField] Tower towerDefault;
    [SerializeField] Tower towerHeavy;
    [SerializeField] Tower towerFast;

    // grid related
    [Header("Grid:")]
    [SerializeField] Tilemap tilemap;
    private Vector3Int cellPosition;


    private bool spawnerIsActive;

    private Vector3 mousePosition;

    [Header("References:")]
    [SerializeField] GameSettingsSO gameSettings;
    [SerializeField] HUDmanager hudManager;
    [SerializeField] TMP_Text status;

    //buttons
    [Header("Buttons:")]
    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Button button3;

    //towers spawned list
    [Header("List of towers:")]
    [SerializeField] public List<Tower> towers;
    public bool towersInScene = false;

    private void Awake()
    {
        // Intial setup
        spawnerIsActive = false;
        towerIndicator = null;
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (gameSettings.currentGameState == GameStates.inGame)
        {         

            // create a tower with a button press
            if (Input.GetKeyUp(KeyCode.Alpha1) && !spawnerIsActive && gameSettings.money >= towerDefault.towerCost)
            {
                StartTowerPlacement(towerDefault);
            }
            if (Input.GetKeyUp(KeyCode.Alpha2) && !spawnerIsActive && gameSettings.money >= towerFast.towerCost)
            {
                StartTowerPlacement(towerFast);
            }
            if (Input.GetKeyUp(KeyCode.Alpha3) && !spawnerIsActive && gameSettings.money >= towerHeavy.towerCost)
            {
                StartTowerPlacement(towerHeavy);
            }

            if (Input.GetKeyUp(KeyCode.Alpha1) && !spawnerIsActive && gameSettings.money <= towerDefault.towerCost)
            {
                Debug.Log($"Not enough money!");
                StartCoroutine(StatusUpdate());
            }

            if (Input.GetKeyUp(KeyCode.Alpha2) && !spawnerIsActive && gameSettings.money <= towerFast.towerCost)
            {
                Debug.Log($"Not enough money!");
                StartCoroutine(StatusUpdate());
            }

            if (Input.GetKeyUp(KeyCode.Alpha3) && !spawnerIsActive && gameSettings.money <= towerHeavy.towerCost)
            {
                Debug.Log($"Not enough money!");
                StartCoroutine(StatusUpdate());
            }


            // place the tower
            if (spawnerIsActive && hudManager.randomEventScreen.activeInHierarchy == false)
            {
                towerIndicator.transform.position = GetMousePosition();

                // drop the tower
                if (Input.GetMouseButton(0))
                {
                    towerIndicator.activateTower();
                    towers.Add(towerIndicator);
                    towerIndicator = null;
                    spawnerIsActive = false;
                }

                // cancel placement
                else if (Input.GetMouseButton(1))
                {
                    gameSettings.money += towerIndicator.towerCost;
                    hudManager.UpdateMoneyText();
                    Destroy(towerIndicator.gameObject);
                    spawnerIsActive = false;
                }


            }

            if (towers.Count > 0)
            {
                towersInScene = true;
            }
            else
            {
                towersInScene = false;
            }

        }

    }

    private void OnEnable()
    {
        button1.onClick.AddListener(delegate { StartTowerPlacement(towerDefault); });
        button2.onClick.AddListener(delegate { StartTowerPlacement(towerFast); });
        button3.onClick.AddListener(delegate { StartTowerPlacement(towerHeavy); });
    }
    private void OnDisable()
    {
        button1.onClick.RemoveListener(delegate { StartTowerPlacement(towerDefault); });
        button2.onClick.RemoveListener(delegate { StartTowerPlacement(towerFast); });
        button3.onClick.RemoveListener(delegate { StartTowerPlacement(towerHeavy); });
    }

    private Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Fire the 'laser' 
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 200f, groundLayers))
        {

            // We hit something! Yippee!
            Debug.DrawLine(Camera.main.transform.position, hit.point,
                Color.red);

            // convert hit point to a position on the grid
            cellPosition = tilemap.LocalToCell(hit.point);

            return new Vector3(
                cellPosition.x + tilemap.cellSize.x / 2f,    // x
                0,                                           // y
                cellPosition.y + tilemap.cellSize.y / 2f    // z
               );

        }

        return Vector3.zero;

    }

    private void StartTowerPlacement(Tower newTower)
    {
        if (!spawnerIsActive && gameSettings.money >= newTower.towerCost)
        {
            Debug.Log($"Purchased tower");
            towerIndicator = Instantiate(newTower, mousePosition, Quaternion.identity);
            towerIndicator.towerSpawner = this;
            spawnerIsActive = true;
            gameSettings.money -= newTower.towerCost;
            hudManager.UpdateMoneyText();
        }

        else
        {
            Debug.Log($"Not enough money!");
            StartCoroutine(StatusUpdate());
        }
           
    } 

    IEnumerator StatusUpdate()
    {
        status.text = $"Not enough money!";
        yield return new WaitForSeconds(3);
        status.text = $" ";
    }

}
