using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LooterRaccoonSpawner : MonoBehaviour
{

    public LooterRaccoon looterRaccoon;
    [SerializeField] Camera cameraMain;

    [SerializeField] LooterRaccoon newRaccoon;
    [SerializeField] public List<LooterRaccoon> LootersInScene;

    [SerializeField] List<LooterRaccoonPath> paths;



    [SerializeField] Button buttonSpawn;
    [SerializeField] GameObject resourcePool;
    [SerializeField] GameObject homebase;

    [SerializeField] HUDmanager hudManager;
    [SerializeField] GameSettingsSO gameSettings;

    private void SpawnLooter()
    {
        if (gameSettings.currentGameState == GameStates.inGame)
        {
            if (gameSettings.money >= gameSettings.looterCurrentPrice)
            {
                newRaccoon = Instantiate(looterRaccoon, transform.position, Quaternion.identity);
                newRaccoon.SetLooterPath(paths[(int)Random.Range(0, paths.Count)]);
                newRaccoon.cameraMain = cameraMain;
                newRaccoon.hudManager = hudManager;
                newRaccoon.resourcePool = resourcePool;
                newRaccoon.homebase = homebase;
                newRaccoon.gameSettings = gameSettings;
                newRaccoon.looterRaccoonSpawner = this;
                LootersInScene.Add(newRaccoon);
                gameSettings.looterCurrentPrice = gameSettings.looterDefaultPrice + 
                    (LootersInScene.Count * gameSettings.looterPriceModifier);

            }
        }
    }

    private void Start()
    {
        SpawnLooter();
    }

    private void OnEnable()
    {
        buttonSpawn.onClick.AddListener(delegate { SpawnLooter(); }) ;
    }


}
