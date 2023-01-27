using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using MoreMountains.Feedbacks;


public class MagicMenu : MonoBehaviour
{
    //State
    [SerializeField] public State MenuState;
    private UIManager ourUI;
    //Spells
    private SpellDictionary ourSpellDictionary;
    private Spell selectedSpell;
    //GameObjects
    private GameObject ourMagicMenu;
    //Scripts
    private UserInterface userInterfaceScript;
    //Bool
    private bool stateGate;
    private GameObject ourPlayer;
    private bool hoverEffectPlayed;
    
    public enum State
    {
        Inactive,
        SpellList,
        SelectMonster,
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ourUI = gameObject.GetComponent<UIManager>();
        userInterfaceScript = UserInterface.singleton_UserInterface.GetComponent<UserInterface>();
        ourSpellDictionary = GameObject.Find("SpellDictionary").GetComponent<SpellDictionary>();
        ourMagicMenu = GameObject.Find("UserInterface").transform.GetChild(2).GameObject();
        ourPlayer = GameObject.Find("Player");
    }

    
    private void Update()
    {
        switch (MenuState)
        {
            case State.Inactive:
                stateGate = false;
                break;
            case State.SpellList:
                if (stateGate == true)
                {
                    DeployMagicMenu();
                }
                else
                {
                    stateGate = true;
                }
                break;
            case State.SelectMonster:
                magicSelectMonster();
                break;
        }
    }

    //State Magic List
    private void GenerateSpells() //Generate the List of Player Spells
    {
        //Look at our player spells
        int[] playerSpells = ourPlayer.GetComponent<PlayerScript>().allocatedSpells;
        for (int i = 0; i < playerSpells.Length; i++) //Throw each spell into a slot in our menu
        {
            GameObject tempSpell = ourUI.actionBlock.transform.GetChild(i).gameObject;
            tempSpell.SetActive(true);
            if (ourSpellDictionary.spellPool[playerSpells[i]] == null)
            {
                tempSpell.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Empty Slot";
            }
            else
            {
                tempSpell.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    ourSpellDictionary.spellPool[playerSpells[i]].GetComponent<Spell>().spellName;
            }
        }
    }


    //Home
    public void DeployMagicMenu() //Display the list to the player
    {
        //Generate player spells
        GenerateSpells();
        //Set our navigation limits
        ourUI.NavigationLimit = new Vector3(3, 0,-1);
        //Change UI manager State
        ourUI.previousMenu = UIManager.State.Home;
        //Initialize Variables
        int hoveredItem = 0;

        //Current Menu Player is Hovering Over
        if (ourUI.menuNavigation.x == 0 && ourUI.menuNavigation.y == 0) 
        {
            hoveredItem = 0;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 1 && ourUI.menuNavigation.y == 0) 
        {
            hoveredItem = 1;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 2 && ourUI.menuNavigation.y == 0) 
        {
            hoveredItem = 2;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 3 && ourUI.menuNavigation.y == 0) 
        {
            hoveredItem = 3;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 0 && ourUI.menuNavigation.y == -1) 
        {
            hoveredItem = 4;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 1 && ourUI.menuNavigation.y == -1) 
        {
            hoveredItem = 5;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 2 && ourUI.menuNavigation.y == -1) 
        {
            hoveredItem = 6;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 3 && ourUI.menuNavigation.y == -1) 
        {
            hoveredItem = 7;
            PrepareSpell(hoveredItem);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) //Leave menu on Escape
        {
            HideMagicMenu();
            ourUI.RestartMenu();
            ourUI.ChangeState(UIManager.State.Home);
            ChangeState(State.Inactive);
            ourUI.menuNavigation.x = ourUI.lastMenuNavigation.x;
            ourUI.resetBlocks();
        }
    }

    private void PrepareSpell(int hoveredItem) //If player is hovering spell
    {
        //Hover Effect
        ourUI.actionBlock.transform.GetChild(hoveredItem).GetComponent<Image>().color = Color.red;
        //State Variables
        int[] playerSpells = ourPlayer.GetComponent<PlayerScript>().allocatedSpells;
        
        if (Input.GetKeyDown("space")) //Select spell on pressing space
        {
            if (playerSpells[hoveredItem] != 0)
            {
                selectedSpell = ourSpellDictionary.spellPool[playerSpells[hoveredItem]].GetComponent<Spell>();
                ourUI.MenuStartingPoint();
                ChangeState(State.SelectMonster);
            }
            else
            {
                Debug.Log("No Spell Allocated");
            }
        }
    }

    public void HideMagicMenu() //Hide our Magic Menu
    {
        ourMagicMenu.SetActive(false);
    }
    
    public void ShowMagicMenu() //Show our Magic Menu
    {
        ourMagicMenu.SetActive(true);
    }

    public void magicSelectMonster() //Monster Select State
    {
        //Initialize Variables
        int hoveredMonster = 0;
        if (CombatManager.enemyCount <= 1) //If only one enemy on screen
        {
            //Hide Menu
            HideMagicMenu();
        }
        else if (CombatManager.enemyCount == 2) //If two ememies on Screen
        {
            //Hide Menu
            HideMagicMenu();
            //Set Menu Limits
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
        else if (CombatManager.enemyCount == 3) //If Three enemies on Screen
        {
            //Hide Menu
            HideMagicMenu();
            //Set Menu Limits
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
        PrepareMonster(hoveredMonster); //On Hover Effects and Selected Monster Changee
        
        if (Input.GetKeyDown(KeyCode.Escape)) //Return to spell menu on pressing Escape
        {
            //Reset Monster Sizes
            ourUI.ResetMonsters();
            hoverEffectPlayed = false;
            //Reset Variables
            ourUI.menuNavigation.x = 0;
            ShowMagicMenu();
            //Return to previous state
            ChangeState(State.SpellList);
        }

    }
    
    
    private void PrepareMonster(int hoveredMonster) //If player is hovering monster
    {
        //Change Navigation Limit to amount of enemies on screen
        ourUI.NavigationLimit = new Vector3(ourUI.encounteredEnemies.transform.childCount-1, 0,-1);
        //If hovering over a monster do on screen effect (Enlarge Them)
        for (int i = 0; i < ourUI.encounteredEnemies.transform.childCount; i++)
        {
            if (i != hoveredMonster) //HoverExit
            {
                MMF_Player ourJuice = ourUI.encounteredEnemies.transform.GetChild(i).transform.GetChild(0).transform.GetChild(3).GetComponent<MMF_Player>();
                ourJuice.PlayFeedbacks();            }
            else
            {
                MMF_Player ourJuice = ourUI.encounteredEnemies.transform.GetChild(i).transform.GetChild(0).transform.GetChild(2).GetComponent<MMF_Player>();
                if(!hoverEffectPlayed) { ourJuice.PlayFeedbacks(); hoverEffectPlayed = true;}
            }
        }
        //If the player moves the menu
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            hoverEffectPlayed = false;
        }

        if (Input.GetKeyDown("space")) //If player presses space, select that monst
        {
            //Change Selected Monster
            UIManager.selectedMonster = ourUI.encounteredEnemies.transform.GetChild(hoveredMonster).gameObject;
            //Reset Monster Sizes
            ourUI.ResetMonsters();
            hoverEffectPlayed = false;
            //Cast Magic Damage
            CombatManager.AwaitMagic();
            CombatManager.SetTargetAndDamage(UIManager.selectedMonster,selectedSpell.Damage);
            //Change State
            ChangeState(State.Inactive);
        }

    }

    
    
    public void ChangeState(State state)
    {
        MenuState = state;
        Debug.Log("Going to State: "+state);
    }

}
