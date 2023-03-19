using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GridDeployer : MonoBehaviour
{
    public GameObject startPoint;
    public GameObject endPoint;
    public GameObject gridObject;

    public GameObject Parent;
    public UnityEvent m_MyEvent = new UnityEvent();
    
    public static GridDeployer singleton_GridDeployer;

    public Animator beatReticleAnim;
    // Start is called before the first frame update
    void Start()
    {
        m_MyEvent.AddListener(PulseReticle);
        
        if (singleton_GridDeployer != null && singleton_GridDeployer != this)
        {
            Destroy(this);
        }
        else
        {
            singleton_GridDeployer = this;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //AudioEvents.singleton_AudioEvents.OnEveryBeat.AddListener(DeployBeatGrid);
    }


    public void DeployBeatGrid()
    {
        GameObject ourGrid = Instantiate(gridObject);
        ourGrid.transform.SetParent(Parent.transform);
        ourGrid.transform.position = startPoint.transform.position;
        ourGrid.GetComponent<Image>().SetNativeSize();
        GridMover ourEntity = ourGrid.GetComponent<GridMover>();
        ourEntity.spawnerPos = startPoint.GetComponent<RectTransform>();
        ourEntity.centerPos = endPoint.GetComponent<RectTransform>();
        ourEntity.travelTime = AudioEvents.secondsPerBar;
    }

    public void PulseReticle()
    {
        beatReticleAnim.Play("BeatReticleAnimation",-1,0f);
        Debug.Log("Circle Smashed");
    }
}
