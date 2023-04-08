using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridMover : MonoBehaviour
{
    [HideInInspector] public RectTransform spawnerPos;
    [HideInInspector] public RectTransform centerPos;
    public float arrivalTime;
    public float t;

    public float travelTime;
    private float normalizedValue;
    private bool reachedMiddle;
    public float currentTime;
    private RectTransform rectTransform;
    public bool inactive;
    public float barPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        t = AudioEvents.singleton_AudioEvents.masterCurrentPosition/arrivalTime;
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

    private void FixedUpdate()
    {
        float fCurrentPosition = AudioEvents.singleton_AudioEvents.masterCurrentPosition;
        barPosition = fCurrentPosition / (MusicManager.singleton_MusicManager.sequenceSecondsPerBeat*4);
    }
}
