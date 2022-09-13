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
        monsterArray = new MonsterData[1];
        BuildMonsterDictionary();
        //BuildLimbDictionary();
    }
    
    

    void BuildMonsterDictionary()
    {
        GameObject temp;
        for (int i = 0; i < monsterArray.Length; i++)
        {
            temp = Instantiate(monsterTemplate);
            //monsterArray[i].gameObject.SetActive(false);
            monsterArray[i] = temp.GetComponent<MonsterData>();
        }

        monsterArray[0].SetMonsterData(0,"Target Dummy",monsterObjectArray[0]);
    }
    
}
