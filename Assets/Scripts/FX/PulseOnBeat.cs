using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseOnBeat : MonoBehaviour
{
    public bool Spin;
    public float spinSpeed=1;
    public bool BeatPulse;
    public bool BeatPulseEveryOther;
    private Vector3 StartingSize = new Vector3(1, 1, 1);
    private int pulseNumber;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if (BeatPulse==true)
        {
            AudioEvents.singleton_AudioEvents.OnEveryBeat.AddListener(GrowOnBeat);
        }
        
        if (BeatPulseEveryOther==true)
        {
            if (pulseNumber == 2)
            {
                AudioEvents.singleton_AudioEvents.OnEveryBeat.AddListener(GrowOnBeat);  
            }
            pulseNumber += 1;
        }
    }

    private void FixedUpdate()
    {
        if (transform.localScale.x > StartingSize.x && (BeatPulse || BeatPulseEveryOther))
        {
            gameObject.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
        }

        if (Spin)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.Rotate( new Vector3( 0, 0, spinSpeed ) );
        }
    }

    public void GrowOnBeat()
    {
        gameObject.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        pulseNumber = 0;
    }
}