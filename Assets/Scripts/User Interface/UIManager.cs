using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;
using Random = System.Random;


public class UIManager : MonoBehaviour
{
    //State
    [SerializeField] public State currentMenu;
    [SerializeField] public State previousMenu;
    //Vector2
    [SerializeField] public Vector3 NavigationLimit;
    [SerializeField] public Vector2 menuNavigation;
    [SerializeField] public Vector2 lastMenuNavigation;
    //Callable Game Objects
    public GameObject encounteredEnemies;
    protected GameObject userInterface;
    protected GameObject combatManager;
    public GameObject actionBlock;
    //Menus
    private MagicMenu ourMagicMenu;
    private AttackMenu ourAttackMenu;
    private ItemMenu ourItemMenu;
    private FleeMenu ourFleeMenu;
    public static GameObject ourButton;
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
        ourButton = userInterface.transform.GetChild(5).transform.gameObject;

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
                //Change Previous State
                ChangePreviousState(State.Home);
                //Set our Action Block to our Home Menu and SetActive
                actionBlock = userInterface.transform.GetChild(0).gameObject;
                userInterface.transform.GetChild(0).gameObject.SetActive(true);
                //Allow Player to Maneuver the menu
                maneuverMenu();
                //Reset Size on All Monsters
                ResetMonsters();
                //Call Home Menu Function
                DeployHomeMenu();
                //Change our Navigation Limit
                NavigationLimit = new Vector3(3, 0,0);
                //On Hover Effect
                actionBlock.transform.GetChild((int) menuNavigation.x).GetComponent<Image>().color=Color.red;
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
                //Set Navigation Limit to the amount of monsters on Screen
                NavigationLimit = new Vector2(CombatManager.enemyCount-1, 0);
                maneuverMenu();
                break;
        }
        
    }

    public void ResetMonsters() //Resets the Scale for all monsters in the Encountered Enemies Object
    {
        for (int i = 0; i < encounteredEnemies.transform.childCount; i++)
        {
            MMF_Player ourJuice = encounteredEnemies.transform.GetChild(i).transform.GetChild(0).transform.GetChild(3).GetComponent<MMF_Player>();
            ourJuice.PlayFeedbacks();
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
                //Set the action block to our Attack Menu & set it active
                userInterface.transform.GetChild(1).gameObject.SetActive(true);
                actionBlock = userInterface.transform.GetChild(1).gameObject;
                //Change the state of our Attack Menu Script
                ourAttackMenu.ChangeState(AttackMenu.State.SelectMonster);
                //Change our menu navigation starting point dependent on how many enemies are on screen
                MenuStartingPoint();
            }
        }
        else if (menuNavigation.x==1) //Magic
        {
            if (Input.GetKeyDown("space")) 
            {
                //Change States
                ChangePreviousState(State.Home);
                ChangeState(State.Magic);
                //Change Navigation
                lastMenuNavigation = menuNavigation;
                menuNavigation.x = 0;
                //Set the action block to our Magic Menu & set it active
                actionBlock = userInterface.transform.GetChild(2).gameObject;
                userInterface.transform.GetChild(2).gameObject.SetActive(true);
                //Change the state of our Attack Menu Script
                ourMagicMenu.ChangeState(MagicMenu.State.SpellList);
            }
        }
        else if (menuNavigation.x==2) //Items
        {
            if (Input.GetKeyDown("space")) 
            {
                //Change States
                ChangePreviousState(State.Home);
                ChangeState(State.Items);
                //Change Navigation
                lastMenuNavigation = menuNavigation;
                menuNavigation.x = 0;
                //Set the action block to our Item Menu & set it active
                actionBlock = userInterface.transform.GetChild(3).gameObject;
                userInterface.transform.GetChild(3).gameObject.SetActive(true);
                //Change the state of our Item Menu Script
                ourItemMenu.ChangeState(ItemMenu.State.ItemList);

            }
        }
        else if (menuNavigation.x==3) //Flee
        {
            if (Input.GetKeyDown("space")) 
            {
                //Change States
                ChangePreviousState(State.Home);
                ChangeState(State.Flee);
                //Change Navigation
                lastMenuNavigation = menuNavigation;
                menuNavigation.x = 0;
                //Set our new action block as active
                userInterface.transform.GetChild(4).gameObject.SetActive(true);
                //Change the state of our Item Menu Script
                ourFleeMenu.ChangeState(FleeMenu.State.Confirm);
            }
        }

    }
    public void MenuStartingPoint() //Changes menu navigation dependent on enemy count
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
