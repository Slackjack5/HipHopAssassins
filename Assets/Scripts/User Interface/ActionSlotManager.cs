using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSlotManager : MonoBehaviour
{
    public static ActionSlotManager singleton_ActionSlotManager;
    public List<AttackAction> Actions = new List<AttackAction>(4);
    
    public int selectedAction = 0;

    public GameObject AttackGameObject;
    public int SequenceCost;

    private GameObject actionGrid;

    private int nextSlot;
    
    private void Awake()
    {
        if (singleton_ActionSlotManager != null && singleton_ActionSlotManager != this)
        {
            Destroy(this);
        }
        else
        {
            singleton_ActionSlotManager = this;
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        actionGrid = GameObject.Find("ActionGridCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        HighlightCurrentBar();
    }

    public void SpawnAttackAction(GameObject SelectedMonster, float SelectedLimb)
    {
        if (Actions.Count < 4)
        {
            //Prepare 
            nextSlot = Actions.Count;
            GameObject ourNewAction = Instantiate(AttackGameObject); //Spawn Object
            //ourNewAction.GetComponent<Image>().SetNativeSize();
            AttackAction ourAttackAction = ourNewAction.GetComponent<AttackAction>();
            //Add Action to List
            ourAttackAction.SelectedMonster = SelectedMonster;
            ourAttackAction.SelectedLimb = SelectedLimb;
            Actions.Add(ourAttackAction);
            //Edit Cost
            Actions[0].actionCost = 0; //Make the First Attack Action Free
            CalculateSequenceCost();
            //Allocate Resources
            if (PlayerScript.singleton_Player.actionPoints >= SequenceCost) //If Player assigns and has enough.
            {
                
            }
            else //If player assigns action but doesnt have enough
            {
                ourAttackAction.RiskyAttack = true;
            }
            
            //Move Object
            ourNewAction.transform.SetParent(actionGrid.transform.GetChild(nextSlot)); //Set Parent
            ourNewAction.GetComponent<Image>().rectTransform.localPosition = new Vector3(0, 0, 0); //Set Position
            
            //Play Sound
            AkSoundEngine.PostEvent("Play_ActionCreated", gameObject);
            //Return
            UIManager.singleton_UIManager.menuNavigation = new Vector3(0, 0, 0);
            UIManager.singleton_UIManager.RepositionAlbums();
        }
        else
        {
            Debug.Log("Action Slots Full");
        }
    }
    

    private void HighlightCurrentBar()
    {
        if (GlobalVariables.currentBar == 1)
        {
            actionGrid.transform.GetChild(0).GetComponent<Image>().color=Color.green;
            actionGrid.transform.GetChild(1).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(2).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(3).GetComponent<Image>().color=Color.white;
            
            //Selected action
            selectedAction = 0;
        }
        else if (GlobalVariables.currentBar == 2)
        {
            actionGrid.transform.GetChild(0).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(1).GetComponent<Image>().color=Color.green;
            actionGrid.transform.GetChild(2).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(3).GetComponent<Image>().color=Color.white;
            
            //Selected action
            //Selected action
         
                if (Actions.Count >= 2)
                {
                    if (Actions[1] != null)
                    {
                        selectedAction = 1;
                    }
                    else
                    {
                        selectedAction = 0;
                    }
                }
        }
        else if (GlobalVariables.currentBar == 3)
        {
            actionGrid.transform.GetChild(0).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(1).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(2).GetComponent<Image>().color=Color.green;
            actionGrid.transform.GetChild(3).GetComponent<Image>().color=Color.white;
            
            //Selected action
            if (Actions.Count >= 3)
            {
                if (Actions[2] != null)
                {
                    selectedAction = 2;
                }
                else
                {
                    selectedAction = 0;
                }
            }
        }
        else if (GlobalVariables.currentBar == 4)
        {
            actionGrid.transform.GetChild(0).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(1).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(2).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(3).GetComponent<Image>().color=Color.green;
            
            //Selected action
            if (Actions.Count >= 4)
            {
                if (Actions[3] != null)
                {
                    selectedAction = 3;
                }
                else
                {
                    selectedAction = 0;
                }
            }
        }
        
    }

    public void SubtractActionCost()
    {
        //Subtract the amount of actions points equal to the action on our current bar
        PlayerScript.singleton_Player.SubtractActionPoints(Actions[singleton_ActionSlotManager.selectedAction].actionCost);
    }

    private void CalculateSequenceCost()
    {
        int sum=0;
        foreach (AttackAction action in Actions)
        {
            sum += action.actionCost;
        }

        SequenceCost = sum;
    }

    public void ErasePreviousAction()
    {
        GameObject previousAction = Actions[Actions.Count-1].gameObject; //Find the Game Object of Our Action
        AttackAction Action = Actions[Actions.Count - 1]; //Find the Script of said Action
        SequenceCost -= Action.actionCost; //Subtract its cost from the Sum
        //Remove said action from list and destroy
        Actions.Remove(Action);
        Destroy(previousAction);
    }

    public void WipeActions()
    {
        for (int i = 0; i < Actions.Count; i++)
        {
            GameObject currentAction = Actions[i].gameObject;
            Destroy(currentAction);
        }
        Actions.Clear();
        //Wipe Sequence Cost
        SequenceCost = 0;
    }
    
}
