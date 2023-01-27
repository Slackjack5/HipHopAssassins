using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSlotManager : MonoBehaviour
{
    public static ActionSlotManager singleton_ActionSlotManager;
    public List<AttackAction> Actions = new List<AttackAction>();

    public GameObject AttackGameObject;

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
            Debug.Log("Spawning Attack Action");
            nextSlot = Actions.Count;
            GameObject ourNewAction = Instantiate(AttackGameObject); //Spawn Object
            ourNewAction.transform.SetParent(actionGrid.transform.GetChild(nextSlot)); //Set Parent
            ourNewAction.GetComponent<Image>().rectTransform.localPosition = new Vector3(0, 0, 0); //Set Position
            AttackAction ourAttackAction = ourNewAction.GetComponent<AttackAction>();
            ourAttackAction.SelectedMonster = SelectedMonster;
            ourAttackAction.SelectedLimb = SelectedLimb;
            Actions.Add(ourAttackAction);
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
        }
        else if (GlobalVariables.currentBar == 2)
        {
            actionGrid.transform.GetChild(0).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(1).GetComponent<Image>().color=Color.green;
            actionGrid.transform.GetChild(2).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(3).GetComponent<Image>().color=Color.white;
        }
        else if (GlobalVariables.currentBar == 3)
        {
            actionGrid.transform.GetChild(0).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(1).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(2).GetComponent<Image>().color=Color.green;
            actionGrid.transform.GetChild(3).GetComponent<Image>().color=Color.white;
        }
        else if (GlobalVariables.currentBar == 4)
        {
            actionGrid.transform.GetChild(0).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(1).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(2).GetComponent<Image>().color=Color.white;
            actionGrid.transform.GetChild(3).GetComponent<Image>().color=Color.green;
        }
        
    }

    public void WipeActions()
    {
        for (int i = 0; i < Actions.Count; i++)
        {
            GameObject currentAction = Actions[i].gameObject;
            Destroy(currentAction);
        }
        Actions.Clear();
    }
}
