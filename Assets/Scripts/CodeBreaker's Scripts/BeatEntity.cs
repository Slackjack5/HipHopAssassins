using System;
using UnityEngine;

public class BeatEntity : MonoBehaviour
{
  [HideInInspector] public Transform spawnerPos;
  [HideInInspector] public Transform centerPos;
  [HideInInspector] public Transform endPos;
  [HideInInspector] public float travelTime;
  [HideInInspector] public int indexNumber;

  private float currentTime;
  private bool reachedMiddle;

  private bool attackUsed;
  private RectTransform ring;
  private Vector3 strartingSize;

  private void Start()
  {
    Debug.Log(("Entity Travel Time" + travelTime));
    ring = transform.GetChild(0).GetComponent<RectTransform>();
    strartingSize = ring.transform.localScale;
  }

  private void Update()
  {
    if (currentTime < travelTime)
    {
      transform.position = reachedMiddle
        ? Vector2.Lerp(centerPos.position, endPos.position, currentTime / travelTime)
        : Vector2.Lerp(spawnerPos.position, centerPos.position, currentTime / travelTime);

      if (!reachedMiddle)
      {
        ring.localScale = Vector3.Lerp(strartingSize, new Vector3(1,1,1), currentTime / travelTime);
      }

      if (indexNumber == MusicManager.cueIndex-1)
      {
        Debug.Log("Current Index: "+indexNumber + " musicManager index: "+MusicManager.cueIndex);
      }
      currentTime += Time.deltaTime;
    }
    else
    {
      if (reachedMiddle)
      {
        // Reached the end of the track, so destroy this object.
        Destroy(gameObject);
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
  }
}
