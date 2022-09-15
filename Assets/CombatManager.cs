using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static int enemyCount;

    private GameObject encounterEnemies;
    private GameObject enemyDictionary;
    private bool enemiesSpawned;
    protected static State currentTurn;

    private static UIManager ourUI;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentTurn)
        {
            case State.Prefight:
                
                //Generate Encounter
                Debug.Log("In Pre-Fight Turn");
                if (!enemiesSpawned) { SpawnEnemies(0);} //Spawn our enemies
                enemyCount = encounterEnemies.transform.childCount;
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

    public void SpawnEnemies(int id)
    {
        GameObject temp = Instantiate(enemyDictionary.transform.GetChild(id).gameObject);
        temp.transform.parent = encounterEnemies.transform;
        temp.SetActive(true);
        enemiesSpawned = true; 
    }

    public static void DamageMonsterHealth(MonsterData ourMonster)
    {
      
    }
    public static void DamageMonsterLimb(MonsterData ourMonster, int limbNumber,int damage)
    {
        ourMonster.limbHealth[limbNumber] -= damage;
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
        ourUI.resetALLMenus();
        testStarted = false;
        ChangeState(State.PlayerTurn);
    }

}
