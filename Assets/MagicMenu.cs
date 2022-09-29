using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

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
    //Bool
    private bool stateGate;
    
    private GameObject ourPlayer;


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
        int[] playerSpells = ourPlayer.GetComponent<PlayerScript>().allocatedSpells;
        for (int i = 0; i < playerSpells.Length; i++)
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
        GenerateSpells();
        //Set our navigation limits
        ourUI.NavigationLimit = new Vector3(3, 0,-1);
        ourUI.previousMenu = UIManager.State.Home;
        int hoveredItem = 0;

        //Current Menu Player is Hovering Over
        if (ourUI.menuNavigation.x == 0 && ourUI.menuNavigation.y == 0) //Attack
        {
            hoveredItem = 0;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 1 && ourUI.menuNavigation.y == 0) //Magic
        {
            hoveredItem = 1;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 2 && ourUI.menuNavigation.y == 0) //Items
        {
            hoveredItem = 2;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 3 && ourUI.menuNavigation.y == 0) //Flee
        {
            hoveredItem = 3;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 0 && ourUI.menuNavigation.y == -1) //Magic
        {
            hoveredItem = 4;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 1 && ourUI.menuNavigation.y == -1) //Items
        {
            hoveredItem = 5;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 2 && ourUI.menuNavigation.y == -1) //Flee
        {
            hoveredItem = 6;
            PrepareSpell(hoveredItem);
        }
        else if (ourUI.menuNavigation.x == 3 && ourUI.menuNavigation.y == -1) //Flee
        {
            hoveredItem = 7;
            PrepareSpell(hoveredItem);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideMagicMenu();
            ourUI.RestartMenu();
            ourUI.ChangeState(UIManager.State.Home);
            ChangeState(State.Inactive);
            ourUI.menuNavigation.x = ourUI.lastMenuNavigation.x;
            ourUI.resetBlocks();
        }
    }

    private void PrepareSpell(int hoveredItem)
    {
        //Hover Effect
        ourUI.actionBlock.transform.GetChild(hoveredItem).GetComponent<Image>().color = Color.red;
        //State Variables
        int[] playerSpells = ourPlayer.GetComponent<PlayerScript>().allocatedSpells;
        
        if (Input.GetKeyDown("space"))
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

    public void HideMagicMenu()
    {
        ourMagicMenu.SetActive(false);
    }
    
    public void ShowMagicMenu()
    {
        ourMagicMenu.SetActive(true);
    }

    public void magicSelectMonster()
    {
        int hoveredMonster = 0;
        if (CombatManager.enemyCount <= 1)
        {
            //Hide Menu
            HideMagicMenu();
        }
        else if (CombatManager.enemyCount == 2)
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
        else if (CombatManager.enemyCount == 3)
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
        
        PrepareMonster(hoveredMonster);
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Reset Enemy Size
            for (int i = 0; i < ourUI.encounteredEnemies.transform.childCount; i++)
            {
                ourUI.encounteredEnemies.transform.GetChild(i).transform.localScale = new Vector3(1, 1, 1);
            }
            //Reset Variables
            ourUI.menuNavigation.x = 0;
            ShowMagicMenu();
            //Return to previous state
            ChangeState(State.SpellList);
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
            
        if (hoveredMonster == ourUI.menuNavigation.x)
        {
            if (Input.GetKeyDown("space"))
            {
                UIManager.selectedMonster = ourUI.encounteredEnemies.transform.GetChild(hoveredMonster).gameObject;
                ourUI.ResetMonsters();
                //Cast Magic Damage
                CombatManager.CastSpell(UIManager.selectedMonster.GetComponent<MonsterData>(),selectedSpell.Damage);
                //Change State
                ChangeState(State.Inactive);
            }
        }
    }

    
    
    public void ChangeState(State state)
    {
        MenuState = state;
        Debug.Log("Going to State: "+state);
    }

}
