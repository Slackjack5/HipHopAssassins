using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private bool preparingSequence=false;
    private bool sequenceInProgress=false;
    public AK.Wwise.Event OurEvent;

    public int nextBar;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void CommenceAttackEvent()
    {
        Debug.Log("Doing CommenceAttackEvent Function");
        preparingSequence = true;
    }

    public void ResetVariables()
    {
        sequenceInProgress=false;
        preparingSequence = false;
    }

    private void AttackMarker()
    {
        Debug.Log("Attack Marker Found");
    }
    // Update is called once per frame
    void Update()
    {
        if (!sequenceInProgress && preparingSequence)
        {
            if (nextBar == GlobalVariables.currentBar)
            {
                AkSoundEngine.PostEvent("Play_AttackSequences", gameObject);
                OurEvent.Post(gameObject,(uint) (AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition), MusicCallbackFunction);
                sequenceInProgress=true;
                preparingSequence = false;
            }
        }
        if(nextBar == GlobalVariables.currentBar) {nextBar = GlobalVariables.currentBar+1;}
    }
    
    void MusicCallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
  {
    AkMusicSyncCallbackInfo _musicInfo = (AkMusicSyncCallbackInfo) in_info;

    switch (_musicInfo.musicSyncType)
    {
      case AkCallbackType.AK_MusicSyncUserCue:

        CustomCues(_musicInfo.userCueName, _musicInfo);
        break;
    }
  }
    
    public void CustomCues(string cueName, AkMusicSyncCallbackInfo _musicInfo)
    {
        switch (cueName)
        {
            case "Example Case":
                break;
            case "Q":
                Debug.Log("Attack Marker Found");

                break;
      
            default:
                break;
        }
    }


}
