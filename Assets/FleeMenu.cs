using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FleeMenu : MonoBehaviour
{
    [SerializeField] public State MenuState;
    private UIManager ourUI;
    protected GameObject userInterface;
    //Bool
    private bool stateGate;
    public enum State
    {
        Inactive,
        Confirm,
    }
    // Start is called before the first frame update
    void Start()
    {
        ourUI = gameObject.GetComponent<UIManager>();
        userInterface = GameObject.Find("UserInterface");
    }

    // Update is called once per frame
    void Update()
    {
        switch (MenuState)
        {
            case State.Inactive:
                stateGate = false;
                break;
            case State.Confirm:
                if (stateGate == true)
                {
                    ConfirmWindow();
                }
                else
                {
                    stateGate = true;
                }
                break;
        }
    }
    
    private void ConfirmWindow()
    {
        //Action Block
        ourUI.actionBlock = userInterface.transform.GetChild(4).gameObject;
        //Confirm Text
        ourUI.actionBlock.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "Run Away ?";
        ourUI.NavigationLimit = new Vector3(1, 0,0);
        //Hover Effect
        ourUI.actionBlock.transform.GetChild((int)ourUI.menuNavigation.x).GetComponent<Image>().color = Color.red;
        //Enable Window
        userInterface.transform.GetChild(4).gameObject.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (ourUI.menuNavigation.x == 0) //Yes?
            {
                Debug.Log("You Run Away!");
            }
            else //No?
            {
                ourUI.menuNavigation.x = ourUI.lastMenuNavigation.x;
                //Disable Confirmation Window
                userInterface.transform.GetChild(4).gameObject.SetActive(false);
                //Re-enable Home Window
                userInterface.transform.GetChild(0).gameObject.SetActive(true);
                //Change state 
                ChangeState(State.Inactive);
                //Go Home
                ourUI.menuNavigation.x = 0;
                ourUI.RestartMenu();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) //Default No on Escape
        {
            ourUI.menuNavigation.x = ourUI.lastMenuNavigation.x;
            //Disable Confirmation Window
            userInterface.transform.GetChild(4).gameObject.SetActive(false);
            //Re-enable Home Window
            userInterface.transform.GetChild(0).gameObject.SetActive(true);
            //Change state 
            ChangeState(State.Inactive);
            //Go Home
            ourUI.menuNavigation.x = 0;
            ourUI.RestartMenu();
        }
        
    }
    public void ChangeState(State state)
    {
        MenuState = state;
        Debug.Log("Going to State: "+state);
    }
}
