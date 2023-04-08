using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript singleton_Player;
    
    [SerializeField] private int Level;
    [SerializeField] public float Health;
    [SerializeField] public float HealthMax;
    public float actionPoints;
    public float APHit = 2;
    public float APRegen = 1f;
    public float actionPointMax=100;
    public int Strikes;
    public int attackMin;
    public int attackMax;
    public float CriticalStrikeChance;
    public int[] allocatedSpells;
    public int[] allocatedItems;

    private bool lerpingHealth;
    private float timeElapsed = 0;
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

    public void GainActionPoints(float amount)
    {
        actionPoints += amount;
    }

    public void GainApRegen(float amount)
    {
        APRegen = amount;
    }
    
    public void SubtractActionPoints(float amount)
    {
        actionPoints -= amount;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        float timeElapsed = 0;
        UserInterface.singleton_UserInterface.resetLerp();
        //StartCoroutine(LerpHealth(1));
        //if(!lerpingHealth) {StartCoroutine(LerpHealth(1));}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            TakeDamage(20);
        }
        //If we run out of action points, Lock the player out
        if (actionPoints < 0 && CombatManager.singleton_CombatManager.LockedOut==false)
        {
            //CombatManager.singleton_CombatManager.LockOut();
        }
        //Make sure our action points don't go over MAX
        if (actionPoints >= actionPointMax)
        {
            actionPoints = actionPointMax;
        }
    }
    
    

    void FixedUpdate()
    {
        //Generate Action Points Passively
        RegenerateAP();
    }

    private void RegenerateAP()
    {
        GainActionPoints(APRegen);
    }

    private IEnumerator LerpHealth(float duration)
    {
        
        
        lerpingHealth = true;
        
        while (timeElapsed < duration)
        {
            float startingHealth = UserInterface.singleton_UserInterface.HealthWhiteBar.fillAmount;
            float targetHealth = UserInterface.singleton_UserInterface.HealthBar.fillAmount;
            float t = timeElapsed / duration;
            UserInterface.singleton_UserInterface.HealthWhiteBar.fillAmount = Mathf.Lerp(startingHealth,targetHealth, t);
            timeElapsed+=Time.deltaTime;
            yield return null;
        }

        lerpingHealth = false;
        UserInterface.singleton_UserInterface.HealthWhiteBar.fillAmount = UserInterface.singleton_UserInterface.HealthBar.fillAmount;
    }
    
    
}
