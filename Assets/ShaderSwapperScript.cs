using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSwapperScript : MonoBehaviour
{
    public GameObject[] Shaders;
    public int currentShader=0;
    public float waitTime;
    private bool PreparingSwap;
    public Animator ourFade;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentShader >= Shaders.Length)
        {
            currentShader = 0;
        }
        if (PreparingSwap == false)
        {
            StartCoroutine(SwapShaderAnim());
            PreparingSwap = true;
        }
    }

    IEnumerator SwapShaderAnim()
    {
        yield return new WaitForSeconds(waitTime);
        ourFade.SetBool("Fading",true);
        PreparingSwap = false;
    }

    public void Swap()
    {
        for(int i = 0; i < Shaders.Length; i++)
        {
            // Code to be repeated.
            if (i == currentShader)
            {
                Shaders[i].SetActive(true);
            }
            else
            {
                Shaders[i].SetActive(false);
            }
        }
        currentShader++;
    }

    public void FadeComplete()
    {
        ourFade.SetBool("Fading",false);
    }
}
