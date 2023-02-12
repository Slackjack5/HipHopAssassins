using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
  [HideInInspector] public Transform spawnerPos;
  [HideInInspector] public Transform centerPos;
  [HideInInspector] public Transform endPos;
  public float travelTime;
  private bool reachedMiddle;
  public float currentTime;
  
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
private void Update()
  {

    if (currentTime < travelTime)
    {
      transform.position = reachedMiddle
        ? Vector2.Lerp(centerPos.position, endPos.position, currentTime / travelTime)
        : Vector2.Lerp(spawnerPos.position, centerPos.position, currentTime / travelTime);
      
      currentTime += Time.deltaTime;
    }
    else
    {
      if (reachedMiddle)
      {
        // Reached the end of the track, so destroy this object.
        Destroy(gameObject);
        //gameObject.SetActive(false);
      }
      else
      {
        reachedMiddle = true;
        currentTime = 0;
      }
    }
  }
}
