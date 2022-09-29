using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackMenu : MonoBehaviour
{
    //Callable Objects
    protected GameObject limbCanvas;
    protected GameObject userInterface;
    //State
    [SerializeField] public State MenuState;
    private UIManager ourUI;
    
    //Bool
    private bool stateGate;
    
    
    public enum State
    {
        Inactive,
        LimbList,
        SelectMonster,
    }
    
    // Start is called before the first frame update
    void Start()
    {
        userInterface = GameObject.Find("UserInterface");
        limbCanvas = userInterface.transform.GetChild(1).gameObject;
        ourUI = gameObject.GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (MenuState)
        {
            case State.Inactive:
                stateGate = false;
                break;
            case State.LimbList:
                ourUI.NavigationLimit = new Vector3(0, 0,-1*(UIManager.selectedMonster.GetComponent<MonsterData>().limbHealth.Length-1));
                DeployAttackMenu();
                break;
            case State.SelectMonster:
            if (stateGate == true)
            {
                SelectMonster();
            }
            else
            {
                stateGate = true;
            }

            break;
        }
    }
    
        public void DeployAttackMenu()
    {
        GenerateLimbs();
        if (ourUI.menuNavigation.y == 0) //Limb #1
        {
            UIManager.selectedMonster.GetComponent<MonsterData>().ShowLimb(0);
            ourUI.actionBlock.transform.GetChild(0).GetComponent<Image>().color = Color.red;
        }
        else if (ourUI.menuNavigation.y == -1) //Limb #1
        {
            UIManager.selectedMonster.GetComponent<MonsterData>().ShowLimb(1);
            ourUI.actionBlock.transform.GetChild(1).GetComponent<Image>().color = Color.red;
        }
        else if (ourUI.menuNavigation.y == -2) //Limb #1
        {
            UIManager.selectedMonster.GetComponent<MonsterData>().ShowLimb(2);
            ourUI. actionBlock.transform.GetChild(2).GetComponent<Image>().color = Color.red;
        }
        else if (ourUI.menuNavigation.y == -3) //Limb #1
        {
            UIManager.selectedMonster.GetComponent<MonsterData>().ShowLimb(3);
            ourUI.actionBlock.transform.GetChild(3).GetComponent<Image>().color = Color.red;
        }
        else if (ourUI.menuNavigation.y == -4) //Limb #1
        {
            UIManager.selectedMonster.GetComponent<MonsterData>().ShowLimb(4);
            ourUI.actionBlock.transform.GetChild(4).GetComponent<Image>().color = Color.red;
        }

        if (Input.GetKeyDown("space")) 
        {
            CombatManager.DamageMonsterLimb(UIManager.selectedMonster.GetComponent<MonsterData>(),(int) ourUI.menuNavigation.y,20);
            ChangeState(State.Inactive);
            ourUI.resetBlocks();
            HideLimbs();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideLimbs();
            ChangeState(State.SelectMonster);
            ourUI.resetBlocks();
            ourUI.NavigationLimit = NewNavigationLimit();
            ourUI.menuNavigation = ourUI.lastMenuNavigation;
            //ourUI.lastMenuNavigation.x = 0;
        }
        
    }
        
    private Vector2 NewNavigationLimit()
    {
        Vector2 newLimit = new Vector2(0,0);
        if (CombatManager.enemyCount <= 1)
        {
            newLimit = new Vector3(0, 0,0);
        }
        else if (CombatManager.enemyCount == 2)
        {
            newLimit = new Vector3(1, 0,0);
        }
        else if (CombatManager.enemyCount == 3)
        {
            newLimit = new Vector3(2, 0,0);
        }
        return newLimit;
    }

    
    private void GenerateLimbs()
    {
        MonsterData selectedData = UIManager.selectedMonster.GetComponent<MonsterData>();
        int limbCount = selectedData.limbHealth.Length;
        
        for (int i = 0; i < limbCount; i++)
        {
            GameObject tempLimb = limbCanvas.transform.GetChild(i).gameObject;
            tempLimb.SetActive(true);
            tempLimb.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = selectedData.limbName[i];
        }
        
        limbCanvas.SetActive(true);
    }
    
    
    public void HideLimbs()
    {
        //Hide Limb Menu
        if (UIManager.selectedMonster != null)
        {
            MonsterData selectedData = UIManager.selectedMonster.GetComponent<MonsterData>();
        
            int limbCount = selectedData.limbHealth.Length;
            //Disable Limb Selection
            for (int i = 0; i < limbCount; i++)
            {
                GameObject tempLimb = limbCanvas.transform.GetChild(i).gameObject;
                tempLimb.SetActive(false);
            }
            selectedData.HideLimbs();
            ourUI.actionBlock.SetActive(false);
        }
    }
    
    public void SelectMonster()
    {
        int hoveredMonster = 0;
        if (CombatManager.enemyCount <= 1)
        {
            ourUI.NavigationLimit = new Vector3(0, 0,0);
            hoveredMonster = 0;
        }
        else if (CombatManager.enemyCount == 2)
        {
            ourUI.NavigationLimit = new Vector3(1, 0,0);
            if (ourUI.menuNavigation.x == 0)
            {
                hoveredMonster = 0;
            }
            else
            {
                hoveredMonster = 1;
            }
        }
        else if (CombatManager.enemyCount == 3)
        {
            ourUI.NavigationLimit = new Vector3(2, 0,0);
            if (ourUI.menuNavigation.x == 1)
            {
                hoveredMonster = 0;
            }
            else if (ourUI.menuNavigation.x == 2)
            {
                hoveredMonster = 2;
            }
            else if (ourUI.menuNavigation.x == 0)
            {
                hoveredMonster = 1;
            }
        }

        PrepareMonster(hoveredMonster);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Reset Enemy Size
            for (int i = 0; i < ourUI.encounteredEnemies.transform.childCount; i++)
            {
                ourUI.encounteredEnemies.transform.GetChild(i).transform.localScale = new Vector3(1, 1, 1);
            }
            //Return to previous state
            ChangeState(State.Inactive);
            ourUI.RestartMenu();
            ourUI.ChangeState(UIManager.State.Home);
            ourUI.NavigationLimit = new Vector3(3, 0,0);
            ourUI.menuNavigation.x = 0;
            ourUI.resetBlocks();
        }
        
    }
    
        
    private void PrepareMonster(int hoveredMonster)
    {
        ourUI.NavigationLimit = new Vector3(ourUI.encounteredEnemies.transform.childCount-1, 0,-1);
        for (int i = 0; i < ourUI.encounteredEnemies.transform.childCount; i++)
        {
            if (i != hoveredMonster)
            {
                ourUI.encounteredEnemies.transform.GetChild(i).transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                ourUI.encounteredEnemies.transform.GetChild(i).transform.localScale = new Vector3(2, 2, 1);
            }
        }
        if (Input.GetKeyDown("space"))
        {
            UIManager.selectedMonster = ourUI.encounteredEnemies.transform.GetChild(hoveredMonster).gameObject;
            ourUI.ResetMonsters();
            //Change State
            ChangeState(State.LimbList);
            //Change Previous Navigation
            ourUI.lastMenuNavigation = ourUI.menuNavigation;
        }
    }

    public void ChangeState(State state)
    {
        MenuState = state;
        Debug.Log("Going to State: "+state);
    }

}
