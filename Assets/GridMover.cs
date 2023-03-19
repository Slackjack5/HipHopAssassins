using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridMover : MonoBehaviour
{
    [HideInInspector] public RectTransform spawnerPos;
    [HideInInspector] public RectTransform centerPos;
    public float travelTime;
    private float normalizedValue;
    private bool reachedMiddle;
    public float currentTime;
    private RectTransform rectTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

        if (currentTime < travelTime)
        {
            normalizedValue=currentTime/travelTime;
            
            rectTransform.anchoredPosition=Vector3.Lerp(spawnerPos.anchoredPosition,centerPos.anchoredPosition, normalizedValue);
            currentTime += Time.deltaTime; 
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
