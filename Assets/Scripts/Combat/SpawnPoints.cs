using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    private GameObject spawnPointSolo;
    private GameObject spawnPointDuo;
    private GameObject spawnPointTrio;
    public GameObject[] EnemyHolder;
    private int soloTaken;
    private int[] duoTaken;
    private int[] trioTaken;
    // Start is called before the first frame update
    void Start()
    {
        spawnPointSolo = gameObject.transform.GetChild(0).gameObject;
        spawnPointDuo = gameObject.transform.GetChild(1).gameObject;
        spawnPointTrio = gameObject.transform.GetChild(2).gameObject;
        EnemyHolder = new GameObject[3];
    }
    

    public Transform getSpawnPoint(int Group, int Position)
    {
        if (Group == 1)
        {
            return spawnPointSolo.transform.GetChild(Position).transform;
        }
        else if (Group == 2)
        {
            return spawnPointDuo.transform.GetChild(Position).transform;
        }
        else if (Group == 3)
        {
            return spawnPointTrio.transform.GetChild(Position).transform;
        }
        return null;
    }

    public void AssignHolder(GameObject enemy)
    {
        for (int i = 0; i < EnemyHolder.Length; i++)
        {
            if (EnemyHolder[i] == null)
            {
                EnemyHolder[i] = enemy;
                break;
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (CombatManager.enemyCount == 1)
        {
            EnemyHolder[0].transform.position = getSpawnPoint(1,0).transform.position;
        }
        else if (CombatManager.enemyCount == 2)
        {
            EnemyHolder[0].transform.position = getSpawnPoint(2,0).transform.position;
            EnemyHolder[1].transform.position = getSpawnPoint(2,1).transform.position;
        }
        else if (CombatManager.enemyCount == 3)
        {
            EnemyHolder[0].transform.position = getSpawnPoint(3,0).transform.position;
            EnemyHolder[1].transform.position = getSpawnPoint(3,1).transform.position;
            EnemyHolder[2].transform.position = getSpawnPoint(3,2).transform.position;
        }
    }
}
