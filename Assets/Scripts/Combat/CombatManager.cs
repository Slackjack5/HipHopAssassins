using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static int enemyCount;

    private GameObject encounterEnemies;
    private GameObject enemyDictionary;
    private static PlayerScript ourPlayer;
    private bool enemiesSpawned;
    protected static State currentTurn;

    private static UIManager ourUI;
    private SpawnPoints ourSpawnPoints;
    private bool testStarted = false;
    
    protected enum State
    {
        Prefight,
        PlayerTurn,
        MonsterTurn,
        Defcon,
        Dialogue,
        Special,
    }
    // Start is called before the first frame update
    void Start()
    {
        encounterEnemies = gameObject.transform.GetChild(0).gameObject;
        enemyDictionary = gameObject.transform.GetChild(1).gameObject;
        ourUI = transform.parent.GetChild(0).GetComponent<UIManager>();
        ourSpawnPoints = gameObject.transform.GetChild(2).gameObject.GetComponent<SpawnPoints>();
        ourPlayer = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentTurn)
        {
            case State.Prefight:
                
                //Generate Encounter
                Debug.Log("In Pre-Fight Turn");
                SpawnEnemies(0, 3); //Spawn our enemies
                ChangeState(State.PlayerTurn);
                break;
            case State.PlayerTurn:
                Debug.Log("In Player Turn");
                break;
            case State.MonsterTurn:
                Debug.Log("In Monster Turn");
                if(!testStarted){StartCoroutine(WaitForNextTurn());
                    testStarted = true;
                }
                break;
            case State.Defcon:
                Debug.Log("In Defcon Turn");
                break;
        }

    }

    private void LateUpdate()
    {
        //Check How Many Enemies We Have On Screen
        enemyCount = encounterEnemies.transform.childCount;
    }

    public void SpawnEnemies(int id, int amount)
    {

            
            for (int i = 0; i < amount; i++)
            {
                GameObject temp = Instantiate(enemyDictionary.transform.GetChild(id).gameObject);
                temp.transform.position = ourSpawnPoints.getSpawnPoint(1, 0).transform.position;
                ourSpawnPoints.AssignHolder(temp);
                temp.transform.parent = encounterEnemies.transform;
                temp.SetActive(true);
                
            }
 
            //Debug.Log("Too Many Enemies On Screen");
            enemiesSpawned = true; 
    }

    public static void DamageMonsterHealth(MonsterData ourMonster, String DamageType, int damage)
    {
      
    }
    
    public static void HealPlayer(int healAmount)
    {
        ourPlayer.Health += healAmount;
        ourUI.HideAllMenus();
        ourUI.UpdateUI();
        //End Player Turn
        ChangeState(State.MonsterTurn);
    }
    public static void DamageMonsterLimb(MonsterData ourMonster, int limbNumber,int damage)
    {
        int newLimbNumber = (int) MathF.Abs(limbNumber);
        ourMonster.limbHealth[newLimbNumber] -= damage;
        ourUI.HideAllMenus();
        ourUI.UpdateUI();
        //End Player Turn
        ChangeState(State.MonsterTurn);

    }
    protected static void ChangeState(State state)
    {
        currentTurn = state;
    }
    
    IEnumerator WaitForNextTurn()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(2);
        print("Skipping Monster Turn " + Time.time);
        ourUI.RestartMenu();
        testStarted = false;
        ChangeState(State.PlayerTurn);
    }

}
