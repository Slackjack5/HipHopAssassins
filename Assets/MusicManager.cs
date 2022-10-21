using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public bool preparingSequence=false;
    private bool sequenceInProgress=false;
    public AK.Wwise.Event OurEvent;
    public bool attackInProgress;
    public int nextBar;
    //Sequence Data
    public float sequenceSecondsPerBar;
    public float sequenceSecondsPerBeat;
    private float sequenceDuration;
    public List<float> CueTimes = new List<float>();
    private int cueIndex;
    
    //Segment
    private AkSegmentInfo currentSegment;
    public float playheadPosition;
    //id of the wwise event - using this to get the playback position
    public uint playingID;
    
    //Notes
    public float TravelTime;
    public GameObject beatCircle;
    public GameObject RhythmUI;
    
    // Start is called before the first frame update
    void Start()
    {
        currentSegment = new AkSegmentInfo();
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
        attackInProgress = false;
        cueIndex = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        //Update our Playhead
        if (playingID != null)
        {
            AkSoundEngine.GetPlayingSegmentInfo(playingID, currentSegment);
            playheadPosition = (currentSegment.iCurrentPosition);
        }
        if (!sequenceInProgress && preparingSequence)
        {
            if (nextBar == GlobalVariables.currentBar)
            {
                AkSoundEngine.PostEvent("Play_AttackSequences", gameObject);
                playingID = OurEvent.Post(gameObject,(uint) (AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition), MusicCallbackFunction);

                sequenceInProgress=true;
                preparingSequence = false;
                attackInProgress = true; //Let everyone know we are commencing our attack
            }
        }

        if(nextBar == GlobalVariables.currentBar) {nextBar = GlobalVariables.currentBar+1;}
    }

    private void FixedUpdate()
    {
                

    }


    void MusicCallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
  {
    AkMusicSyncCallbackInfo _musicInfo = (AkMusicSyncCallbackInfo) in_info;
    switch (_musicInfo.musicSyncType)
    {
        case AkCallbackType.AK_MusicSyncEntry:
            sequenceSecondsPerBar = _musicInfo.segmentInfo_fBarDuration;
            sequenceSecondsPerBeat = _musicInfo.segmentInfo_fBeatDuration;
            TravelTime = sequenceSecondsPerBeat;
            break;
        case AkCallbackType.AK_MusicSyncUserCue:
            Debug.Log(sequenceSecondsPerBar);
        CustomCues(_musicInfo.userCueName, _musicInfo);
        break;
        case AkCallbackType.AK_MusicSyncExit:
            ResetVariables();
            CombatManager.SkipPlayerTurn();
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
                Debug.Log(playheadPosition); 
                CueTimes.Add(playheadPosition);
                NoteSpawner();
                cueIndex++;
                break;
      
            default:
                break;
        }
    }

    public void NoteSpawner()
    {
        GameObject ourCircle = Instantiate(beatCircle);
        BeatEntity ourEntity = ourCircle.GetComponent<BeatEntity>();
        ourCircle.transform.SetParent(RhythmUI.transform);
        ourEntity.spawnerPos = RhythmUI.transform.GetChild(0).GetComponent<RectTransform>().transform;
        ourEntity.centerPos = RhythmUI.transform.GetChild(1).GetComponent<RectTransform>().transform;
        ourEntity.endPos = RhythmUI.transform.GetChild(2).GetComponent<RectTransform>().transform;
        ourEntity.travelTime = /*((sequenceSecondsPerBar/2)+CueTimes[cueIndex])-*/(sequenceSecondsPerBar)-TravelTime;
    }


}
