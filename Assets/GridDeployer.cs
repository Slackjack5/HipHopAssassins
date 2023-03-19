using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridDeployer : MonoBehaviour
{
    public GameObject startPoint;
    public GameObject endPoint;
    public GameObject gridObject;

    public GameObject Parent;
    // Start is called before the first frame update
    void Start()
    {

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
}
