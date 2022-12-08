using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class MonsterData : MonoBehaviour
{
    private int id;
    public int monsterHealth;
    private string monsterName;
    public GameObject monsterGFX;
    public GameObject monsterCanvas;
    public String[] limbName;
    public int[] limbHealth;
    public BeatData[] Resistance = new BeatData[4];
    
    [System.Serializable]
    public class BeatData 
    {
        public string name;//Attack Element Resistance Name
        public bool[] Beat = new bool[4];
    }
    

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

    public void UpdateHealthUI()
    {
        for (int i = 0; i < monsterCanvas.transform.childCount; i++)
        {
            monsterCanvas.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = limbHealth[i].ToString();
        }
    }

    public void ShowLimb(int LimbNumber)
    {
        monsterCanvas.transform.GetChild(LimbNumber).gameObject.SetActive(true);
    }
    public void HideLimbs()
    {
        for (int i = 0; i < monsterCanvas.transform.childCount; i++)
        {
            monsterCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }
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

