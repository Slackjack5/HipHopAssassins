using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Item
{
    // Start is called before the first frame update
    protected override void Start()
    {
        onUse();
    }

    protected override void onUse()
    {
        Debug.Log("Using Item: " + itemName);
        CombatManager.HealPlayer(onUseValue);
        Destroy(gameObject);
    }

    // Update is called once per frame
    protected override void Update()
    {
  
    }
}
