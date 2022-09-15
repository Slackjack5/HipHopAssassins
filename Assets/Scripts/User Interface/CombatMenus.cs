using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatMenus : UIManager
{
    private UIManager ourUI;
    protected override void Start()
    {
        base.Start();
    }
    
    public override void DeployAttackMenu()
    {
        GenerateLimbs();
        if (menuNavigation.y == 0) //Limb #1
        {
            selectedMonster.GetComponent<MonsterData>().ShowLimb(0);
        }
        else if (menuNavigation.y == 1) //Limb #1
        {
            selectedMonster.GetComponent<MonsterData>().ShowLimb(1);
        }

        if (Input.GetKeyDown("space")) 
        {
            CombatManager.DamageMonsterLimb(selectedMonster.GetComponent<MonsterData>(),(int) menuNavigation.y,20);
            resetBlocks();
            HideAllMenus();
            ChangeState(State.Home);
        }
        
        
    }
    
    private void GenerateLimbs()
    {
        MonsterData selectedData = selectedMonster.GetComponent<MonsterData>();
        int limbCount = selectedData.limbHealth.Length;
        
        for (int i = 0; i < limbCount; i++)
        {
            GameObject tempLimb = limbCanvas.transform.GetChild(i).gameObject;
            tempLimb.SetActive(true);
            tempLimb.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = selectedData.limbName[i];
        }
        
        limbCanvas.SetActive(true);
    }
    
    //Home
    public override void DeployHomeMenu()
    {
        //Current Menu Player is Hovering Over
        if (menuNavigation.x == 0) //Attack
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.SelectMonster);
            }
        }
        else if (menuNavigation.x==1) //Magic
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Magic);
            }
        }
        else if (menuNavigation.x==2) //Items
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Items);
            }
        }
        else if (menuNavigation.x==3) //Flee
        {
            if (Input.GetKeyDown("space")) 
            {
                ChangeState(State.Flee);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuNavigation.x = 0;
            ChangeState(State.Home);
        }

    }


}
