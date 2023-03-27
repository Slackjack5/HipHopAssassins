﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class BeatEntity : MonoBehaviour
{
  [HideInInspector] public Vector2 spawnerPos;
  [HideInInspector] public Vector2 centerPos;
  private float normalizedValue;
  private RectTransform rectTransform;

  [HideInInspector] public float travelTime;
  [HideInInspector] public int indexNumber;

  private float currentTime;
  private bool reachedMiddle;

  private bool attackUsed;
  private RectTransform ring;
  public GameObject LockOutX;
  private Vector3 strartingSize;

  public bool LockedOut;

  private void Start()
  {
    Debug.Log(("Entity Travel Time" + travelTime));
    rectTransform = gameObject.GetComponent<RectTransform>();
    ring = transform.GetChild(0).GetComponent<RectTransform>();
    strartingSize = ring.transform.localScale;

  }

  private void Update()
  {
  /*
    if (currentTime < travelTime)
    {
      transform.position = reachedMiddle
        ? Vector2.Lerp(centerPos.position, endPos.position, currentTime / travelTime)
        : Vector2.Lerp(spawnerPos.position, centerPos.position, currentTime / travelTime);

      if (!reachedMiddle)
      {
        ring.localScale = Vector3.Lerp(strartingSize, new Vector3(1,1,1), currentTime / travelTime);
      }

      if (indexNumber == MusicManager.singleton_MusicManager.cueIndex-1)
      {
        Debug.Log("Current Index: "+indexNumber + " musicManager index: "+ MusicManager.singleton_MusicManager.cueIndex);
      }
      currentTime += Time.deltaTime;
    }
    else
    {
      if (reachedMiddle)
      {
        // Reached the end of the track, so destroy this object.
        //Destroy(gameObject);
        gameObject.SetActive(false);
      }
      else
      {
        if (!attackUsed) //If we reach the middle, Whiff an Attack
        {
          attackUsed=true;
        }
        reachedMiddle = true;
        currentTime = 0;
      }
    }
  */
  if (currentTime < travelTime)
  {
    normalizedValue=currentTime/travelTime;
            
    rectTransform.anchoredPosition=Vector3.Lerp(spawnerPos,centerPos, normalizedValue);
    currentTime += Time.deltaTime; 
  }
  else
  {
    GridDeployer.singleton_GridDeployer.PulseReticle();
    Destroy(gameObject);
  }

  }

  public void EnableLockout()
  {
    LockedOut = true; //Enable Lockout for BeatEntity
    LockOutX.SetActive(true); //Turn On X
    transform.GetChild(0).gameObject.SetActive(false); //Turn Off Ring
  }
}
