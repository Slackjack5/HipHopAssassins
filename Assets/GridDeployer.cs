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
    public GameObject beatEntity;
    public GameObject halfBeatGridObject;


    public GameObject Parent;
    public UnityEvent m_MyEvent = new UnityEvent();
    
    public static GridDeployer singleton_GridDeployer;

    public Animator beatReticleAnim;

    public Image ReticleColor;
    //Beat Images
    public Sprite GridSprites;
    // Start is called before the first frame update
    void Start()
    {
        m_MyEvent.AddListener(PulseReticleWhite);
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
        GatherBeatData(ourGrid.GetComponent<GridMover>());

    }

    public void DeployBeatEntity()
    {
        GameObject ourGrid = Instantiate(beatEntity);
        ourGrid.transform.SetParent(Parent.transform);
        //Parent.transform.GetChild(Parent.transform.childCount-2).GetComponent<GridMover>().inactive=true;//Delete the child in front of the Beat
        ourGrid.transform.position = startPoint.transform.position;
        GatherBeatData(ourGrid.GetComponent<GridMover>());

    }
    
    public void DeployHalfBeatGrid()
    {
        GameObject ourGrid = Instantiate(halfBeatGridObject);
        ourGrid.transform.SetParent(Parent.transform);
        ourGrid.transform.position = startPoint.transform.position;
        GatherBeatData(ourGrid.GetComponent<GridMover>());
    }

    public void GatherBeatData(GridMover ourEntity)
    {
        ourEntity.spawnerPos = startPoint.GetComponent<RectTransform>();
        ourEntity.centerPos = endPoint.GetComponent<RectTransform>();
        ourEntity.arrivalTime = (AudioEvents.singleton_AudioEvents.barAheadTime);
        ourEntity.startTime = AudioEvents.singleton_AudioEvents.masterCurrentPosition;
    }

    public void PulseReticleWhite()
    {
        beatReticleAnim.Play("BeatReticleAnimation",-1,0f);
        ReticleColor.color = Color.white;
        Debug.Log("Circle Smashed");
    }
    public void PulseReticleRed()
    {
        beatReticleAnim.Play("BeatReticleAnimation",-1,0f);
        ReticleColor.color = Color.red;
        Debug.Log("Circle Smashed");
    }
    public void PulseReticleGreen()
    {
        beatReticleAnim.Play("BeatReticleAnimation",-1,0f);
        ReticleColor.color = Color.green;

        Debug.Log("Circle Smashed");
    }
    public void PulseReticleYellow()
    {
        beatReticleAnim.Play("BeatReticleAnimation",-1,0f);
        ReticleColor.color = Color.yellow;
        Debug.Log("Circle Smashed");
    }
    public void PulseReticleOrange()
    {
        beatReticleAnim.Play("BeatReticleAnimation",-1,0f);
        ReticleColor.color = Color.grey;
        Debug.Log("Circle Smashed");
    }
}
