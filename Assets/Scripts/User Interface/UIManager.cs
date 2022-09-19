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
    [SerializeField] protected State currentMenu;
    //Vector2
    [SerializeField] protected Vector2 NavigationLimit;
    [SerializeField] protected Vector2 menuNavigation;
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

    protected virtual void Update()
    {
        switch (currentMenu)
        {
            case State.Home:
                
                actionBlock = userInterface.transform.GetChild(0).gameObject;
                maneuverMenu();
                ResetMonsters();
                DeployHomeMenu();
                NavigationLimit = new Vector2(3, 0);
                actionBlock.transform.GetChild((int) menuNavigation.x).GetComponent<Image>().color=Color.red;
                break;
            case State.Attack:
                actionBlock = userInterface.transform.GetChild(1).gameObject;
                NavigationLimit = new Vector2(0, selectedMonster.GetComponent<MonsterData>().limbHealth.Length-1);
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
                NavigationLimit = new Vector2(CombatManager.enemyCount-1, 0);
                maneuverMenu();
                if (CombatManager.enemyCount <= 1)
                {
                    selectedMonster = encounteredEnemies.transform.GetChild(0).gameObject;
                    Debug.Log(selectedMonster.name);
                    ChangeState(State.Attack);
                }
                else if (CombatManager.enemyCount == 2)
                {
                    if (menuNavigation.x == 0)
                    {
                        encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(2,2,1);
                        encounteredEnemies.transform.GetChild(1).transform.localScale = new Vector3(1,1,1);
                        if (Input.GetKeyDown("space"))
                        {
                            selectedMonster = encounteredEnemies.transform.GetChild(0).gameObject;
                            ResetMonsters();
                            ChangeState(State.Attack);
                        }
                    }
                    else
                    {
                        encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(1,1,1);
                        encounteredEnemies.transform.GetChild(1).transform.localScale = new Vector3(2,2,1);
                        if (Input.GetKeyDown("space"))
                        {
                            selectedMonster = encounteredEnemies.transform.GetChild(1).gameObject;
                            ResetMonsters();
                            ChangeState(State.Attack);
                        }
                    }
                }               
                else if (CombatManager.enemyCount == 3)
                {
                    if (menuNavigation.x == 1)
                    {
                        encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(2,2,1);
                        encounteredEnemies.transform.GetChild(1).transform.localScale = new Vector3(1,1,1);
                        encounteredEnemies.transform.GetChild(2).transform.localScale = new Vector3(1,1,1);
                        if (Input.GetKeyDown("space"))
                        {
                            selectedMonster = encounteredEnemies.transform.GetChild(0).gameObject;
                            ResetMonsters();
                            ChangeState(State.Attack);
                        }
                    }
                    else if (menuNavigation.x == 2)
                    {
                        encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(1,1,1);
                        encounteredEnemies.transform.GetChild(2).transform.localScale = new Vector3(2,2,1);
                        encounteredEnemies.transform.GetChild(1).transform.localScale = new Vector3(1,1,1);
                        if (Input.GetKeyDown("space"))
                        {
                            selectedMonster = encounteredEnemies.transform.GetChild(2).gameObject;
                            ResetMonsters();
                            ChangeState(State.Attack);
                        }
                    }
                    else if (menuNavigation.x == 0)
                    {
                        encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(1,1,1);
                        encounteredEnemies.transform.GetChild(2).transform.localScale = new Vector3(1,1,1);
                        encounteredEnemies.transform.GetChild(1).transform.localScale = new Vector3(2,2,1);
                        if (Input.GetKeyDown("space"))
                        {
                            selectedMonster = encounteredEnemies.transform.GetChild(1).gameObject;
                            ResetMonsters();
                            ChangeState(State.Attack);
                        }
                    }
                }
                break;
        }
        
    }
/////////////////////////////////////////Combat Manager///////////////////////////////////////////////////////

    public void DeployAttackMenu()
    {
        GenerateLimbs();
        if (menuNavigation.y == 0) //Limb #1
        {
            selectedMonster.GetComponent<MonsterData>().ShowLimb(0);
        }
        else if (menuNavigation.y == 1) //Limb #1
        {
            selectedMonster.GetComponent<MonsterData>().ShowLimb(1);
        }

        if (Input.GetKeyDown("space")) 
        {
            CombatManager.DamageMonsterLimb(selectedMonster.GetComponent<MonsterData>(),(int) menuNavigation.y,20);
            resetBlocks();
            HideAllMenus();
            ChangeState(State.Home);
        }
        
        
    }
    
    private void GenerateLimbs()
    {
        MonsterData selectedData = selectedMonster.GetComponent<MonsterData>();
        int limbCount = selectedData.limbHealth.Length;
        
        for (int i = 0; i < limbCount; i++)
        {
            GameObject tempLimb = limbCanvas.transform.GetChild(i).gameObject;
            tempLimb.SetActive(true);
            tempLimb.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = selectedData.limbName[i];
        }
        
        limbCanvas.SetActive(true);
    }

    private void ResetMonsters()
    {
        for (int i = 0; i < encounteredEnemies.transform.childCount; i++)
        {
            GameObject ourEnemy = encounteredEnemies.transform.GetChild(i).gameObject;
            ourEnemy.transform.localScale = new Vector3(1, 1, 1);
        }
            
    }
    
    //Home
    public void DeployHomeMenu()
    {
        //Current Menu Player is Hovering Over
        if (menuNavigation.x == 0) //Attack
        {
            if (Input.GetKeyDown("space"))
            {
                ChangeState(State.SelectMonster);
                MenuStartingPoint();
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

    }

    private void MenuStartingPoint()
    {
        if (CombatManager.enemyCount <= 1)
        {
            menuNavigation = new Vector2(0, 0);
        }
        else if (CombatManager.enemyCount == 2)
        {
            menuNavigation = new Vector2(1, 0);
        }
        else if (CombatManager.enemyCount == 3)
        {
            menuNavigation = new Vector2(1, 0);
        }
    }

}
