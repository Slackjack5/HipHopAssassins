using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class AttackMenu : MonoBehaviour
{
    //Callable Objects
    protected GameObject limbCanvas;
    protected GameObject userInterface;
    public ActionSlotManager ourActionSlotManager;

    private UserInterface userInterfaceScript;
    //State
    [SerializeField] public State MenuState;
    private UIManager ourUIManager;
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
        userInterfaceScript = UserInterface.singleton_UserInterface;
        ourUIManager = UIManager.singleton_UIManager;
        limbCanvas = userInterfaceScript.BodyPartText;
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
        MonsterData selectedData = UIManager.selectedMonster.GetComponent<MonsterData>();

        GenerateLimbs();
        //Set our Navigation limit to the amount of monster Limbs
        UIManager.singleton_UIManager.NavigationLimit = new Vector3(selectedData.limbName.Length-1, 0,0);
        if (ourUIManager.menuNavigation.x == 0) //Limb #1
        {

            UIManager.selectedMonster.GetComponent<MonsterData>().ShowLimb();
            limbCanvas.GetComponent<TextMeshProUGUI>().text = selectedData.limbName[0];
        }
        if (ourUIManager.menuNavigation.x == 1) //Limb #1
        {
            UIManager.selectedMonster.GetComponent<MonsterData>().ShowLimb();
            limbCanvas.GetComponent<TextMeshProUGUI>().text = selectedData.limbName[1];
        }

        if (Input.GetKeyDown("space")) //Create Action 
        {
            //CombatManager.AwaitAttack(); //Start Attack
            ourUIManager.menuNavigation = new Vector3(0, 0, 0);
            CreateAction(UIManager.selectedMonster,ourUIManager.menuNavigation.x); //Set Action based on Navigation
            ChangeState(State.Inactive);
            HideLimbs();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideLimbs();
            //Reset Monsters
            ourUIManager.ResetMonsters();
            //Return to previous state
            ChangeState(State.Inactive);
            //Reset values and return to home menu
            ourUIManager.RestartMenu();
            ourUIManager.ChangeState(UIManager.State.Home);
            //ourUIManager.NavigationLimit = new Vector3(3, 0,0);
            ourUIManager.menuNavigation.x = 0;
            ourUIManager.resetBlocks();
            hoverEffectPlayed = false;
            //Effects
            SelectionAlbumManager.singleton_AlbumManager.MovePrimaryAlbum(SelectionAlbumManager.singleton_AlbumManager.attackAlbumTransform);
        }

    }

        public void CreateAction(GameObject SelectedMonster, float SelectedLimb)
        {
            ourActionSlotManager.SpawnAttackAction(SelectedMonster,SelectedLimb);
            ourUIManager.resetBlocks();
            ourUIManager.RestartMenu();
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
        
        /*
        for (int i = 0; i < limbCount; i++)
        {
            GameObject tempLimb = limbCanvas.transform.GetChild(i).gameObject;
            tempLimb.SetActive(true);
            tempLimb.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = selectedData.limbName[i];
        }
        */
        
        limbCanvas.SetActive(true);
    }
    
    
    public void HideLimbs()
    {
        //Hide Limb Menu
        if (UIManager.selectedMonster != null)
        {
            MonsterData selectedData = UIManager.selectedMonster.GetComponent<MonsterData>();
        
            int limbCount = selectedData.limbHealth.Length;
            /*
            //Disable Limb Selection
            for (int i = 0; i < limbCount; i++)
            {
                GameObject tempLimb = limbCanvas.transform.GetChild(i).gameObject;
                tempLimb.SetActive(false);
            }
            */
            selectedData.HideLimbs();
            limbCanvas.SetActive(false);
            ourUIManager.actionBlock.SetActive(false);
        }
    }
    
    public void SelectMonster() //Have the player select a monster
    {
        //Declared Variables
        int hoveredMonster = 0; 
        if (CombatManager.enemyCount <= 1) //If only 1 enemy on screen
        {
            ourUIManager.NavigationLimit = new Vector3(0, 0,0); //Change our navigation limit
            hoveredMonster = 0; //Change our hovered monster int
        }
        else if (CombatManager.enemyCount == 2) //If two enemies on Screen
        {
            ourUIManager.NavigationLimit = new Vector3(1, 0,0);
            if (ourUIManager.menuNavigation.x == 0)
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
            ourUIManager.NavigationLimit = new Vector3(2, 0,0);
            if (ourUIManager.menuNavigation.x == 1)
            {
                hoveredMonster = 0;
            }
            else if (ourUIManager.menuNavigation.x == 2)
            {
                hoveredMonster = 2;
            }
            else if (ourUIManager.menuNavigation.x == 0)
            {
                hoveredMonster = 1;
            }
        }

        PrepareMonster(hoveredMonster);
/*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Reset Monsters
            ourUIManager.ResetMonsters();
            //Return to previous state
            ChangeState(State.Inactive);
            //Reset values and return to home menu
            ourUIManager.RestartMenu();
            ourUIManager.ChangeState(UIManager.State.Home);
            ourUIManager.NavigationLimit = new Vector3(3, 0,0);
            ourUIManager.menuNavigation.x = 0;
            ourUIManager.resetBlocks();
            hoverEffectPlayed = false;

        }
        */
    }
    
        
    private void PrepareMonster(int hoveredMonster)
    {
        /*
        //Change Navigation Limit to amount of enemies on screen
        ourUIManager.NavigationLimit = new Vector3(ourUIManager.encounteredEnemies.transform.childCount-1, 0,-1);
        //If hovering over a monster do on screen effect (Enlarge Them)
        for (int i = 0; i < ourUIManager.encounteredEnemies.transform.childCount; i++)
        {
            if (i != hoveredMonster) //Hover Exit
            {
                MMF_Player ourJuice = ourUIManager.encounteredEnemies.transform.GetChild(i).transform.GetChild(0).transform.GetChild(3).GetComponent<MMF_Player>();
                ourJuice.PlayFeedbacks();
            }
            else //Hover Enter
            {
                MMF_Player ourJuice = ourUIManager.encounteredEnemies.transform.GetChild(i).transform.GetChild(0).transform.GetChild(2).GetComponent<MMF_Player>();
                if(!hoverEffectPlayed) { ourJuice.PlayFeedbacks(); hoverEffectPlayed = true;}
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            hoverEffectPlayed = false;
        }
        */
        //if (Input.GetKeyDown("space")) //If player presses space, select that monster
        //{
            //Change Selected Monster
            UIManager.selectedMonster = ourUIManager.encounteredEnemies.transform.GetChild(hoveredMonster).gameObject;
            //Reset Monster Sizes
            ourUIManager.ResetMonsters();
            hoverEffectPlayed = false;
            //Change State
            ChangeState(State.LimbList);
            //Change Previous Navigation
            ourUIManager.lastMenuNavigation = ourUIManager.menuNavigation;
        //}
        
    }

    public void ChangeState(State state)
    {
        MenuState = state;
        Debug.Log("Going to State: "+state);
    }

}
