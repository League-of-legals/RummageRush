using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LooterRacoonSpawner : MonoBehaviour
{

    [SerializeField] LooterRaccoon looterRaccoon;

    [SerializeField] List<LooterRaccoonPath> paths;

    [SerializeField] Button buttonSpawn;

    private void SpawnLooter(LooterRaccoonPath chosenPath)
    {
        Instantiate(looterRaccoon, transform.position, Quaternion.identity).SetLooterPath(chosenPath);
    }

    private void Start()
    {
        SpawnLooter(paths[(int)Random.Range(0, paths.Count)]);
    }

    private void OnEnable()
    {
        buttonSpawn.onClick.AddListener(delegate { SpawnLooter(paths[(int)Random.Range(0, paths.Count)]); }) ;
    }

}
