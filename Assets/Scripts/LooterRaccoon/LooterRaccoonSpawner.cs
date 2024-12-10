using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LooterRaccoonSpawner : MonoBehaviour
{
    [SerializeField] List<LooterRaccoonPath> paths;

    public LooterRaccoon looterRaccoon;

    [SerializeField] LooterRaccoon newRaccoon;
    [SerializeField] public List<LooterRaccoon> LootersInScene;


    [Header("Cooldown:")]
    [SerializeField] bool onCooldown;
    [SerializeField] float cooldownTimer;
    [SerializeField] float cooldownTime = 5f;

    [Header("References:")]
    [SerializeField] Button buttonSpawn;
    [SerializeField] Button buttonRecall;
    [SerializeField] Button buttonSendOut;
    [SerializeField] GameObject resourcePool;
    [SerializeField] GameObject homebase;
    [SerializeField] Camera cameraMain;
    [SerializeField] HUDmanager hudManager;
    [SerializeField] GameSettingsSO gameSettings;

    private void SpawnLooter() //spawn a Looter Raccoon on given conditions
    {
        if (gameSettings.currentGameState == GameStates.inGame)
        { if (LootersInScene.Count == 0f)
            { gameSettings.looterCurrentPrice = 0; }
               

           if (!onCooldown)
                {
                cooldownTimer = 0;
                newRaccoon = Instantiate(looterRaccoon, transform.position, Quaternion.identity);
                newRaccoon.SetLooterPath(paths[(int)Random.Range(0, paths.Count)]);
                newRaccoon.cameraMain = cameraMain;
                newRaccoon.hudManager = hudManager;
                newRaccoon.resourcePool = resourcePool;
                newRaccoon.homebase = homebase;
                newRaccoon.gameSettings = gameSettings;
                newRaccoon.looterRaccoonSpawner = this;
                LootersInScene.Add(newRaccoon);
                gameSettings.money -= gameSettings.looterCurrentPrice;
                {gameSettings.looterCurrentPrice = gameSettings.looterDefaultPrice + 
                    (LootersInScene.Count * gameSettings.looterPriceModifier); }

                     
                

            }
        }
    }

    private void Start()
    {
        cooldownTimer = 0;
        SpawnLooter();
    }

    private void Update()
    {
        //cooldown timer and button interactivity
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= cooldownTime && gameSettings.money >= gameSettings.looterCurrentPrice)
        {
            onCooldown = false;
            buttonSpawn.GetComponent<Button>().interactable = true;
        }
        else { onCooldown = true;
            buttonSpawn.GetComponent<Button>().interactable = false;
        } 

    }

    private void OnEnable()
    {
        buttonSpawn.onClick.AddListener(delegate { SpawnLooter(); }) ;
        buttonRecall.onClick.AddListener(delegate { RecallLooters(); });
        buttonSendOut.onClick.AddListener(delegate { SendOutLooters(); });

    }

    private void RecallLooters()
    {
        foreach (LooterRaccoon looter in LootersInScene)
            looter.recalled = true;
    }

    private void SendOutLooters()
    {
        foreach (LooterRaccoon looter in LootersInScene)
            looter.recalled = false;
    }
}
