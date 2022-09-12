using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbData : MonoBehaviour
{
    private int id;
    private int limbNumber;
    private int limbHealth;
    private string limbName;
    public void SetLimbData(int id,int limbNumber,  int limbHealth, string limbName)
    {
        this.id = id;
        this.limbNumber = limbNumber;
        this.limbHealth = limbHealth;
        this.limbName = limbName;
    }   
}
