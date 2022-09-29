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
    
    public enum State
    {
        Inactive,
        ItemList,
        SelectMonster,
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
        }
    }
    
    
    private void GenerateItems()
    {
        int[] playerItems = ourPlayer.GetComponent<PlayerScript>().allocatedItems;
        ourUI.actionBlock.gameObject.SetActive(true);
        for (int i = 0; i < playerItems.Length; i++)
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
        GenerateItems();
        ourUI.NavigationLimit = new Vector3(3, 0,-1);
        int hoveredItem = 0;
        
        //Current Menu Player is Hovering Over
        if (ourUI.menuNavigation.x == 0 && ourUI.menuNavigation.y == 0) //Attack
        {
            hoveredItem = 0;
            prepareItem(hoveredItem);
        }
        else if (ourUI.menuNavigation.x==1 && ourUI.menuNavigation.y == 0) //Magic
        {
            hoveredItem = 1;
            prepareItem(hoveredItem);
        }
        else if (ourUI.menuNavigation.x==2 && ourUI.menuNavigation.y == 0) //Items
        {
            hoveredItem = 2;
            prepareItem(hoveredItem);
        }
        else if (ourUI.menuNavigation.x==3 && ourUI.menuNavigation.y == 0) //Flee
        {
            hoveredItem = 3;
            prepareItem(hoveredItem);
        }        
        else if (ourUI.menuNavigation.x==0 && ourUI.menuNavigation.y == -1) //Magic
        {
            hoveredItem = 4;
            prepareItem(hoveredItem);
        }
        else if (ourUI.menuNavigation.x==1 && ourUI.menuNavigation.y == -1) //Items
        {
            hoveredItem = 5;
            prepareItem(hoveredItem);
        }
        else if (ourUI.menuNavigation.x==2 && ourUI.menuNavigation.y == -1) //Flee
        {
            hoveredItem = 6;
            prepareItem(hoveredItem);
        }
        else if (ourUI.menuNavigation.x==3 && ourUI.menuNavigation.y == -1) //Flee
        {
            hoveredItem = 7;
            prepareItem(hoveredItem);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ourUI.RestartMenu();
            ourUI.menuNavigation.x = 2;
            HideItemMenu();
            ourUI.resetBlocks();
        }

    }

      public void prepareItem(int hoveredItem)
      {
          //State Variables
          int[] playerItems = ourPlayer.GetComponent<PlayerScript>().allocatedItems;
          //Hover Effect
          ourUI.actionBlock.transform.GetChild(hoveredItem).GetComponent<Image>().color = Color.red;
          if (Input.GetKeyDown(KeyCode.Space))
          {
              if (playerItems[hoveredItem] != 0)
              {
                  selectedItem = ourItemDictionary.itemPool[playerItems[hoveredItem]].GetComponent<Item>();
                  CombatManager.UseItem(ourItemDictionary.itemPool[playerItems[hoveredItem]]);
                  playerItems[hoveredItem] = 0;
                  HideItemMenu();
              }
              else
              { 
                  Debug.Log("No Item Allocated");
              }
          }
      }

      private void HideItemMenu()
      {
          ChangeState(State.Inactive);
          ourItemMenu.SetActive(false);
      }

          
      public void ChangeState(State state)
      {
          MenuState = state;
          Debug.Log("Going to State: "+state);
      }

}
