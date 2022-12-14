using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackMenu : MonoBehaviour
{
    //Callable Objects
    protected GameObject limbCanvas;
    protected GameObject userInterface;
    public ActionSlotManager ourActionSlotManager;
    //State
    [SerializeField] public State MenuState;
    private UIManager ourUI;
    //Bool
    private bool stateGate;
    private bool hoverEffectPlayed;
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
        ourActionSlotManager = GameObject.Find("ActionSlotManager").GetComponent<ActionSlotManager>();
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
                //Set our Navigation limit to the amount of monster Limbs
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

        if (Input.GetKeyDown("space")) //Create Action 
        {
            //CombatManager.AwaitAttack(); //Start Attack
            CreateAction(UIManager.selectedMonster,ourUI.menuNavigation.y);
            ChangeState(State.Inactive);
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

        public void CreateAction(GameObject SelectedMonster, float SelectedLimb)
        {
            ourActionSlotManager.SpawnAttackAction(SelectedMonster,SelectedLimb);
            ourUI.resetBlocks();
            ourUI.RestartMenu();
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
    
    public void SelectMonster() //Have the player select a monster
    {
        //Declared Variables
        int hoveredMonster = 0; 
        if (CombatManager.enemyCount <= 1) //If only 1 enemy on screen
        {
            ourUI.NavigationLimit = new Vector3(0, 0,0); //Change our navigation limit
            hoveredMonster = 0; //Change our hovered monster int
        }
        else if (CombatManager.enemyCount == 2) //If two enemies on Screen
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
        else if (CombatManager.enemyCount == 3) //If three enemies on Screen
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
            //Reset Monsters
            ourUI.ResetMonsters();
            //Return to previous state
            ChangeState(State.Inactive);
            //Reset values and return to home menu
            ourUI.RestartMenu();
            ourUI.ChangeState(UIManager.State.Home);
            ourUI.NavigationLimit = new Vector3(3, 0,0);
            ourUI.menuNavigation.x = 0;
            ourUI.resetBlocks();
            hoverEffectPlayed = false;

        }
        
    }
    
        
    private void PrepareMonster(int hoveredMonster)
    {
        //Change Navigation Limit to amount of enemies on screen
        ourUI.NavigationLimit = new Vector3(ourUI.encounteredEnemies.transform.childCount-1, 0,-1);
        //If hovering over a monster do on screen effect (Enlarge Them)
        for (int i = 0; i < ourUI.encounteredEnemies.transform.childCount; i++)
        {
            if (i != hoveredMonster) //Hover Exit
            {
                MMF_Player ourJuice = ourUI.encounteredEnemies.transform.GetChild(i).transform.GetChild(0).transform.GetChild(3).GetComponent<MMF_Player>();
                ourJuice.PlayFeedbacks();
            }
            else //Hover Enter
            {
                MMF_Player ourJuice = ourUI.encounteredEnemies.transform.GetChild(i).transform.GetChild(0).transform.GetChild(2).GetComponent<MMF_Player>();
                if(!hoverEffectPlayed) { ourJuice.PlayFeedbacks(); hoverEffectPlayed = true;}
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            hoverEffectPlayed = false;
        }
        if (Input.GetKeyDown("space")) //If player presses space, select that monster
        {
            //Change Selected Monster
            UIManager.selectedMonster = ourUI.encounteredEnemies.transform.GetChild(hoveredMonster).gameObject;
            //Reset Monster Sizes
            ourUI.ResetMonsters();
            hoverEffectPlayed = false;
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
