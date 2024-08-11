using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooterRacoonSpawner : MonoBehaviour
{
    [SerializeField] LooterRaccoon looterRaccoon;

    [SerializeField] List<LooterRaccoonPath> paths;

    private void SpawnLooter(LooterRaccoonPath chosenPath)
    {
        Instantiate(looterRaccoon, transform.position, Quaternion.identity).SetLooterPath(chosenPath);
    }

    private void Start()
    {
        SpawnLooter(paths[(int)Random.Range(0, paths.Count)]);
    }
}
