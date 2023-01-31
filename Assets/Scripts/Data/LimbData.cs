using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbData : MonoBehaviour
{
    private int id;
    private int limbNumber;
    private int limbHealth;
    private string limbName;
    private LimbData[] limbArray;
    public int limbAmount=1;

    private void Start()
    {
        limbArray = new LimbData[limbAmount];
    }

    public void SetLimbData(int id,int limbNumber,  int limbHealth, string limbName)
    {
        this.id = id;
        this.limbNumber = limbNumber;
        this.limbHealth = limbHealth;
        this.limbName = limbName;
    }

}
