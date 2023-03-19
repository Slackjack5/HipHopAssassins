using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    
    public static TimeManager singleton_TimeManager;

    public float TimeMax = 1000;

    public float TimeRemaining;
    // Start is called before the first frame update
    void Start()
    {
        if (singleton_TimeManager != null && singleton_TimeManager != this)
        {
            Destroy(this);
        }
        else
        {
            singleton_TimeManager = this;
        }

        TimeRemaining = TimeMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        TimeRemaining -= 1 * Time.deltaTime;
    }
}
