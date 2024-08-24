using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LooterRacoonSpawner : MonoBehaviour
{

    public LooterRaccoon looterRaccoon;
    [SerializeField] Camera cameraMain;

    [SerializeField] LooterRaccoon newRaccoon;

    [SerializeField] List<LooterRaccoonPath> paths;

    [SerializeField] Button buttonSpawn;

    private void SpawnLooter()
    {
        //Instantiate(looterRaccoon, transform.position, Quaternion.identity).SetLooterPath(chosenPath)
        newRaccoon = Instantiate(looterRaccoon, transform.position, Quaternion.identity);
        newRaccoon.SetLooterPath(paths[(int)Random.Range(0, paths.Count)]);
        newRaccoon.cameraMain = cameraMain;
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
