using System;
using UnityEngine;
using UnityEngine.UI;

public class BeatEntity : MonoBehaviour
{
  /*
  [HideInInspector] public RectTransform spawnerPos;
  [HideInInspector] public RectTransform centerPos;
  private float normalizedValue;

  [HideInInspector] public float travelTime;
  [HideInInspector] public int indexNumber;

  //Time
  private float currentTime;
  public float startTime;
  public float arrivalTime;
  public float t;
  private bool reachedMiddle;
*/
  private RectTransform rectTransform;

  //private bool attackUsed;
  private RectTransform ring;
  public GameObject LockOutX;
  private Vector3 strartingSize;

  

  private void Start()
  {
    rectTransform = gameObject.GetComponent<RectTransform>();
    ring = transform.GetChild(0).GetComponent<RectTransform>();
    strartingSize = ring.transform.localScale;

  }
/*
  void Update()
  {
    currentTime = AudioEvents.singleton_AudioEvents.masterCurrentPosition;
    t = Mathf.InverseLerp(startTime, arrivalTime, currentTime); //currentTime2/arrivalTime;
  }

  private void FixedUpdate()
  {
    if (t < 1)
    {
      rectTransform.anchoredPosition=Vector3.Lerp(spawnerPos.anchoredPosition,centerPos.anchoredPosition, t);
    }
    else
    {
      Destroy(gameObject);
    }

  }
*/
}
