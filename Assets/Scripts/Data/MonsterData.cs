using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterData : MonoBehaviour
{
    private int id;
    private string monsterName;
    private Sprite monsterSprite;
    


    public void SetMonsterData(int id, string monsterName)
    {
        this.id = id;
        this.monsterName = monsterName;
        this.monsterSprite = monsterSprite;
    }

    public int getMonsterId()
    {
        return id;
    }
    
    public String getMonsterName()
    {
        return monsterName;
    }
    
    public Sprite getMonsterSprite()
    {
        return monsterSprite;
    }


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
