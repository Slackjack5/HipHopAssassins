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
    protected State currentMenu;
    //Vector2
    public Vector2 NavigationLimit;
    public Vector2 menuNavigation;
    //Callable Objects
    protected GameObject limbCanvas;
    protected GameObject encounteredEnemies;
    protected GameObject userInterface;
    protected GameObject combatManager;
    protected GameObject actionBlock;
    //static
    protected static GameObject selectedMonster;
    
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

    public virtual void DeployAttackMenu()
    {
        Debug.Log(("No Menu Available"));
    }
    public virtual void DeployHomeMenu()
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
                DeployHomeMenu();
                NavigationLimit = new Vector2(3, 0);
                actionBlock.transform.GetChild((int) menuNavigation.x).GetComponent<Image>().color=Color.red;
                break;
            case State.Attack:
                actionBlock = userInterface.transform.GetChild(1).gameObject;
                maneuverMenu();
                DeployAttackMenu();
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

}
