using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "GameSettings", menuName = "Managers/Game Settings")]
public class GameSettingsSO : ScriptableObject
{

    public float money = 45f;

    public float damageDealt = 0f;

    [SerializeField] public GameStates currentGameState;
    [SerializeField] public GameStates previousGameState;
    [SerializeField] EventManagerSO eventManager;
    public Enemy enemy;

    public int enemiesSpawned = 0;
    public int enemiesDestroyed = 0;


    private void Awake()
    {
        currentGameState = GameStates.inMainMenu;
        previousGameState = currentGameState;
        enemiesSpawned = 7;
        enemiesDestroyed = 0;
    }




    public void ResetMoney()
    {
        money = 45f;
    }

    public void ResetDamageDealt()
    {
        damageDealt = 0f;
    }



}
