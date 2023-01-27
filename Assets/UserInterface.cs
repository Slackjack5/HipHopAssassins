using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public static UserInterface singleton_UserInterface;
    public GameObject homeCanvas;
    public GameObject attackCanvas;
    public GameObject magicCanvas;
    public GameObject itemCanvas;
    public GameObject confirmationCanvas;
    public GameObject Button;

    private void Awake()
    {
        if (singleton_UserInterface != null && singleton_UserInterface != this)
        {
            Destroy(this);
        }
        else
        {
            singleton_UserInterface = this;
        }
    }
    //Home Menu Actions
    public void enableHome()
    {
        homeCanvas.SetActive(true);
    }
    
    public void disableHome()
    {
        homeCanvas.SetActive(false);
    }
    //Attack Menu Actions
    public void enableAttackMenu()
    {
        attackCanvas.SetActive(true);
    }
    
    public void disableAttackMenu()
    {
        attackCanvas.SetActive(false);
    }
    //Magic Menu Actions
    public void enableMagicMenu()
    {
        magicCanvas.SetActive(true);
    }
    
    public void disableMagicMenu()
    {
        magicCanvas.SetActive(false);
    }
    
    //Item Menu Actions
    public void enableItemMenu()
    {
        itemCanvas.SetActive(true);
    }
    
    public void disableItemMenu()
    {
        itemCanvas.SetActive(false);
    }
    
    //Flee Menu Actions
    public void enableConfirmationmMenu()
    {
        confirmationCanvas.SetActive(true);
    }
    
    public void disableConfirmationmMenu()
    {
        confirmationCanvas.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
