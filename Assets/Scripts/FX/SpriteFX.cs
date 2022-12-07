using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpriteFX : MonoBehaviour
{
    public bool RandomRotation;
    private void Start()
    {
        int Rand = Random.Range(0, 360);
        if (RandomRotation) {transform.rotation = Quaternion.Euler(0, 0, Rand); }
    }

    public void DestroyFX()
    {
        Destroy(gameObject);
    }
}
