using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Vector2 = System.Numerics.Vector2;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject userInterface;
    private GameObject actionBlock;
    protected State currentMenu;
    protected int menuNumber;
    private int menuNavigation;
    protected enum State
    {
        Home,
        Attack,
        Magic,
        Items,
        Flee,
        EnemyTurn,
    }

    private void Start()
    {
        menuNavigation = 0;
    }

    protected void ChangeState(State state)
    {
        currentMenu = state;
        //Hide UI
    }
    // Update is called once per frame
    void Update()
    {
        switch (currentMenu)
        {
            case State.Home:
                actionBlock = userInterface.transform.GetChild(0).gameObject;
                menuNumber = 1;
                maneuverMenu();
                HomeButtons();
                break;
            case State.Attack:
                actionBlock = userInterface.transform.GetChild(1).GetChild(0).gameObject;
                menuNumber = 2;
                maneuverMenu();
                AttackButtons();
                break;
            case State.Magic:
                Debug.Log("In Magic State");
                break;
            case State.Items:
                Debug.Log("In Items State");
                break;
            case State.Flee:
                Debug.Log("In Flee State");
                break;
            case State.EnemyTurn:

                break;
        }
    }

    private void maneuverMenu()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            menuNavigation -= 1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            menuNavigation += 1;
        }
        
        //Set for a Max size of 2
        if (menuNavigation > actionBlock.transform.childCount-1) { menuNavigation = actionBlock.transform.childCount-1; }
        else if (menuNavigation < 0) { menuNavigation = 0; }

        //Current Menu Player is Hovering Over
        if (menuNavigation == 0) //Attack
        {
            resetBlocks();
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Attack);
            }
            actionBlock.transform.GetChild(0).GetComponent<Image>().color=Color.red;
            
        }
        else if (menuNavigation==1) //Magic
        {
            resetBlocks();
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Magic);
            }
            actionBlock.transform.GetChild(1).GetComponent<Image>().color=Color.red;
        }
        else if (menuNavigation==2) //Items
        {
            resetBlocks();
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Items);
            }
            actionBlock.transform.GetChild(2).GetComponent<Image>().color=Color.red;
        }
        else if (menuNavigation==3) //Flee
        {
            resetBlocks();
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Flee);
            }
            actionBlock.transform.GetChild(3).GetComponent<Image>().color=Color.red;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuNavigation = 0;
            resetBlocks();
            ChangeState(State.Home);
        }
    }

    private void HomeButtons()
    {
        //Current Menu Player is Hovering Over
        if (menuNavigation == 0) //Attack
        {
            resetBlocks();
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Attack);
            }
            actionBlock.transform.GetChild(0).GetComponent<Image>().color=Color.red;
            
        }
        else if (menuNavigation==1) //Magic
        {
            resetBlocks();
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Magic);
            }
            actionBlock.transform.GetChild(1).GetComponent<Image>().color=Color.red;
        }
        else if (menuNavigation==2) //Items
        {
            resetBlocks();
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Items);
            }
            actionBlock.transform.GetChild(2).GetComponent<Image>().color=Color.red;
        }
        else if (menuNavigation==3) //Flee
        {
            resetBlocks();
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Flee);
            }
            actionBlock.transform.GetChild(3).GetComponent<Image>().color=Color.red;
        }
    }

    private void AttackButtons()
    {
        //Current Menu Player is Hovering Over
        if (menuNavigation == 0) //Attack
        {
            resetBlocks();
            if (Input.GetKeyDown("space")) 
            {
                //Send Attack Data to combat Manager
            }
            actionBlock.transform.GetChild(0).GetComponent<Image>().color=Color.red;
            
        }
        //Current Menu Player is Hovering Over
        if (menuNavigation == 1) //Attack
        {
            resetBlocks();
            if (Input.GetKeyDown("space")) 
            {
                //Send Attack Data to combat Manager
            }
            actionBlock.transform.GetChild(0).GetComponent<Image>().color=Color.red;
            
        }
    }
    
    private void resetBlocks()
    {
        foreach (Transform child in actionBlock.transform)
        {
            child.GetComponent<Image>().color=Color.white;
        }
    }

    private void FixedUpdate()
    {
    }
}
