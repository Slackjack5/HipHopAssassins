using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class SelectionAlbumManager : MonoBehaviour
{
    public static SelectionAlbumManager singleton_AlbumManager;
    public AlbumScript ourAttackAlbum;
    //Albums
    public Transform attackAlbumTransform;
    public Transform magicAlbumTransform;
    public Transform itemAlbumTransform;
    public Transform escapeAlbumTransform;


    
    //Parent Slots
    public Transform PrimarySlot;
    public Transform SecondarySlot;
    public Transform ThirdSlot;
    public Transform FourthSlot;
    
    //Locations
    public Transform PrimaryPosition;
    public Transform SecondaryPosition;
    public Transform ThirdPosition;
    public Transform FourthPosition;
    
    //Effects
    public MMF_Player PrimaryAlbum;
    public MMF_Player SecondaryAlbum;
    public MMF_Player ThirdAlbum;
    public MMF_Player FourthAlbum;
    


    // Start is called before the first frame update
    void Start()
    {
        if (singleton_AlbumManager != null && singleton_AlbumManager != this)
        {
            Destroy(this);
        }
        else
        {
            singleton_AlbumManager = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectAttackAction()
    {
        //Play the Selection Feedback effects
        ourAttackAlbum.ActionSelected.PlayFeedbacks();
    }
    
    public void MovePrimaryAlbum(Transform target)
    {
        //Define Variables
        AlbumScript targetScript = target.gameObject.GetComponent<AlbumScript>();
        //Set Targets
        PrimaryAlbum.GetFeedbackOfType<MMF_DestinationTransform>().TargetTransform = target;
        //Change Parent
        target.parent = PrimarySlot;
        //Play the Selection Feedback effects
        PrimaryAlbum.PlayFeedbacks();
        targetScript.ShowDisc.PlayFeedbacks();
        targetScript.DiscSpin.StopFeedbacks();
        targetScript.ResetDisc.PlayFeedbacks();
    }
    
    public void MoveSecondaryAlbum(Transform target)
    {
        //Set Destinations
        AlbumScript targetScript = target.gameObject.GetComponent<AlbumScript>();

        SecondaryAlbum.GetFeedbackOfType<MMF_DestinationTransform>().TargetTransform = target;
        //Change Parent
        target.parent = SecondarySlot;
        //Play the Selection Feedback effects
        SecondaryAlbum.PlayFeedbacks();
        targetScript.HideDisc.PlayFeedbacks();
    }
    public void MoveThirdAlbum(Transform target)
    {
        //Set Destinations
        AlbumScript targetScript = target.gameObject.GetComponent<AlbumScript>();

        ThirdAlbum.GetFeedbackOfType<MMF_DestinationTransform>().TargetTransform = target;
        //Change Parent
        target.parent = ThirdSlot;
        //Play the Selection Feedback effects
        ThirdAlbum.PlayFeedbacks();
        targetScript.HideDisc.PlayFeedbacks();

    }
    public void MoveFourthAlbum(Transform target)
    {
        //Set Destinations
        AlbumScript targetScript = target.gameObject.GetComponent<AlbumScript>();

        FourthAlbum.GetFeedbackOfType<MMF_DestinationTransform>().TargetTransform = target;
        //Change Parent
        target.parent = FourthSlot;
        //Play the Selection Feedback effects
        FourthAlbum.PlayFeedbacks();
        targetScript.HideDisc.PlayFeedbacks();

    }
    
    
}
