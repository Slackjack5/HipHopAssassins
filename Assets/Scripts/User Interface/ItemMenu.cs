using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemMenu : MonoBehaviour
{
    protected GameObject userInterface;
    //State
    [SerializeField] public State MenuState;
    private UIManager ourUI;
    //Bool
    private bool stateGate;
    //items
    private ItemDictionary ourItemDictionary;
    private GameObject ourItemMenu;
    private Item selectedItem;
    //Player
    private GameObject ourPlayer;
    //Ints
    int hoveredItem = 0;
    public enum State
    {
        Inactive,
        ItemList,
        SelectMonster,
        Confirm,
    }

    // Start is called before the first frame update
    void Start()
    {
        userInterface = GameObject.Find("UserInterface");
        ourUI = gameObject.GetComponent<UIManager>();
        ourItemDictionary = GameObject.Find("ItemDictionary").GetComponent<ItemDictionary>();
        ourPlayer = GameObject.Find("Player");
        ourItemMenu = userInterface.transform.GetChild(3).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        switch (MenuState)
        {
            case State.Inactive:
                stateGate = false;
                break;
            case State.ItemList:
                if (stateGate == true)
                {
                    DeployItemMenu();
                }
                else
                {
                    stateGate = true;
                }
                break;
            case State.SelectMonster:


                break;
            case State.Confirm:
                ConfirmWindow(hoveredItem);
                break;
        }
    }
    
    
    private void GenerateItems() //Generate the List of Player Items
    {
        //Look at our player Items
        int[] playerItems = ourPlayer.GetComponent<PlayerScript>().allocatedItems;
        ourUI.actionBlock.gameObject.SetActive(true);
        for (int i = 0; i < playerItems.Length; i++) //Throw each item into a slot in our menu
        {
            GameObject tempItem = ourUI.actionBlock.transform.GetChild(i).gameObject;
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
    
      public void DeployItemMenu()
    {        
        //Generate player items
        GenerateItems();
        //Set Navigation Limit and Previous Menu Navigation
        ourUI.NavigationLimit = new Vector3(3, 0,-1);
        ourUI.lastMenuNavigation.x = ourUI.menuNavigation.x;
        //Current Menu Player is Hovering Over
        if (ourUI.menuNavigation.x == 0 && ourUI.menuNavigation.y == 0) 
        {
            hoveredItem = 0;
        }
        else if (ourUI.menuNavigation.x==1 && ourUI.menuNavigation.y == 0) 
        {
            hoveredItem = 1;
        }
        else if (ourUI.menuNavigation.x==2 && ourUI.menuNavigation.y == 0) 
        {
            hoveredItem = 2;
        }
        else if (ourUI.menuNavigation.x==3 && ourUI.menuNavigation.y == 0) 
        {
            hoveredItem = 3;
        }        
        else if (ourUI.menuNavigation.x==0 && ourUI.menuNavigation.y == -1) 
        {
            hoveredItem = 4;
        }
        else if (ourUI.menuNavigation.x==1 && ourUI.menuNavigation.y == -1) 
        {
            hoveredItem = 5;
        }
        else if (ourUI.menuNavigation.x==2 && ourUI.menuNavigation.y == -1) 
        {
            hoveredItem = 6;
        }
        else if (ourUI.menuNavigation.x==3 && ourUI.menuNavigation.y == -1) 
        {
            hoveredItem = 7;
        }
        //Prepare our Item
        prepareItem(hoveredItem); 
        if (Input.GetKeyDown(KeyCode.Escape)) //Return to the home menu on escape
        {
            ourUI.RestartMenu();
            ourUI.menuNavigation.x = 2;
            HideItemMenu();
            ourUI.resetBlocks();
        }

    }

      public void prepareItem(int hoveredItem) //If player is hovering item
      {
          //State Variables
          int[] playerItems = ourPlayer.GetComponent<PlayerScript>().allocatedItems;
          //Hover Effect
          ourUI.actionBlock.transform.GetChild(hoveredItem).GetComponent<Image>().color = Color.red;
          if (Input.GetKeyDown(KeyCode.Space)) //Select item on pressing space
          {
              if (playerItems[hoveredItem] != 0)
              {
                  HideItemMenu();
                  ourUI.menuNavigation.x = 0;
                  ChangeState(State.Confirm);
              }
              else
              { 
                  Debug.Log("No Item Allocated");
              }
          }
      }

      private void ConfirmWindow(int hoveredItem) //Confirm your choice
      {
          //State Variables
          int[] playerItems = ourPlayer.GetComponent<PlayerScript>().allocatedItems;
          //Action Block
          ourUI.actionBlock = userInterface.transform.GetChild(4).gameObject;
          //Chosen Item
          selectedItem = ourItemDictionary.itemPool[playerItems[hoveredItem]].GetComponent<Item>();
          //Confirm Text
          ourUI.actionBlock.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "Consume " + selectedItem.itemName+" ?";
          ourUI.NavigationLimit = new Vector3(1, 0,0);
          //Hover Effect
          ourUI.actionBlock.transform.GetChild((int)ourUI.menuNavigation.x).GetComponent<Image>().color = Color.red;
          //Enable Window
          userInterface.transform.GetChild(4).gameObject.SetActive(true);
          if (Input.GetKeyDown(KeyCode.Space))
          {
              if (ourUI.menuNavigation.x == 0) //Yes?
              {
                  //Ask for Conformation
                  CombatManager.UseItem(ourItemDictionary.itemPool[playerItems[hoveredItem]]);
                  playerItems[hoveredItem] = 0;
                  HideItemMenu();
              }
              else //No?
              {
                  HideItemMenu();
                  ourUI.menuNavigation.x = ourUI.lastMenuNavigation.x;
                  ourUI.actionBlock = userInterface.transform.GetChild(3).gameObject;
                  ChangeState(State.ItemList);
              }
          }
          else if (Input.GetKeyDown(KeyCode.Escape)) //Default No on Escape
          {
              HideItemMenu();
              ourUI.menuNavigation.x = ourUI.lastMenuNavigation.x;
              ourUI.actionBlock = userInterface.transform.GetChild(3).gameObject;
              ChangeState(State.ItemList);
          }
      }

      private void HideItemMenu() //Hide the Item Menu
      {
          ChangeState(State.Inactive);
          ourItemMenu.SetActive(false);
          //Disable Confirmation Window
          userInterface.transform.GetChild(4).gameObject.SetActive(false);
      }

          
      public void ChangeState(State state) //Change States
      {
          MenuState = state;
          Debug.Log("Going to State: "+state);
      }

}
