using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;


public class UIManager : MonoBehaviour
{
    //State
    [SerializeField] public State currentMenu;
    [SerializeField] public State previousMenu;
    //Vector2
    [SerializeField] public Vector3 NavigationLimit;
    [SerializeField] public Vector2 menuNavigation;
    [SerializeField] public Vector2 lastMenuNavigation;

    public GameObject encounteredEnemies;
    protected GameObject userInterface;
    protected GameObject combatManager;
    public GameObject actionBlock;
    private MagicMenu ourMagicMenu;
    private AttackMenu ourAttackMenu;
    private ItemMenu ourItemMenu;

    private FleeMenu ourFleeMenu;
    //static
    public static GameObject selectedMonster;

    //Spells
    private SpellDictionary ourSpellDictionary;
    private Spell selectedSpell;



    
    
    
    public enum State
    {
        Home,
        Attack,
        Magic,
        Items,
        Flee,
        EnemyTurn,
        SelectMonster,
    }
    protected enum PreviousState
    {
        Home,
        Attack,
        Magic,
        Items,
        Flee,
        EnemyTurn,
        SelectMonster,
    }
 
    protected virtual void Start()
    {
        //GameObjects
        userInterface = GameObject.Find("UserInterface");
        combatManager = GameObject.Find("CombatManager");
        ourMagicMenu = gameObject.GetComponent<MagicMenu>();
        ourAttackMenu = gameObject.GetComponent<AttackMenu>();
        ourItemMenu = gameObject.GetComponent<ItemMenu>();
        ourFleeMenu = gameObject.GetComponent<FleeMenu>();
        encounteredEnemies = combatManager.transform.GetChild(0).gameObject;
        actionBlock = gameObject;
        

        //Variables
        menuNavigation.x = 0;
    }

    public void ChangeState(State state)
    {
        currentMenu = state;
    }
    protected void ChangePreviousState(State previousState)
    {
        previousMenu = previousState;
    }

    private void maneuverMenu()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            menuNavigation.x -= 1;
            resetBlocks();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            menuNavigation.x += 1;
            resetBlocks();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            menuNavigation.y += 1;
            resetBlocks();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            menuNavigation.y -= 1;
            resetBlocks();
        }
        
        //Set for a Max size of 2
        if (menuNavigation.x > NavigationLimit.x) { menuNavigation.x = NavigationLimit.x; Debug.Log("Breached Navigation Limit");}
        else if (menuNavigation.x < 0) { menuNavigation.x = 0; Debug.Log("Breached Navigation Limit");}
        
        if (menuNavigation.y > NavigationLimit.y) { menuNavigation.y = NavigationLimit.y; Debug.Log("Breached Navigation Limit");}
        else if (menuNavigation.y < NavigationLimit.z) { menuNavigation.y = NavigationLimit.z; Debug.Log("Breached Navigation Limit");}
        
    }

    public void resetBlocks()
    {
        for (int i = 0; i < userInterface.transform.childCount; i++)
        {
            foreach (Transform child in userInterface.transform.GetChild(i).transform)
            {
                child.GetComponent<Image>().color = Color.white;
            }
        }

        if (selectedMonster!=null){selectedMonster.GetComponent<MonsterData>().HideLimbs();}
    }

    public void UpdateUI()
    {
        GameObject ourEnemies = encounteredEnemies.transform.gameObject;
        for (int i = 0; i < ourEnemies.transform.childCount; i++)
        {
            GameObject currentEnemy = ourEnemies.transform.GetChild(i).gameObject;
            currentEnemy.GetComponent<MonsterData>().UpdateHealthUI();
        }
    }
    
    public void RestartMenu()
    {
        userInterface.transform.GetChild(0).gameObject.SetActive(true); //Enable Home UI
        ChangeState(State.Home);
    }

    protected virtual void Update()
    {
        switch (currentMenu)
        {
            case State.Home:
                ChangePreviousState(State.Home);
                actionBlock = userInterface.transform.GetChild(0).gameObject;
                maneuverMenu();
                ResetMonsters();
                DeployHomeMenu();
                NavigationLimit = new Vector3(3, 0,0);
                actionBlock.transform.GetChild((int) menuNavigation.x).GetComponent<Image>().color=Color.red;
                userInterface.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case State.Attack:
                maneuverMenu();
                userInterface.transform.GetChild(0).gameObject.SetActive(false);
                break;
            case State.Magic:
                userInterface.transform.GetChild(0).gameObject.SetActive(false);
                maneuverMenu();
                break;
            case State.Items:
                userInterface.transform.GetChild(0).gameObject.SetActive(false);
                maneuverMenu();
                break;
            case State.Flee:
                userInterface.transform.GetChild(0).gameObject.SetActive(false);
                maneuverMenu();
                break;
            case State.EnemyTurn:

                break;
            case State.SelectMonster:
                NavigationLimit = new Vector2(CombatManager.enemyCount-1, 0);
                maneuverMenu();
                break;
        }
        
    }

    public void ResetMonsters()
    {
        for (int i = 0; i < encounteredEnemies.transform.childCount; i++)
        {
            GameObject ourEnemy = encounteredEnemies.transform.GetChild(i).gameObject;
            ourEnemy.transform.localScale = new Vector3(1, 1, 1);
        }
            
    }

    public void DeployHomeMenu()
    {
        //Current Menu Player is Hovering Over
        if (menuNavigation.x == 0) //Attack
        {
            if (Input.GetKeyDown("space"))
            {
                //Change States
                ChangePreviousState(State.Home);
                ChangeState(State.Attack);
                //Change Navigation
                lastMenuNavigation = menuNavigation;
                userInterface.transform.GetChild(1).gameObject.SetActive(true);
                actionBlock = userInterface.transform.GetChild(1).gameObject;
                ourAttackMenu.ChangeState(AttackMenu.State.SelectMonster);
                MenuStartingPoint();
            }
        }
        else if (menuNavigation.x==1) //Magic
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangePreviousState(State.Home);
                //Change Navigation
                lastMenuNavigation = menuNavigation;
                menuNavigation.x = 0;
                actionBlock = userInterface.transform.GetChild(2).gameObject;
                ourMagicMenu.ChangeState(MagicMenu.State.SpellList);
                userInterface.transform.GetChild(2).gameObject.SetActive(true);
                ChangeState(State.Magic);
            }
        }
        else if (menuNavigation.x==2) //Items
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangePreviousState(State.Home);
                ChangeState(State.Items);
                //Change Navigation
                lastMenuNavigation = menuNavigation;
                menuNavigation.x = 0;
                actionBlock = userInterface.transform.GetChild(3).gameObject;
                ourItemMenu.ChangeState(ItemMenu.State.ItemList);
                userInterface.transform.GetChild(3).gameObject.SetActive(true);
            }
        }
        else if (menuNavigation.x==3) //Flee
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangePreviousState(State.Home);
                ChangeState(State.Flee);
                lastMenuNavigation = menuNavigation;
                menuNavigation.x = 0;
                ourFleeMenu.ChangeState(FleeMenu.State.Confirm);
                userInterface.transform.GetChild(4).gameObject.SetActive(true);
            }
        }

    }

    public void MenuStartingPoint()
    {
        if (CombatManager.enemyCount <= 1)
        {
            menuNavigation = new Vector3(0, 0,0);
        }
        else if (CombatManager.enemyCount == 2)
        {
            menuNavigation = new Vector3(0, 0,0);
        }
        else if (CombatManager.enemyCount == 3)
        {
            menuNavigation = new Vector3(1, 0,0);
        }
    }

    
  
}
