using System;
using System.Collections;
//using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

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
    private ItemDictionary ourItemDictionary;

    private static int hitsRemaining;
    
    protected enum State
    {
        Prefight,
        PlayerTurn,
        MonsterTurn,
        Defcon,
        AwaitingAttack,
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
        hitsRemaining = ourPlayer.hitsMax;
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
            case State.AwaitingAttack:
                Debug.Log("In Awaiting Attack Turn");
                if (Input.GetKeyDown("space"))
                {
                    int generatedDamage = UnityEngine.Random.Range(ourPlayer.attackMin, ourPlayer.attackMax);
                    DamageMonsterLimb(UIManager.selectedMonster,(int) ourUI.menuNavigation.y, generatedDamage);
                    hitsRemaining -= 1;
                }
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
        ourUI.UpdateUI();
        //End Player Turn
        ChangeState(State.MonsterTurn);
    }

    public static void UseItem(GameObject item)
    {
        Instantiate(item);
    }
    public static void DamageMonsterLimb(GameObject Monster, int limbNumber,int damage)
    {
        //Initialize Variables
        MonsterData ourMonster = Monster.GetComponent<MonsterData>();
        GameObject monsterGFX = Monster.transform.GetChild(0).gameObject;
        
        //Initialie Feedback
        MMF_Player targetFeedback = monsterGFX.transform.GetChild(1).GetComponent<MMF_Player>();
        MMF_FloatingText floatingText = targetFeedback.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = damage.ToString();
        //Deal Damage
        targetFeedback.PlayFeedbacks();
        int newLimbNumber = (int) MathF.Abs(limbNumber);
        ourMonster.limbHealth[newLimbNumber] -= damage;
        
        ourUI.UpdateUI();
        if (hitsRemaining <= 0)
        {
            //End Player Turn
            ChangeState(State.MonsterTurn);
            hitsRemaining = ourPlayer.hitsMax;
        }
        else
        {
            Debug.Log("Awaiting next hit!");
            ChangeState(State.AwaitingAttack);
        }


    }
    public static void CastSpell(MonsterData ourMonster,int damage)
    {
        ourMonster.monsterHealth -= damage;
        ourUI.UpdateUI();
        //End Player Turn
        ChangeState(State.MonsterTurn);

    }

    public static void AwaitAttack()
    {
        //End Player Turn
        ChangeState(State.AwaitingAttack);
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
