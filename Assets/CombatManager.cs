using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static int enemyCount;

    private GameObject encounterEnemies;
    private GameObject enemyDictionary;
    private bool enemiesSpawned;
    // Start is called before the first frame update
    void Start()
    {
        encounterEnemies = gameObject.transform.GetChild(0).gameObject;
        enemyDictionary = gameObject.transform.GetChild(1).gameObject;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemiesSpawned) { SpawnEnemies(0);} //Spawn our enemies
        enemyCount = encounterEnemies.transform.childCount;
    }

    public void SpawnEnemies(int id)
    {
        GameObject temp = Instantiate(enemyDictionary.transform.GetChild(id).gameObject);
        temp.transform.parent = encounterEnemies.transform;
        temp.SetActive(true);
        enemiesSpawned = true; 
    }
}
