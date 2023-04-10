using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionAlbumManager : MonoBehaviour
{
    public static SelectionAlbumManager singleton_AlbumManager;
    public AttackAlbumScript ourAttackAlbum;

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
    
    public void PrimaryAlbumAttack()
    {
        //Play the Selection Feedback effects
        //ourAttackAlbum.ResetDisc.PlayFeedbacks();
        ourAttackAlbum.ShowAlbum.PlayFeedbacks();
        ourAttackAlbum.PrimaryAlbum.PlayFeedbacks();
        ourAttackAlbum.DiscSpin.StopFeedbacks();
    }
}
