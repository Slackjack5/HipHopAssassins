using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackAction : MonoBehaviour
{
    public string attackType;
    public GameObject SelectedMonster;
    public int actionCost;
    public bool RiskyAttack;
    public float SelectedLimb;
    public GameObject SelectedAction;

    public Image ourIcon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (RiskyAttack == true)
        {
            ourIcon.color = new Color(255, 120, 120, .5f);
        }
    }
}
