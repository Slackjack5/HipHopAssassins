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
    private bool playerAttackStarted = false;
    //Sequence Data
    public float sequenceSecondsPerBar;
    public float sequenceSecondsPerBeat;
    private float sequenceDuration;
    public List<float> CueTimes = new List<float>();
    public static int cueIndex;
    public int hitIndex;
    
    //Data
    [SerializeField] [Range(0.00f, 1.2f)] private float leniency = 0.07f;
    private float lateBound; // The latest, in seconds, that the player can hit the note before it is considered a miss
    //Segment
    private AkSegmentInfo currentSegment;
    public float playheadPosition;
    //id of the wwise event - using this to get the playback position
    public uint playingID;
    
    //Notes
    public float TravelTime;
    public GameObject beatCircle;
    public GameObject RhythmUI;
    
    //Managers
    private CombatManager ourCombatManager;
    
    // Start is called before the first frame update
    void Start()
    {
        currentSegment = new AkSegmentInfo();
        ourCombatManager = GameObject.Find("CombatManager").GetComponent<CombatManager>();
        //Set Hit Leniency
        lateBound = leniency * 2 / 3;
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
        playingID = 0;
        //Wipe List
        CueTimes.Clear();
    }
    
    // Update is called once per frame
    void Update()
    {
        //Update our Playhead
        if (playingID != 0)
        {
            AkSoundEngine.GetPlayingSegmentInfo(playingID, currentSegment);
            playheadPosition = (currentSegment.iCurrentPosition) / 1000f;
            
            /*//Check for Player Input
            if (cueIndex < CueTimes.Count && playerAttackStarted)
            {
                Debug.Log("Percentage to Reaching End: " + playheadPosition/(CueTimes[cueIndex]+(sequenceSecondsPerBar)) * 100);
            }
            */
            
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

        if (cueIndex < CueTimes.Count)
        {
            Debug.Log(("Looking For:")+ (CueTimes[hitIndex]+(sequenceSecondsPerBar))+"PlayHead Position: "+playheadPosition);
        }
        
        if (hitIndex < CueTimes.Count && playheadPosition >= (CueTimes[hitIndex]+sequenceSecondsPerBar)) //If we go over our next Cue Time
        {
            CombatManager.playerMeleeAttack(0);
            hitIndex++;
            Debug.Log("Hitting Miss");
        }
        
        if (cueIndex < CueTimes.Count && playheadPosition >= CueTimes[cueIndex]+(sequenceSecondsPerBar-TravelTime)) //Spawn Beat Circle , 1 Beat ahead of time
        {
            NoteSpawner();
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

    public void PlayerSequenceStart()
    {
        CombatManager.hitsRemaining = CueTimes.Count;
        playerAttackStarted=true;
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
                break;
            case "Start":
                Debug.Log("Player Start!");
                PlayerSequenceStart();
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
        ourEntity.indexNumber = cueIndex;
        ourEntity.spawnerPos = RhythmUI.transform.GetChild(0).GetComponent<RectTransform>().transform;
        ourEntity.centerPos = RhythmUI.transform.GetChild(1).GetComponent<RectTransform>().transform;
        ourEntity.endPos = RhythmUI.transform.GetChild(2).GetComponent<RectTransform>().transform;
        ourEntity.travelTime = TravelTime;
        Debug.Log("Spawning Index:"+cueIndex);
        cueIndex++;
    }


}
