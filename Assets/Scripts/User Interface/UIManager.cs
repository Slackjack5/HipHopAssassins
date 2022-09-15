using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;


public class UIManager : MonoBehaviour
{
    protected GameObject userInterface;
    protected GameObject combatManager;
    private GameObject actionBlock;
    private Vector2 NavigationLimit;
    protected State currentMenu;
    protected Vector2 menuNavigation;
    
    //Callable Objects
    protected GameObject limbCanvas;
    protected GameObject selectedMonster;
    protected GameObject encounteredEnemies;
    
    //bools
    protected bool limbsGenerated;
    
    //Scripts
    protected AttackMenu ourAttackMenu;
    

    protected enum State
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
        limbCanvas = userInterface.transform.GetChild(1).gameObject;
        encounteredEnemies = combatManager.transform.GetChild(0).gameObject;

        
        //Scripts
        ourAttackMenu = gameObject.GetComponent<AttackMenu>();
        
        //Variables
        menuNavigation.x = 0;

    }

    protected void ChangeState(State state)
    {
        currentMenu = state;
        menuNavigation.x = 0;
        menuNavigation.y = 0;
        //Hide UI
    }
    // Update is called once per frame


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
            menuNavigation.y -= 1;
            resetBlocks();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            menuNavigation.y += 1;
            resetBlocks();
        }
        
        //Set for a Max size of 2
        if (menuNavigation.x > NavigationLimit.x) { menuNavigation.x = NavigationLimit.x; }
        else if (menuNavigation.x < 0) { menuNavigation.x = 0; }
        
        if (menuNavigation.y > NavigationLimit.y) { menuNavigation.y = NavigationLimit.y; }
        else if (menuNavigation.y < 0) { menuNavigation.y = 0; }
    }

    public void resetBlocks()
    {
        foreach (Transform child in actionBlock.transform)
        {
            child.GetComponent<Image>().color=Color.white;
        }
        if (selectedMonster!=null){selectedMonster.GetComponent<MonsterData>().HideLimbs();}
    }

    private void SelectMonster()
    {

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



    public void HideAllMenus()
    {
        
        //Hide Limb Menu
        MonsterData selectedData = selectedMonster.GetComponent<MonsterData>();
        
        int limbCount = selectedData.limbHealth.Length;
        //Disable Limb Selection
        for (int i = 0; i < limbCount; i++)
        {
            GameObject tempLimb = limbCanvas.transform.GetChild(i).gameObject;
            tempLimb.SetActive(false);
        }
        selectedData.HideLimbs();
        userInterface.transform.GetChild(0).gameObject.SetActive(false);
        userInterface.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void RestartMenu()
    {
        menuNavigation = new Vector2(0, 0);
        userInterface.transform.GetChild(0).gameObject.SetActive(true); //Enable Home UI
        ChangeState(State.Home);
    }

    private void HomeButtons()
    {
        
        //Current Menu Player is Hovering Over
        if (menuNavigation.x == 0) //Attack
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.SelectMonster);
            }
        }
        else if (menuNavigation.x==1) //Magic
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Magic);
            }
        }
        else if (menuNavigation.x==2) //Items
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Items);
            }
        }
        else if (menuNavigation.x==3) //Flee
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Flee);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuNavigation.x = 0;
            ChangeState(State.Home);
        }
        actionBlock.transform.GetChild((int) menuNavigation.x).GetComponent<Image>().color=Color.red;

    }

    public virtual void DeployMenu()
    {
        Debug.Log(("No Menu Available"));
    }

    protected virtual void Update()
    {
        switch (currentMenu)
        {
            case State.Home:
                actionBlock = userInterface.transform.GetChild(0).gameObject;
                maneuverMenu();
                HomeButtons();
                NavigationLimit = new Vector2(3, 0);
                break;
            case State.Attack:
                actionBlock = userInterface.transform.GetChild(1).gameObject;
                maneuverMenu();
                //ourAttackMenu.AttackButtons();
                DeployMenu();
                actionBlock.transform.GetChild((int) menuNavigation.y).GetComponent<Image>().color = Color.red;
                break;
            case State.Magic:
                Debug.Log("In Magic State");
                break;
            case State.Items:
                Debug.Log("In Items State");
                break;
            case State.Flee:
                Debug.Log("In Flee State");
                break;
            case State.EnemyTurn:

                break;
            case State.SelectMonster:

                if (CombatManager.enemyCount <= 1)
                {
                    selectedMonster = encounteredEnemies.transform.GetChild(0).gameObject;
                    Debug.Log(selectedMonster.name);
                    NavigationLimit = new Vector2(0, CombatManager.enemyCount);
                    ChangeState(State.Attack);
                }
                else
                {
            
                    NavigationLimit = new Vector2(0, CombatManager.enemyCount);
                }
                break;
        }
        
    }
    
    private void FixedUpdate()
    {
    }
}
