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
    [SerializeField] private GameObject actionBlock;
    protected State currentMenu;
    private Vector2 menuNavigation;
    protected enum State
    {
        Home,
        Attack,
        Magic,
        Items,
        Flee
    }

    private void Start()
    {
        menuNavigation.X = 0;
        menuNavigation.Y = 1;
    }

    protected void ChangeState(State state)
    {
        currentMenu = state;
    }
    // Update is called once per frame
    void Update()
    {
        switch (currentMenu)
        {
            case State.Home:
                maneuverMenu();
                Debug.Log("Menu Location:"+menuNavigation.X+","+menuNavigation.Y);
                break;
            case State.Attack:

                break;
            case State.Magic:

                break;
            case State.Items:

                break;
            case State.Flee:

                break;
        }
    }

    private void maneuverMenu()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            menuNavigation.Y -= 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            menuNavigation.X -= 1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            menuNavigation.X += 1;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            menuNavigation.Y += 1;
        }
        
        //Set for a Max size of 2
        if (menuNavigation.X > 1) { menuNavigation.X = 1; }
        else if (menuNavigation.X < 0) { menuNavigation.X = 0; }

        if (menuNavigation.Y > 1) { menuNavigation.Y = 1; }
        else if (menuNavigation.Y < 0) { menuNavigation.Y = 0; }
        
        //Current Menu Player is Hovering Over
        if (menuNavigation.X == 0 && menuNavigation.Y == 1) //Attack
        {
            resetBlocks();
            actionBlock.transform.GetChild(0).GetComponent<Image>().color=Color.red;
        }
        else if (menuNavigation.X == 1 && menuNavigation.Y == 1) //Magic
        {
            resetBlocks();
            actionBlock.transform.GetChild(1).GetComponent<Image>().color=Color.red;
        }
        else if (menuNavigation.X == 0 && menuNavigation.Y == 0) //Items
        {
            resetBlocks();
            actionBlock.transform.GetChild(2).GetComponent<Image>().color=Color.red;
        }
        else if (menuNavigation.X == 1 && menuNavigation.Y == 0) //Flee
        {
            resetBlocks();
            actionBlock.transform.GetChild(3).GetComponent<Image>().color=Color.red;
        }
    }

    private void resetBlocks()
    {
        actionBlock.transform.GetChild(0).GetComponent<Image>().color=Color.white;
        actionBlock.transform.GetChild(1).GetComponent<Image>().color=Color.white;
        actionBlock.transform.GetChild(2).GetComponent<Image>().color=Color.white;
        actionBlock.transform.GetChild(3).GetComponent<Image>().color=Color.white;

    }

    private void FixedUpdate()
    {
    }
}
