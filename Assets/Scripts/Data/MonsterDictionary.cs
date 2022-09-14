using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDictionary : MonoBehaviour
{
    public GameObject monsterTemplate;
    private MonsterData[] monsterArray;
    String[] tempLimbNames;
    [SerializeField] private GameObject[] monsterObjectArray;

    private void Start()
    {
        //Amount of Monsters in the Game
        monsterArray = new MonsterData[1];
        BuildMonsterDictionary();
    }
    
    

    void BuildMonsterDictionary()
    {
        GameObject temp;
        for (int i = 0; i < monsterArray.Length; i++)
        {
            temp = Instantiate(monsterTemplate);
            temp.transform.SetParent(gameObject.transform);
            monsterArray[i] = temp.GetComponent<MonsterData>();
        }
        //Training Dummy Enemy
        monsterArray[0].SetMonsterData(0,"Target Dummy",monsterObjectArray[0]);
        monsterArray[0].gameObject.SetActive(false);
    }
    
}
