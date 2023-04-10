using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public static UserInterface singleton_UserInterface;
    public GameObject homeCanvas;
    public GameObject BodyPartText;
    public GameObject magicCanvas;
    public GameObject itemCanvas;
    public GameObject confirmationCanvas;
    public GameObject Button;
    public GameObject ActionPointCanvas;
    public GameObject SequenceCostCanvas;
    public GameObject StrikeOutCanvas;
    public GameObject BeatMapCanvas;
    public GameObject BeatCanvas;
    public Image EnergyBar;
    public Image TimerBar;
    public Image HealthBar;
    public Image HealthWhiteBar;

    private float lerpTimer;
    

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
        BodyPartText.SetActive(true);
    }
    
    public void disableAttackMenu()
    {
        BodyPartText.SetActive(false);
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

    public void enableBeatMap()
    {
        BeatMapCanvas.SetActive(true);
    }
    
    public void disableBeatMap()
    {
        BeatMapCanvas.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        
        //Update our amount of Action Points
        GameObject actionpointText = ActionPointCanvas.transform.GetChild(0).GameObject();
        TextMeshProUGUI textmeshPro = actionpointText.GetComponent<TextMeshProUGUI>();
        textmeshPro.SetText("Action Points: "+PlayerScript.singleton_Player.actionPoints+" / " + PlayerScript.singleton_Player.actionPointMax);
        //Update our amount of Sequence Cost
        GameObject SequenceCostText = SequenceCostCanvas.transform.GetChild(0).GameObject();
        TextMeshProUGUI textmeshPro2 = SequenceCostText.GetComponent<TextMeshProUGUI>();
        textmeshPro2.SetText("Sequence Cost: "+ActionSlotManager.singleton_ActionSlotManager.SequenceCost+" / " + PlayerScript.singleton_Player.actionPointMax);
        //Update our amount of Strikes
        GameObject StrikesText = StrikeOutCanvas.transform.GetChild(0).GameObject();
        TextMeshProUGUI textmeshPro3 = StrikesText.GetComponent<TextMeshProUGUI>();
        textmeshPro3.SetText("Strikes: "+PlayerScript.singleton_Player.Strikes+" / " + "3");
        //textObject.transform.GetComponent<TextMeshPro>().text = "Hello It Works";
        
        //Update  Energy
        EnergyBar.fillAmount = PlayerScript.singleton_Player.actionPoints / PlayerScript.singleton_Player.actionPointMax;
        
        //Update Timer
        TimerBar.fillAmount = TimeManager.singleton_TimeManager.TimeRemaining / TimeManager.singleton_TimeManager.TimeMax;
        
        //Update Player Health
        UpdateHealthUI();
        
    }

    public void UpdateHealthUI()
    {
        float fillF = HealthBar.fillAmount;
        float fillB = HealthWhiteBar.fillAmount;
        float hFraction = PlayerScript.singleton_Player.Health / PlayerScript.singleton_Player.HealthMax;
        //Lerp Color
        HealthBar.color = Color.Lerp(Color.red,Color.white, hFraction);
        if (fillB > hFraction)
        {
            HealthBar.fillAmount = PlayerScript.singleton_Player.Health / PlayerScript.singleton_Player.HealthMax;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / 15f;
            HealthWhiteBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        else
        {
            lerpTimer = 0;
        }
    }

    public void resetLerp()
    {
        lerpTimer = 0;
    }
}
