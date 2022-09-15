using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject userInterface;
    [SerializeField] public GameObject combatManager;
    private GameObject actionBlock;
    private Vector2 NavigationLimit;
    protected State currentMenu;
    private Vector2 menuNavigation;
    
    //Callable Objects
    private GameObject limbCanvas;
    private GameObject selectedMonster;
    private GameObject encounteredEnemies;
    
    //bools
    private bool limbsGenerated;
    protected enum State
    {
        Home,
        Attack,
        Magic,
        Items,
        Flee,
        EnemyTurn,
    }

    private void Start()
    {
        menuNavigation.x = 0;
        limbCanvas = userInterface.transform.GetChild(1).gameObject;
        encounteredEnemies = combatManager.transform.GetChild(0).gameObject;
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

    private void AttackButtons()
    {
        SelectMonster();
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
        actionBlock.transform.GetChild((int) menuNavigation.y).GetComponent<Image>().color=Color.red;

    }
    
    private void resetBlocks()
    {
        foreach (Transform child in actionBlock.transform)
        {
            child.GetComponent<Image>().color=Color.white;
        }
        if (selectedMonster!=null){selectedMonster.GetComponent<MonsterData>().HideLimbs();}
    }

    private void SelectMonster()
    {

        if (CombatManager.enemyCount <= 1)
        {
            selectedMonster = encounteredEnemies.transform.GetChild(0).gameObject;
            Debug.Log(selectedMonster.name);
            NavigationLimit = new Vector2(0, CombatManager.enemyCount);
        }
        else
        {
            
            NavigationLimit = new Vector2(0, CombatManager.enemyCount);
        }
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

    public void resetALLMenus()
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
                ChangeState(State.Attack);
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

    void Update()
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
                AttackButtons();
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
        }
        
    }
    
    private void FixedUpdate()
    {
    }
}
