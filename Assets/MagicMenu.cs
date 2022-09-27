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
    private GameObject combatManager;
    //Spells
    private SpellDictionary ourSpellDictionary;
    private Spell selectedSpell;
    public Vector2 newMenuNavigation;
    //GameObjects
    private GameObject ourMagicMenu;

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
        combatManager = GameObject.Find("CombatManager");
        ourSpellDictionary = GameObject.Find("SpellDictionary").GetComponent<SpellDictionary>();
        ourMagicMenu = GameObject.Find("UserInterface").transform.GetChild(2).GameObject();
    }
    private void Update()
    {
        switch (MenuState)
        {
            case State.Inactive:
                break;
            case State.SpellList:
                DeployMagicMenu();
                break;
            case State.SelectMonster:
                magicSelectMonster();
                break;
        }
    }

    public void ChangeState(State state)
    {
        MenuState = state;
    }
    
    //State Magic List
    private void GenerateSpells() //Generate the List of Player Spells
    {
        int[] playerSpells = UIManager.ourPlayer.GetComponent<PlayerScript>().allocatedSpells;
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
        int[] playerSpells = UIManager.ourPlayer.GetComponent<PlayerScript>().allocatedSpells;

        //Current Menu Player is Hovering Over
        if (ourUI.menuNavigation.x == 0 && ourUI.menuNavigation.y == 0) //Attack
        {
            ourUI.actionBlock.transform.GetChild(0).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space"))
            {
                if (playerSpells[0] != 0)
                {
                    selectedSpell = ourSpellDictionary.spellPool[playerSpells[0]].GetComponent<Spell>();
                    ChangeState(State.SelectMonster);
                }
                else
                {
                    Debug.Log("No Spell Allocated");
                }

            }
        }
        else if (ourUI.menuNavigation.x == 1 && ourUI.menuNavigation.y == 0) //Magic
        {
            ourUI.actionBlock.transform.GetChild(1).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space"))
            {
                if (playerSpells[1] != 0)
                {
                    selectedSpell = ourSpellDictionary.spellPool[playerSpells[1]].GetComponent<Spell>();
                    ChangeState(State.SelectMonster);
                }
                else
                {
                    Debug.Log("No Spell Allocated");
                }

            }

        }
        else if (ourUI.menuNavigation.x == 2 && ourUI.menuNavigation.y == 0) //Items
        {
            ourUI.actionBlock.transform.GetChild(2).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space"))
            {
                if (playerSpells[2] != 0)
                {
                    selectedSpell = ourSpellDictionary.spellPool[playerSpells[2]].GetComponent<Spell>();
                    ChangeState(State.SelectMonster);
                }
                else
                {
                    Debug.Log("No Spell Allocated");
                }
            }
        }
        else if (ourUI.menuNavigation.x == 3 && ourUI.menuNavigation.y == 0) //Flee
        {
            ourUI.actionBlock.transform.GetChild(3).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space"))
            {
                if (playerSpells[3] != 0)
                {
                    selectedSpell = ourSpellDictionary.spellPool[playerSpells[3]].GetComponent<Spell>();
                    ChangeState(State.SelectMonster);
                }
                else
                {
                    Debug.Log("No Spell Allocated");
                }
            }
        }
        else if (ourUI.menuNavigation.x == 0 && ourUI.menuNavigation.y == -1) //Magic
        {
            ourUI.actionBlock.transform.GetChild(4).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space"))
            {
                if (playerSpells[4] != 0)
                {
                    selectedSpell = ourSpellDictionary.spellPool[playerSpells[4]].GetComponent<Spell>();
                    ChangeState(State.SelectMonster);
                }
                else
                {
                    Debug.Log("No Spell Allocated");
                }
            }

        }
        else if (ourUI.menuNavigation.x == 1 && ourUI.menuNavigation.y == -1) //Items
        {
            ourUI.actionBlock.transform.GetChild(5).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space"))
            {
                if (playerSpells[5] != 0)
                {
                    selectedSpell = ourSpellDictionary.spellPool[playerSpells[5]].GetComponent<Spell>();
                    ChangeState(State.SelectMonster);
                }
                else
                {
                    Debug.Log("No Spell Allocated");
                }
            }
        }
        else if (ourUI.menuNavigation.x == 2 && ourUI.menuNavigation.y == -1) //Flee
        {
            ourUI.actionBlock.transform.GetChild(6).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space"))
            {
                if (playerSpells[6] != 0)
                {
                    selectedSpell = ourSpellDictionary.spellPool[playerSpells[6]].GetComponent<Spell>();
                    ChangeState(State.SelectMonster);
                }
                else
                {
                    Debug.Log("No Spell Allocated");
                }
            }
        }
        else if (ourUI.menuNavigation.x == 3 && ourUI.menuNavigation.y == -1) //Flee
        {
            ourUI.actionBlock.transform.GetChild(7).GetComponent<Image>().color = Color.red;
            if (Input.GetKeyDown("space"))
            {
                if (playerSpells[7] != 0)
                {
                    selectedSpell = ourSpellDictionary.spellPool[playerSpells[7]].GetComponent<Spell>();
                    ChangeState(State.SelectMonster);
                }
                else
                {
                    Debug.Log("No Spell Allocated");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideMagicMenu();
            ourUI.RestartMenu();
            ourUI.ChangeState(UIManager.State.Home);
            ChangeState(State.Inactive);
            ourUI.menuNavigation.x = 0;
            ourUI.resetBlocks();
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
        if (CombatManager.enemyCount <= 1)
        {
            //Hide Menu
            HideMagicMenu();
            //Hovering Over
            ourUI.NavigationLimit = new Vector3(0, 0,0);
            ourUI.encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(2, 2, 1);
            if (Input.GetKeyDown("space"))
            {
                UIManager.selectedMonster = ourUI.encounteredEnemies.transform.GetChild(0).gameObject;
                ourUI.ResetMonsters();
                //Cast Magic Damage
                CombatManager.CastSpell(UIManager.selectedMonster.GetComponent<MonsterData>(),selectedSpell.Damage);
                //Change State
                ChangeState(State.Inactive);
            }

        }
        else if (CombatManager.enemyCount == 2)
        {
            //Hide Menu
            HideMagicMenu();
            //Set Menu Limits
            ourUI.NavigationLimit = new Vector3(1, 0,0);
            if (ourUI.menuNavigation.x == 0)
            {
                ourUI.encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(2, 2, 1);
                ourUI.encounteredEnemies.transform.GetChild(1).transform.localScale = new Vector3(1, 1, 1);
                if (Input.GetKeyDown("space"))
                {
                    UIManager.selectedMonster = ourUI.encounteredEnemies.transform.GetChild(0).gameObject;
                    ourUI.ResetMonsters();
                    //Cast Magic Damage
                    CombatManager.CastSpell(UIManager.selectedMonster.GetComponent<MonsterData>(),selectedSpell.Damage);
                    //Change State
                    ChangeState(State.Inactive);
                }
            }
            else
            {
                ourUI.encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
                ourUI.encounteredEnemies.transform.GetChild(1).transform.localScale = new Vector3(2, 2, 1);
                if (Input.GetKeyDown("space"))
                {
                    UIManager.selectedMonster = ourUI.encounteredEnemies.transform.GetChild(1).gameObject;
                    ourUI.ResetMonsters();
                    //Cast Magic Damage
                    CombatManager.CastSpell(UIManager.selectedMonster.GetComponent<MonsterData>(),selectedSpell.Damage);
                    //Change State
                    ChangeState(State.Inactive);
                }
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
                ourUI.encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(2, 2, 1);
                ourUI.encounteredEnemies.transform.GetChild(1).transform.localScale = new Vector3(1, 1, 1);
                ourUI.encounteredEnemies.transform.GetChild(2).transform.localScale = new Vector3(1, 1, 1);
                if (Input.GetKeyDown("space"))
                {
                    UIManager.selectedMonster = ourUI.encounteredEnemies.transform.GetChild(0).gameObject;
                    ourUI.ResetMonsters();
                    //Cast Magic Damage
                    CombatManager.CastSpell(UIManager.selectedMonster.GetComponent<MonsterData>(),selectedSpell.Damage);
                    //Change State
                    ChangeState(State.Inactive);
                }
            }
            else if (ourUI.menuNavigation.x == 2)
            {
                ourUI.encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
                ourUI.encounteredEnemies.transform.GetChild(2).transform.localScale = new Vector3(2, 2, 1);
                ourUI.encounteredEnemies.transform.GetChild(1).transform.localScale = new Vector3(1, 1, 1);
                if (Input.GetKeyDown("space"))
                {
                    UIManager.selectedMonster = ourUI.encounteredEnemies.transform.GetChild(2).gameObject;
                    ourUI.ResetMonsters();
                    //Cast Magic Damage
                    CombatManager.CastSpell(UIManager.selectedMonster.GetComponent<MonsterData>(),selectedSpell.Damage);
                    //Change State
                    ChangeState(State.Inactive);
                }
            }
            else if (ourUI.menuNavigation.x == 0)
            {
                ourUI.encounteredEnemies.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
                ourUI.encounteredEnemies.transform.GetChild(2).transform.localScale = new Vector3(1, 1, 1);
                ourUI.encounteredEnemies.transform.GetChild(1).transform.localScale = new Vector3(2, 2, 1);
                if (Input.GetKeyDown("space"))
                {
                    UIManager.selectedMonster = ourUI.encounteredEnemies.transform.GetChild(1).gameObject;
                    ourUI.ResetMonsters();
                    //Cast Magic Damage
                    CombatManager.CastSpell(UIManager.selectedMonster.GetComponent<MonsterData>(),selectedSpell.Damage);
                    //Change State
                    ChangeState(State.Inactive);
                }
            }
        }
        

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
    
}
