using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;


public class GridSpawner : MonoBehaviour
{
  [HideInInspector] public Transform spawnerPos;
  [HideInInspector] public Transform centerPos;
  [HideInInspector] public Transform endPos;
  public float travelTime;
  private bool reachedMiddle;
  public float currentTime;
  public Image sr;

  
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
        ? Vector2.Lerp(spawnerPos.position, centerPos.position, currentTime / travelTime)
        : Vector2.Lerp(spawnerPos.position, centerPos.position, currentTime / travelTime);
      
      float newAlpha = Mathf.Lerp(spawnerPos.position.x, centerPos.position.x, currentTime / travelTime);
      sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha*255);
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
        Destroy(gameObject);
        currentTime = 0;
      }
    }
  }
}
