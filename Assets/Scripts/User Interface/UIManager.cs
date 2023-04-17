using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using Random = System.Random;


public class UIManager : MonoBehaviour
{
    //Singleton
    public static UIManager singleton_UIManager;
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
    private UserInterface userInterfaceScript;
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
    private static ActionSlotManager ourActionSlotManager;
    //Bools
    private bool navigationOnCooldown;
    public enum State
    {
        Home,
        Attack,
        Magic,
        Items,
        Flee,
        EnemyTurn,
        Disabled,
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
        Disabled,
        SelectMonster,
    }
    
    private void Awake()
    {
        if (singleton_UIManager != null && singleton_UIManager != this)
        {
            Destroy(this);
        }
        else
        {
            singleton_UIManager = this;
        }
    }
 
    protected virtual void Start()
    {
        //GameObjects
        userInterface = UserInterface.singleton_UserInterface.GameObject();
        userInterfaceScript = userInterface.GetComponent<UserInterface>();
        ourMagicMenu = gameObject.GetComponent<MagicMenu>();
        ourAttackMenu = gameObject.GetComponent<AttackMenu>();
        ourItemMenu = gameObject.GetComponent<ItemMenu>();
        ourFleeMenu = gameObject.GetComponent<FleeMenu>();
        encounteredEnemies = CombatManager.singleton_CombatManager.transform.GetChild(0).gameObject;
        actionBlock = gameObject;
        ourButton = userInterfaceScript.Button;
        ourActionSlotManager = GameObject.Find("ActionSlotManager").GetComponent<ActionSlotManager>();
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
            CheckNavigationLimits();

            if(currentMenu==State.Home) {RepositionAlbums();}
            //Play Animation
            SelectionAlbumManager.singleton_AlbumManager.PlayAnimation_SpinBackward();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            menuNavigation.x += 1;
            resetBlocks();
            CheckNavigationLimits();

            if(currentMenu==State.Home) {RepositionAlbums();}
            //Play Animation
            SelectionAlbumManager.singleton_AlbumManager.PlayAnimation_SpinForward();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            menuNavigation.y += 1;
            CheckNavigationLimits();

            resetBlocks();
            if(currentMenu==State.Home) {RepositionAlbums();}
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            menuNavigation.y -= 1;
            CheckNavigationLimits();
            resetBlocks();
            if(currentMenu==State.Home) {RepositionAlbums();}
        }
    }

    
    public void resetBlocks()
    {
        /*
        for (int i = 0; i < userInterface.transform.childCount; i++)
        {
            foreach (Transform child in userInterface.transform.GetChild(i).transform)
            {
                child.GetComponent<Image>().color = Color.white;
            }
        }
        */

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
        userInterfaceScript.enableHome();
        //Enable Home UI
        ChangeState(State.Home);
        userInterface.SetActive(true);
    }
    
    public void ResetMenu()
    {
        userInterfaceScript.enableHome();
        //Enable Home UI
        ChangeState(State.Home);
        userInterface.transform.GetChild(0).gameObject.SetActive(true);
        ourActionSlotManager.WipeActions();
    }

    public void DisableMenu()
    {
        ChangeState(State.Disabled);
        userInterface.transform.GetChild(0).gameObject.SetActive(false);
        //Disable Certain Features
    }

    private void AwaitingStart()
    {
        //For TESTING
        if (Input.GetKeyDown("r") && ourActionSlotManager.Actions.Count>0) //Create Action 
        {
            CombatManager.AwaitAttack();
            resetBlocks();
            DisableMenu();
        }
        
        if (Input.GetKeyDown("e") && ourActionSlotManager.Actions.Count>0) //Create Action 
        {
            ActionSlotManager.singleton_ActionSlotManager.ErasePreviousAction();
        }
    }

    protected virtual void Update()
    {
        switch (currentMenu)
        {
            case State.Home:
                //Change Previous State
                ChangePreviousState(State.Home);
                //Set our Action Block to our Home Menu and SetActive
                actionBlock = userInterfaceScript.homeCanvas;
                userInterfaceScript.enableHome();
                //Allow Player to Maneuver the menu
                if (navigationOnCooldown == false)
                {
                    maneuverMenu();
                } 
                //Turn Back On When Ready to Test Magic

                //Reset Size on All Monsters
                ResetMonsters();
                //Call Home Menu Function
                DeployHomeMenu();
                //Change our Navigation Limit
                NavigationLimit = new Vector3(3, 0,0);
                //On Hover Effect
                //actionBlock.transform.GetChild((int) menuNavigation.x).GetComponent<Image>().color=Color.red;
                //Allow Player to erase Previous action
                
                AwaitingStart();
                break;
            case State.Attack:
                maneuverMenu();
                userInterfaceScript.disableHome();
                break;
            case State.Magic:
                userInterfaceScript.disableHome();
                maneuverMenu();
                break;
            case State.Items:
                userInterfaceScript.disableHome();
                maneuverMenu();
                break;
            case State.Flee:
                userInterfaceScript.disableHome();
                maneuverMenu();
                break;
            case State.EnemyTurn:

                break;
            case State.Disabled:

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
                userInterfaceScript.enableAttackMenu();
                actionBlock =  userInterfaceScript.BodyPartText;;
                //Change the state of our Attack Menu Script
                ourAttackMenu.ChangeState(AttackMenu.State.SelectMonster);
                //Change our menu navigation starting point dependent on how many enemies are on screen
                MenuStartingPoint();
                //Play Feedbacks
                SelectionAlbumManager.singleton_AlbumManager.SelectAttackAction();
            }
        }
        /*
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
                actionBlock = userInterfaceScript.magicCanvas;
                userInterfaceScript.enableMagicMenu();
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
                actionBlock = userInterfaceScript.itemCanvas;
                userInterfaceScript.enableItemMenu();
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
                userInterfaceScript.enableConfirmationmMenu();
                //Change the state of our Item Menu Script
                ourFleeMenu.ChangeState(FleeMenu.State.Confirm);
            }
            
        }
        */
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

    public void CheckNavigationLimits()
    {
                
        //Set for a Max size of 2
        if (menuNavigation.x > NavigationLimit.x) { menuNavigation.x = 0; Debug.Log("Breached Navigation Limit");}
        else if (menuNavigation.x < 0) { menuNavigation.x = NavigationLimit.x; Debug.Log("Breached Navigation Limit");}
        
        if (menuNavigation.y > NavigationLimit.y) { menuNavigation.y = NavigationLimit.y; Debug.Log("Breached Navigation Limit");}
        else if (menuNavigation.y < NavigationLimit.z) { menuNavigation.y = NavigationLimit.z; Debug.Log("Breached Navigation Limit");}

    }

    public void RepositionAlbums()
    {
        navigationOnCooldown = true;
        StartCoroutine(NavigationCooldown(.25f)); //Navigation Speed
        Transform ourAttackAlbumTransform = SelectionAlbumManager.singleton_AlbumManager.attackAlbumTransform;
        Transform ourMagicAlbumTransform = SelectionAlbumManager.singleton_AlbumManager.magicAlbumTransform;

        Transform ourItemAlbumTransform = SelectionAlbumManager.singleton_AlbumManager.itemAlbumTransform;
        Transform ourEscapeAlbumTransform = SelectionAlbumManager.singleton_AlbumManager.escapeAlbumTransform;

        if (menuNavigation.x == 0) //Attack Album Hovered
        {
            SelectionAlbumManager.singleton_AlbumManager.MovePrimaryAlbum(ourAttackAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MoveThirdAlbum(ourMagicAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MoveSecondaryAlbum(ourItemAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MoveFourthAlbum(ourEscapeAlbumTransform);
        }
        if (menuNavigation.x == 1) //Magic Album Hovered
        {
            SelectionAlbumManager.singleton_AlbumManager.MoveSecondaryAlbum(ourAttackAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MovePrimaryAlbum(ourMagicAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MoveFourthAlbum(ourItemAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MoveThirdAlbum(ourEscapeAlbumTransform);

        }
        if (menuNavigation.x == 2) //Escape Album Hovered
        {
            SelectionAlbumManager.singleton_AlbumManager.MoveFourthAlbum(ourAttackAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MoveSecondaryAlbum(ourMagicAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MoveThirdAlbum(ourItemAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MovePrimaryAlbum(ourEscapeAlbumTransform);


        }
        if (menuNavigation.x == 3) //Item Album Hovered
        {
            SelectionAlbumManager.singleton_AlbumManager.MoveThirdAlbum(ourAttackAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MoveFourthAlbum(ourMagicAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MovePrimaryAlbum(ourItemAlbumTransform);
            SelectionAlbumManager.singleton_AlbumManager.MoveSecondaryAlbum(ourEscapeAlbumTransform);

        }
    }

    public IEnumerator NavigationCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        navigationOnCooldown = false;
    }
}
