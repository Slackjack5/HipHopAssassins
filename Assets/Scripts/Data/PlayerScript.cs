using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript singleton_Player;
    
    [SerializeField] private int Level;
    [SerializeField] public int Health;
    public int actionPoints;
    public int actionPointMax=100;
    public int Strikes;
    public int hitsMax;
    public int attackMin;
    public int attackMax;
    public int[] allocatedSpells;
    public int[] allocatedItems;
    
    private void Awake()
    {
        if (singleton_Player != null && singleton_Player != this)
        {
            Destroy(this);
        }
        else
        {
            singleton_Player = this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (actionPoints < 0 && CombatManager.singleton_CombatManager.LockedOut==false)
        {
            CombatManager.singleton_CombatManager.LockOut();
        }
    }
}
