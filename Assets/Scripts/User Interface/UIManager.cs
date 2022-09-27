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
    //Callable Objects
    protected GameObject limbCanvas;
    public GameObject encounteredEnemies;
    protected GameObject userInterface;
    protected GameObject combatManager;
    public GameObject actionBlock;

    private MagicMenu ourMagicMenu;
    //static
    public static GameObject selectedMonster;
    public static GameObject ourPlayer;
    //Spells
    private SpellDictionary ourSpellDictionary;
    private Spell selectedSpell;
    //items
    private ItemDictionary ourItemDictionary;
    private Item selectedItem;

    
    
    
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
        ourPlayer = GameObject.Find("Player");
        ourItemDictionary = GameObject.Find("ItemDictionary").GetComponent<ItemDictionary>();
        ourMagicMenu = gameObject.GetComponent<MagicMenu>();
        limbCanvas = userInterface.transform.GetChild(1).gameObject;
        encounteredEnemies = combatManager.transform.GetChild(0).gameObject;
        actionBlock = gameObject;
        

        //Variables
        menuNavigation.x = 0;
    }

    public void ChangeState(State state)
    {
        currentMenu = state;
        menuNavigation.x = 0;
        menuNavigation.y = 0;
        //Hide UI
    }
    protected void ChangePreviousState(State previousState)
    {
        previousMenu = previousState;
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
            menuNavigation.y += 1;
            resetBlocks();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            menuNavigation.y -= 1;
            resetBlocks();
        }
        
        //Set for a Max size of 2
        if (menuNavigation.x > NavigationLimit.x) { menuNavigation.x = NavigationLimit.x; }
        else if (menuNavigation.x < 0) { menuNavigation.x = 0; }
        
        if (menuNavigation.y > NavigationLimit.y) { menuNavigation.y = NavigationLimit.y; }
        else if (menuNavigation.y < NavigationLimit.z) { menuNavigation.y = NavigationLimit.z; }
        
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



    public void HideAllMenus()
    {
        //Hide Limb Menu
        if (selectedMonster != null)
        {
            MonsterData selectedData = selectedMonster.GetComponent<MonsterData>();
        
            int limbCount = selectedData.limbHealth.Length;
            //Disable Limb Selection
            for (int i = 0; i < limbCount; i++)
            {
                GameObject tempLimb = limbCanvas.transform.GetChild(i).gameObject;
                tempLimb.SetActive(false);
            }
            selectedData.HideLimbs();
        }
        
        userInterface.transform.GetChild(0).gameObject.SetActive(false);
        userInterface.transform.GetChild(1).gameObject.SetActive(false);
        userInterface.transform.GetChild(2).gameObject.SetActive(false);
        userInterface.transform.GetChild(3).gameObject.SetActive(false);
    }

    public void RestartMenu()
    {
        menuNavigation = new Vector3(0, 0,0);
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
                ChangePreviousState(State.SelectMonster);
                actionBlock = userInterface.transform.GetChild(1).gameObject;
                NavigationLimit = new Vector3(0, 0,-1*(selectedMonster.GetComponent<MonsterData>().limbHealth.Length-1));
                maneuverMenu();
                DeployAttackMenu();
                userInterface.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case State.Magic:
                ChangePreviousState(State.Home);
                userInterface.transform.GetChild(0).gameObject.SetActive(false);
                maneuverMenu();
                break;
            case State.Items:
                actionBlock = userInterface.transform.GetChild(3).gameObject;
                userInterface.transform.GetChild(0).gameObject.SetActive(false);
                ChangePreviousState(State.Home);
                maneuverMenu();
                DeployItemMenu();
                NavigationLimit = new Vector3(3, 0,-1);
                userInterface.transform.GetChild(3).gameObject.SetActive(true);
                break;
            case State.Flee:
                Debug.Log("In Flee State");
                break;
            case State.EnemyTurn:

                break;
            case State.SelectMonster:
                NavigationLimit = new Vector2(CombatManager.enemyCount-1, 0);
                maneuverMenu();
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
            actionBlock.transform.GetChild(0).GetComponent<Image>().color = Color.red;
        }
        else if (menuNavigation.y == -1) //Limb #1
        {
            selectedMonster.GetComponent<MonsterData>().ShowLimb(1);
            actionBlock.transform.GetChild(1).GetComponent<Image>().color = Color.red;
        }
        else if (menuNavigation.y == -2) //Limb #1
        {
            selectedMonster.GetComponent<MonsterData>().ShowLimb(2);
            actionBlock.transform.GetChild(2).GetComponent<Image>().color = Color.red;
        }
        else if (menuNavigation.y == -3) //Limb #1
        {
            selectedMonster.GetComponent<MonsterData>().ShowLimb(3);
            actionBlock.transform.GetChild(3).GetComponent<Image>().color = Color.red;
        }
        else if (menuNavigation.y == -4) //Limb #1
        {
            selectedMonster.GetComponent<MonsterData>().ShowLimb(4);
            actionBlock.transform.GetChild(4).GetComponent<Image>().color = Color.red;
        }

        if (Input.GetKeyDown("space")) 
        {
            CombatManager.DamageMonsterLimb(selectedMonster.GetComponent<MonsterData>(),(int) menuNavigation.y,20);
            resetBlocks();
            HideAllMenus();
            ChangeState(State.Home);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuNavigation.x = 0;
            HideAllMenus();
            RestartMenu();
            ChangeState(previousMenu);
            resetBlocks();
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
    

    private void GenerateItems()
    {
        int[] playerItems = ourPlayer.GetComponent<PlayerScript>().allocatedItems;
        actionBlock.gameObject.SetActive(true);
        for (int i = 0; i < playerItems.Length; i++)
        {
            GameObject tempItem = actionBlock.transform.GetChild(i).gameObject;
            tempItem.SetActive(true);
            if (ourItemDictionary.itemPool[playerItems[i]] == null)
            {
                tempItem.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Empty Slot";
            }
            else
            {
                tempItem.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = ourItemDictionary.itemPool[playerItems[i]].GetComponent<Item>().itemName;
            }
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
    
    //Home
    public void DeployHomeMenu()
    {
        //Current Menu Player is Hovering Over
        if (menuNavigation.x == 0) //Attack
        {
            if (Input.GetKeyDown("space"))
            {
                ChangePreviousState(State.Home);
                ChangeState(State.SelectMonster);
                MenuStartingPoint();
            }
        }
        else if (menuNavigation.x==1) //Magic
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangePreviousState(State.Home);
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
            }
        }
        else if (menuNavigation.x==3) //Flee
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangePreviousState(State.Home);
                ChangeState(State.Flee);
            }
        }

    }

    private void MenuStartingPoint()
    {
        if (CombatManager.enemyCount <= 1)
        {
            menuNavigation = new Vector3(0, 0,0);
        }
        else if (CombatManager.enemyCount == 2)
        {
            menuNavigation = new Vector3(1, 0,0);
        }
        else if (CombatManager.enemyCount == 3)
        {
            menuNavigation = new Vector3(1, 0,0);
        }
    }

    
    public void DeployItemMenu()
    {
        GenerateItems();
        int[] playerItems = ourPlayer.GetComponent<PlayerScript>().allocatedItems;
        
        //Current Menu Player is Hovering Over
        if (menuNavigation.x == 0 && menuNavigation.y == 0) //Attack
        {
            actionBlock.transform.GetChild(0).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space"))
            {
                if (playerItems[0] != 0)
                {
                    selectedItem = ourItemDictionary.itemPool[playerItems[0]].GetComponent<Item>();
                    Instantiate( ourItemDictionary.itemPool[playerItems[0]]);
                    playerItems[0] = 0;
                }
                else
                { 
                    Debug.Log("No Item Allocated");
                }

            }
        }
        else if (menuNavigation.x==1 && menuNavigation.y == 0) //Magic
        {
            actionBlock.transform.GetChild(1).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space")) 
            {
                if (playerItems[1] != 0)
                {
                    selectedItem = ourItemDictionary.itemPool[playerItems[1]].GetComponent<Item>();
                    Instantiate( ourItemDictionary.itemPool[playerItems[1]]);
                    playerItems[1] = 0;
                }               
                else
                { 
                    Debug.Log("No Item Allocated");
                }

            }

        }
        else if (menuNavigation.x==2 && menuNavigation.y == 0) //Items
        {
            actionBlock.transform.GetChild(2).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space")) 
            {
                if (playerItems[2] != 0)
                {
                    selectedItem = ourItemDictionary.itemPool[playerItems[2]].GetComponent<Item>();
                    Instantiate( ourItemDictionary.itemPool[playerItems[2]]);
                    playerItems[2] = 0;
                }
                else
                { 
                    Debug.Log("No Item Allocated");
                }
            }
        }
        else if (menuNavigation.x==3 && menuNavigation.y == 0) //Flee
        {
            actionBlock.transform.GetChild(3).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space")) 
            {
                if (playerItems[3] != 0)
                {
                    selectedItem = ourItemDictionary.itemPool[playerItems[3]].GetComponent<Item>();
                    Instantiate( ourItemDictionary.itemPool[playerItems[3]]);
                    playerItems[3] = 0;
                }
                else
                { 
                    Debug.Log("No Item Allocated");
                }
            }
        }        
        else if (menuNavigation.x==0 && menuNavigation.y == -1) //Magic
        {
            actionBlock.transform.GetChild(4).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space")) 
            {
                if (playerItems[4] != 0)
                {
                    selectedItem = ourItemDictionary.itemPool[playerItems[4]].GetComponent<Item>();
                    Instantiate( ourItemDictionary.itemPool[playerItems[4]]);
                    playerItems[4] = 0;
                }
                else
                { 
                    Debug.Log("No Item Allocated");
                }
            }

        }
        else if (menuNavigation.x==1 && menuNavigation.y == -1) //Items
        {
            actionBlock.transform.GetChild(5).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space")) 
            {
                if (playerItems[5] != 0)
                {
                    selectedItem = ourItemDictionary.itemPool[playerItems[5]].GetComponent<Item>();
                    Instantiate( ourItemDictionary.itemPool[playerItems[5]]);
                    playerItems[5] = 0;
                }
                else
                { 
                    Debug.Log("No Item Allocated");
                }
            }
        }
        else if (menuNavigation.x==2 && menuNavigation.y == -1) //Flee
        {
            actionBlock.transform.GetChild(6).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space")) 
            {
                if (playerItems[6] != 0)
                {
                    selectedItem = ourItemDictionary.itemPool[playerItems[6]].GetComponent<Item>();
                    Instantiate( ourItemDictionary.itemPool[playerItems[6]]);
                    playerItems[6] = 0;
                }
                else
                { 
                    Debug.Log("No Item Allocated");
                }
            }
        }
        else if (menuNavigation.x==3 && menuNavigation.y == -1) //Flee
        {
            actionBlock.transform.GetChild(7).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space")) 
            {
                if (playerItems[7] != 0)
                {
                    selectedItem = ourItemDictionary.itemPool[playerItems[7]].GetComponent<Item>();
                    Instantiate( ourItemDictionary.itemPool[playerItems[7]]);
                    playerItems[7] = 0;
                }
                else
                { 
                    Debug.Log("No Item Allocated");
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideAllMenus();
            RestartMenu();
            ChangeState(previousMenu);
            menuNavigation.x = 0;
            for (int i = 0; i < actionBlock.transform.childCount; i++)
            {
                GameObject tempSpell = actionBlock.transform.GetChild(i).gameObject;
                tempSpell.SetActive(false);
            }
            resetBlocks();
        }

    }

}
