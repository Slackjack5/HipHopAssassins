using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class MonsterData : MonoBehaviour
{
    private int id;
    private int monsterHealth;
    private string monsterName;
    public GameObject monsterGFX;
    public GameObject monsterCanvas;
    public String[] limbName;
    public int[] limbHealth;
    

    

    public void SetMonsterData(int id, string monsterName, GameObject monster)
    {
        this.id = id;
        this.monsterName = monsterName;
        //Set Name in Inspector
        monster.name = "Monster: " + monsterName;
        //Set Limb Names and Helath
        for (int i = 0; i < limbName.Length; i++)
        {
            setLimbHealth(monsterCanvas.transform.GetChild(i).gameObject,limbHealth[i]);
        }
    }
    

    public int getMonsterId()
    {
        return id;
    }
    
    public String getMonsterName()
    {
        return monsterName;
    }
    
    public GameObject getmonsterGFX()
    {
        return monsterGFX;
    }

    public void setLimbHealth(GameObject limb, int health)
    {
        limb.GetComponent<TextMeshProUGUI>().text = health.ToString();
    }


    
    // Start is called before the first frame update
    void Start()
    {
        //Parent GFX to Game Object
        gameObject.transform.SetParent(gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
