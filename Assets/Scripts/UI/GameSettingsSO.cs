using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "GameSettings", menuName = "Managers/Game Settings")]
public class GameSettingsSO : ScriptableObject
{
    [SerializeField] public GameStates currentGameState;
    [SerializeField] public GameStates previousGameState;
    [SerializeField] public LevelStates currentLevel;
    [SerializeField] public LevelStates previousLevel;

    public float money = 45f;

    public float damageDealt = 0f;
    public int enemiesSpawned = 0;
    public int enemiesDestroyed = 0;

    public float looterDefaultPrice = 30f;
    public float looterCurrentPrice = 30f;
    [SerializeField] public float looterPriceModifier = 15f;

    [Header("References:")]
    [SerializeField] EventManagerSO eventManager;
    public Enemy enemy;

    private void Awake()
    {
        currentGameState = GameStates.inMainMenu;
        previousGameState = currentGameState;
        enemiesSpawned = 7;
        enemiesDestroyed = 0;
    }




    public void ResetMoney()
    {
        money = 60f;
    }

    public void ResetDamageDealt()
    {
        damageDealt = 0f;
    }



}
