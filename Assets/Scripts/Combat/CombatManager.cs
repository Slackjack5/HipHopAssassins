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
    private GameObject actionSlotManager;
    private static PlayerScript ourPlayer;
    private bool enemiesSpawned;
    protected static State currentTurn;

    private static UIManager ourUI;
    private SpawnPoints ourSpawnPoints;
    private bool testStarted = false;
    private ItemDictionary ourItemDictionary;
    private MusicManager ourMusicManager;

    public static int hitsRemaining;

    private static int ourSetDamage;

    private static GameObject ourSeleectedMonster;
    protected GameObject userInterface;
    public static int comboPoints;
    private static ActionSlotManager ourActionSlotManager;
    //Music
    private static AudioEvents ourAudioEvents;


    protected enum State
    {
        Prefight,
        PlayerTurn,
        MonsterTurn,
        Defcon,
        AwaitingAttack,
        AwaitingMagic,
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
        ourMusicManager = GameObject.Find("WwiseGlobal").GetComponent<MusicManager>();
        ourAudioEvents = GameObject.Find("WwiseGlobal").GetComponent<AudioEvents>();
        hitsRemaining = 0;
        userInterface = GameObject.Find("UserInterface");
        ourActionSlotManager = GameObject.Find("ActionSlotManager").GetComponent<ActionSlotManager>();
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
                //Play Stinger
                if (!ourMusicManager.preparingSequence)
                {
                    ourMusicManager.CommenceAttackEvent();
                    Debug.Log("Starting Attack Event");
                }

                if (ourMusicManager.attackInProgress == true) //Start once our sequence begins
                {
                    UIManager.ourButton.SetActive(true);
                }

                break;
            case State.AwaitingMagic:
                Debug.Log("In Awaiting Attack Turn");
                
                UIManager.ourButton.SetActive(true);
                if (Input.GetKeyDown("space"))
                {
                    hitsRemaining -= 1;
                    //Button Down
                    Animator ourButtonAnim = UIManager.ourButton.transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
                    CastSpell(ourSeleectedMonster, ourSetDamage);
                    ourButtonAnim.Play("ButtonHeld");
                }

                if (Input.GetKeyUp("space"))
                {
                    Animator ourButtonAnim = UIManager.ourButton.transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
                    ourButtonAnim.Play("ButtonRise");
                }
                break;
        }

    }

    private void LateUpdate()
    {
        //Check How Many Enemies We Have On Screen
        enemyCount = encounterEnemies.transform.childCount;
    }

    public static void Reset()
    {
        hitsRemaining = ourPlayer.hitsMax;
    }

    public static void playerMeleeMiss()
    {
        
        //Feedbacks
        GameObject monsterGFX = UIManager.selectedMonster.transform.GetChild(0).gameObject;
        MMF_Player targetFeedback = monsterGFX.transform.GetChild(6).GetComponent<MMF_Player>();
        targetFeedback.ResetFeedbacks();
        targetFeedback.PlayFeedbacks();
        //Button Down
        Animator ourButtonAnim = UIManager.ourButton.transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
        ourButtonAnim.Play("ButtonHeld");
        hitsRemaining -= 1;
    }
    
    public static void playerMeleeAttack(int damageMultiplier)
    {
        if (ourActionSlotManager.Actions[ourAudioEvents.currentBar-1]!=null)
        {
            int generatedDamage = UnityEngine.Random.Range(ourPlayer.attackMin, ourPlayer.attackMax) * damageMultiplier;
            GameObject newSelectedMonster = ourActionSlotManager.Actions[ourAudioEvents.currentBar-1].GetComponent<AttackAction>().SelectedMonster;
            float newSelectedLimb = ourActionSlotManager.Actions[ourAudioEvents.currentBar-1].GetComponent<AttackAction>().SelectedLimb;
            DamageMonsterLimb(newSelectedMonster,(int) newSelectedLimb, generatedDamage, damageMultiplier);
        
            hitsRemaining -= 1;
            //Button Down
            Animator ourButtonAnim = UIManager.ourButton.transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
            ourButtonAnim.Play("ButtonHeld");
        }
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
    public static void DamageMonsterLimb(GameObject Monster, int limbNumber,int damage, int multiplier)
    {
        
        //Initialize Variables
        MonsterData ourMonster = Monster.GetComponent<MonsterData>();
        GameObject monsterGFX = Monster.transform.GetChild(0).gameObject;

        if (ourMonster.Resistance[0].Beat[GlobalVariables.currentBar-1] == false)
        {
            ourUI.UpdateUI();
        
            Debug.Log("Awaiting next hit!");
            ChangeState(State.AwaitingAttack);
            
            //Initialie Feedback
            MMF_Player targetFeedback = monsterGFX.transform.GetChild(1).GetComponent<MMF_Player>();
            MMF_FloatingText floatingText = targetFeedback.GetFeedbackOfType<MMF_FloatingText>();
            targetFeedback.FeedbacksIntensity = multiplier;
            floatingText.Value = damage.ToString();
            //Deal Damage
            targetFeedback.ResetFeedbacks();
            targetFeedback.PlayFeedbacks();
            int newLimbNumber = (int) MathF.Abs(limbNumber);
            ourMonster.limbHealth[newLimbNumber] -= damage;
        }
        else
        {
            ourUI.UpdateUI();
        
            Debug.Log("Awaiting next hit!");
            ChangeState(State.AwaitingAttack);
            
            //Initialie Feedback
            MMF_Player targetFeedback = monsterGFX.transform.GetChild(7).GetComponent<MMF_Player>();
            MMF_FloatingText floatingText = targetFeedback.GetFeedbackOfType<MMF_FloatingText>();
            targetFeedback.FeedbacksIntensity = 0.1f;
            floatingText.Value = "RESIST: "+(damage/3).ToString();
            //Deal Damage
            targetFeedback.ResetFeedbacks();
            targetFeedback.PlayFeedbacks();
            int newLimbNumber = (int) MathF.Abs(limbNumber);
            ourMonster.limbHealth[newLimbNumber] -= damage/3;
        }

    }
    public static void CastSpell(GameObject Monster,int damage)
    {
        //Initialize Variables
        MonsterData ourMonster = Monster.GetComponent<MonsterData>();
        GameObject monsterGFX = Monster.transform.GetChild(0).gameObject;
        
        ourUI.UpdateUI();

        if (hitsRemaining <= 0)
        {
            //End Player Turn
            ChangeState(State.MonsterTurn);
            UIManager.ourButton.SetActive(false);
            hitsRemaining = ourPlayer.hitsMax;
            ourMonster.monsterHealth -= damage;
            
            //Initialie Feedback
            MMF_Player targetFeedback = monsterGFX.transform.GetChild(4).GetComponent<MMF_Player>();
            MMF_FloatingText floatingText = targetFeedback.GetFeedbackOfType<MMF_FloatingText>();
            floatingText.Value = ourSetDamage.ToString();
            //Deal Damage
            targetFeedback.ResetFeedbacks();
            targetFeedback.PlayFeedbacks();
        }
        else
        {
            Debug.Log("Awaiting next hit!");
            MMF_Player targetFeedback = monsterGFX.transform.GetChild(5).GetComponent<MMF_Player>();
            targetFeedback.ResetFeedbacks();
            targetFeedback.PlayFeedbacks();
            ourSetDamage = damage;
            ourSeleectedMonster = Monster;
            ChangeState(State.AwaitingMagic);
        }


    }

    public void SetHitsRemaining(int Hits)
    {
        hitsRemaining = Hits;
    }

    public static void AwaitAttack()
    {
        //End Player Turn
        ChangeState(State.AwaitingAttack);
    }
    
    public static void AwaitMagic()
    {
        //End Player Turn
        ChangeState(State.AwaitingMagic);
    }

    public static void SkipPlayerTurn()
    {
        //End Player Turn
        ChangeState(State.MonsterTurn);
        UIManager.ourButton.SetActive(false);
    }


    public static void SetTargetAndDamage(GameObject Target, int Damage)
    {
        //End Player Turn
        ourSeleectedMonster = Target;
        ourSetDamage = Damage;
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
        ourUI.ResetMenu();
        testStarted = false;
        ChangeState(State.PlayerTurn);
    }

}
