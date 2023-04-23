using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridMover : MonoBehaviour
{
    public RectTransform spawnerPos;
    public RectTransform centerPos;
    public float arrivalTime;
    private float t;

    public float currentTime;
    public float startTime;
    private float normalizedValue;
    private bool reachedMiddle;
    private RectTransform rectTransform;
    public bool inactive;
    public float barPosition;
    
    //Beat Entity
    private bool beatEntity;

    //Lock-Out
    public bool LockedOut;
    
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        currentTime = AudioEvents.singleton_AudioEvents.masterCurrentPosition;
        t = Mathf.InverseLerp(startTime, arrivalTime, currentTime); //currentTime2/arrivalTime;
        
        float fCurrentPosition = AudioEvents.singleton_AudioEvents.masterCurrentPosition;
        barPosition = fCurrentPosition / (MusicManager.singleton_MusicManager.sequenceSecondsPerBeat*4);
        
        if (t < 1)
        {
            rectTransform.anchoredPosition=Vector3.Lerp(spawnerPos.anchoredPosition,centerPos.anchoredPosition, t);
        }
        else
        {
            if(!inactive){GridDeployer.singleton_GridDeployer.PulseReticleWhite();}
            Destroy(gameObject);
        }

    }
    
    public void EnableLockout()
    {
        LockedOut = true; //Enable Lockout for BeatEntity
        //LockOutX.SetActive(true); //Turn On X
        transform.GetChild(0).gameObject.SetActive(false); //Turn Off Ring
    }
}
