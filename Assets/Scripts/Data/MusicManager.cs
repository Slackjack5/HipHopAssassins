using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager singleton_MusicManager;
    public bool preparingSequence=false;
    private bool sequenceInProgress=false;
    public AK.Wwise.Event OurEvent;
    
    public AK.Wwise.Event AttackSequence1;
    public AK.Wwise.Event AttackSequence2;
    public AK.Wwise.Event AttackSequence3;
    public AK.Wwise.Event AttackSequence4;
    
    public AK.Wwise.Event OurTrack;
    public bool attackInProgress;
    public int nextBar;
    //Sequence Data
    public float sequenceSecondsPerBar;
    public float sequenceSecondsPerBeat;
    private float sequenceDuration;
    public List<float> CueTimes = new List<float>();
    public List<String> AttackType = new List<String>();
    public List<GameObject> CueObjects = new List<GameObject>();
    public int cueIndex;
    public int hitIndex;
    
    //Data
    [SerializeField] [Range(0.00f, 1.2f)] private float leniency = 0.07f;
    private float lateBound; // The latest, in seconds, that the player can hit the note before it is considered a miss
    //Segment
    private AkSegmentInfo currentSegment;
    public float playheadPosition;
    //id of the wwise event - using this to get the playback position
    public uint playingIDGlobal;
    public uint playingID;
    public uint playingID2;
    public uint playingID3;
    public uint playingID4;
    public int Lane = 1;
    
    //Notes
    public float TravelTime;
    public GameObject beatCircle;
    public GameObject RhythmUI;
    
    //Managers
    private CombatManager ourCombatManager;
    
    //Public
    public int AttackCounter;
    private ActionSlotManager ourActionSlotManager;
    public GameObject musichandler;
    
    private void Awake()
    {
        if (singleton_MusicManager != null && singleton_MusicManager != this)
        {
            Destroy(this);
        }
        else
        {
            singleton_MusicManager = this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.SetSwitch("SequenceNumber", "Sequence1", gameObject);
        currentSegment = new AkSegmentInfo();
        ourCombatManager = GameObject.Find("CombatManager").GetComponent<CombatManager>();
        //Set Hit Leniency
        lateBound = leniency * 2 / 3;
        Lane = 1;
        ourActionSlotManager = GameObject.Find("ActionSlotManager").GetComponent<ActionSlotManager>();
    }
    
    
    public void CommenceAttackEvent()
    {
        Debug.Log("Doing CommenceAttackEvent Function");
        preparingSequence = true;
        //Update Attack Counter
        AttackCounter = ourActionSlotManager.Actions.Count-1;
    }
    
    public void EnableQTELockout()
    {
        foreach (GameObject QTE in CueObjects)
        {
            QTE.GetComponent<BeatEntity>().EnableLockout();
        }
        
        //Turn Off Attack Sequence
        AkSoundEngine.PostEvent("Play_AttackLockout", gameObject);
    }
    public void ResetVariables()
    {
        AkSoundEngine.PostEvent("Stop_AttackSequences", gameObject);
        AkSoundEngine.PostEvent("Play_CancleAttackLockout", gameObject);
        CombatManager.singleton_CombatManager.LockedOut = false;
        sequenceInProgress=false;
        preparingSequence = false;
        attackInProgress = false;
        cueIndex = 0;
        playingID = 0;
        hitIndex = 0;
        //Destroy All Queue Circles
        foreach (GameObject CueObject in CueObjects)
        {
            Destroy(CueObject);
        }
        //Wipe List
        CueTimes.Clear();
        AttackType.Clear();
        CueObjects.Clear();
    }
    
    
    // Update is called once per frame
    void Update()
    {
        //Update our Playhead
        if (playingID != 0)
        {
            AkSoundEngine.GetPlayingSegmentInfo(playingIDGlobal, currentSegment);
            playheadPosition = (currentSegment.iCurrentPosition) / 1000f;
        }
        if (!sequenceInProgress && preparingSequence)
        {
            if (GlobalVariables.currentBeat == 1 && GlobalVariables.currentBar%4 == 0) //Have the Sequence start on the Final Bar
            {
                Debug.Log("Starting NEW Attack Sequence");
                //Debug.Break();
                AkSoundEngine.PostEvent("Play_AttackTrack172BPM", gameObject);
                playingIDGlobal = OurTrack.Post(gameObject,(uint) (AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition), MusicCallbackFunction);

                sequenceInProgress=true;
                preparingSequence = false;
                attackInProgress = true; //Let everyone know we are commencing our attack
            }
        }
        if (cueIndex < CueTimes.Count && playheadPosition >= CueTimes[cueIndex]+((sequenceSecondsPerBeat*1)-TravelTime)) //Spawn Beat Circle , 4 Beats ahead of time
        {
            NoteSpawner();
        }
        
        if (hitIndex < CueTimes.Count && !CombatManager.singleton_CombatManager.LockedOut) // Check if Player Hit In Time
        {
            CheckHit(playheadPosition);
        }
        
        
        if(nextBar == GlobalVariables.currentBar) {nextBar = GlobalVariables.currentBar+1;}
    }

    private void FixedUpdate()
    {
        

    }

    private void CheckHit(float currentSegmentPosition)
    {
        float error = (CueTimes[hitIndex]+sequenceSecondsPerBar) - currentSegmentPosition;
        

        if (error >= -lateBound && error <= leniency)
        {
            Debug.Log("Awaiting Player Input");
            if (error <= leniency / 3) //PerfectHit
                {
                    if (hitIndex < CueObjects.Count){CueObjects[hitIndex].GetComponent<Image>().color = new Color32(0,255,0,255);}
                    if (Input.GetKeyDown("space"))
                    {
                        Debug.Log("Perfect Hit");
                        if (AttackType[hitIndex] == "PS1")
                        {
                            AkSoundEngine.PostEvent("Play_PS1", gameObject);
                            CombatManager.playerMeleeAttack(1,1);
                            PlayerScript.singleton_Player.GainActionPoints(PlayerScript.singleton_Player.APHit*3);
                        }
                        else if (AttackType[hitIndex] == "PS2")
                        {
                            AkSoundEngine.PostEvent("Play_PS1", gameObject);
                            CombatManager.playerMeleeAttack(1,5);
                            PlayerScript.singleton_Player.GainActionPoints(PlayerScript.singleton_Player.APHit*3);
                        }
                        else if (AttackType[hitIndex] == "PS3")
                        {
                            AkSoundEngine.PostEvent("Play_PS2", gameObject);
                            CombatManager.playerMeleeAttack(2,5);
                            PlayerScript.singleton_Player.GainActionPoints(PlayerScript.singleton_Player.APHit*3);
                        }
                        NextInLine();
                        //hitIndex++;
                    }

                }
                else if (error <= (leniency / 3) * 2) //Great Hit
                {
                    if (hitIndex < CueObjects.Count){CueObjects[hitIndex].GetComponent<Image>().color = new Color32(255,255,0,255);}

                    if (Input.GetKeyDown("space"))
                    {
                        if (AttackType[hitIndex] == "PS1")
                        {
                            AkSoundEngine.PostEvent("Play_PS1", gameObject);
                            CombatManager.playerMeleeAttack(1,1);
                            PlayerScript.singleton_Player.GainActionPoints(PlayerScript.singleton_Player.APHit*2);
                        }
                        else if (AttackType[hitIndex] == "PS2")
                        {
                            AkSoundEngine.PostEvent("Play_PS1", gameObject);
                            CombatManager.playerMeleeAttack(1,2);
                            PlayerScript.singleton_Player.GainActionPoints(PlayerScript.singleton_Player.APHit*2);
                        }
                        else if (AttackType[hitIndex] == "PS3")
                        {
                            AkSoundEngine.PostEvent("Play_PS2", gameObject);
                            CombatManager.playerMeleeAttack(2,2);
                            PlayerScript.singleton_Player.GainActionPoints(PlayerScript.singleton_Player.APHit*3);
                        }
                        Debug.Log("Great Hit");
                        
                        NextInLine();
                        //hitIndex++;
                    }
                }
            else
            {
                if (hitIndex < CueObjects.Count) { CueObjects[hitIndex].GetComponent<Image>().color = new Color32(255, 100, 0, 255); }
                if (Input.GetKeyDown("space"))
                {
                    if (AttackType[hitIndex] == "PS1")
                    {
                        AkSoundEngine.PostEvent("Play_PS1", gameObject);
                        CombatManager.playerMeleeAttack(1,1);
                        PlayerScript.singleton_Player.GainActionPoints(PlayerScript.singleton_Player.APHit*1);
                    }
                    else if (AttackType[hitIndex] == "PS2")
                    {
                        AkSoundEngine.PostEvent("Play_PS1", gameObject);
                        CombatManager.playerMeleeAttack(2,1);
                        PlayerScript.singleton_Player.GainActionPoints(PlayerScript.singleton_Player.APHit*1);
                    }
                    else if (AttackType[hitIndex] == "PS3")
                    {
                        AkSoundEngine.PostEvent("Play_PS2", gameObject);
                        CombatManager.playerMeleeAttack(2,1);
                        PlayerScript.singleton_Player.GainActionPoints(PlayerScript.singleton_Player.APHit*3);
                    }
                    NextInLine();
                    //hitIndex++;
                    Debug.Log("Ok Hit");
                }
            }
        }
        
        if (error > leniency && error < leniency*2) // Player hits too early but not too far
        {
            if (hitIndex < CueObjects.Count){CueObjects[hitIndex].GetComponent<Image>().color = new Color32(255,0,0,255);}

            if (Input.GetKeyDown("space"))
            {
                Debug.Log("Miss Hit");
                CombatManager.playerMeleeMiss();
                NextInLine();
                //hitIndex++;
            }
        }

        if (error < (-leniency/4)) //If player goes over time
        {
            if (hitIndex < CueObjects.Count){CueObjects[hitIndex].GetComponent<Image>().color = new Color32(25,144,144,255);}
            NextInLine();
            //hitIndex++;
            CombatManager.playerMeleeMiss();
        }
    }

    private void NextInLine()
    {
        hitIndex++;
        //CueObjects.RemoveAt(hitIndex);
        //CueTimes.RemoveAt(hitIndex);
        //AttackType.RemoveAt(hitIndex);
    }
    

    void MusicCallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
  {
    AkMusicSyncCallbackInfo _musicInfo = (AkMusicSyncCallbackInfo) in_info;
    switch (_musicInfo.musicSyncType)
    {
        case AkCallbackType.AK_MusicSyncEntry:
            sequenceSecondsPerBar = _musicInfo.segmentInfo_fBarDuration;
            sequenceSecondsPerBeat = _musicInfo.segmentInfo_fBeatDuration;
            TravelTime = sequenceSecondsPerBeat*4;
            break;
        case AkCallbackType.AK_MusicSyncUserCue:
            CustomCues(_musicInfo.userCueName, _musicInfo);
        break;
        case AkCallbackType.AK_MusicSyncExit:
            if (AttackCounter <= 0)
            {
                ResetVariables();
                CombatManager.SkipPlayerTurn();
            }

            if (AttackCounter > 0)
            {
                AttackCounter--;
                if (CombatManager.singleton_CombatManager.LockedOut == false)
                {
                    ourActionSlotManager.SubtractActionCost();
                }
            }
            else
            {
                AttackCounter = 0;
            }
            
            break;
    }
  }

    private void PlayerSequenceStart()
    {
        
    }
    
    public void CustomCues(string cueName, AkMusicSyncCallbackInfo _musicInfo)
    {
        switch (cueName)
        {
            case "Example Case":
                break;
            case "PS1":
                addTime(playheadPosition,"PS1");
                break;
            case "PS2":
                addTime(playheadPosition,"PS2");
                break;
            case "PS3":
                addTime(playheadPosition,"PS3");
                break;
            case "PS4":
                addTime(playheadPosition,"PS4");
                break;
            case "Next1":
                Debug.Log("Next Called");
                if (AttackCounter > 0)
                {
                    AkSoundEngine.PostEvent("Play_AttackSequences", gameObject);
                    
                    playingID2 = AttackSequence2.Post(gameObject,
                            (uint) (AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition),
                            MusicCallbackFunction);
                            
                }
                break;
            case "Next2":
                Debug.Log("Next Called");
                if (AttackCounter > 0)
                {
                    AkSoundEngine.PostEvent("Play_AttackSequences", gameObject);
                    
                    playingID2 = AttackSequence3.Post(gameObject,
                        (uint) (AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition),
                        MusicCallbackFunction);
                            
                }
                break;
            case "Next3":
                Debug.Log("Next Called");
                if (AttackCounter > 0)
                {
                    AkSoundEngine.PostEvent("Play_AttackSequences", gameObject);
                    
                    playingID2 = AttackSequence4.Post(gameObject,
                        (uint) (AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition),
                        MusicCallbackFunction);
                            
                }
                break;
            case "Start":
                AkSoundEngine.PostEvent("Play_AttackSequences", gameObject);
                playingID = AttackSequence1.Post(gameObject,
                    (uint) (AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition),
                    MusicCallbackFunction);
                break;
            default:
                break;
        }
    }

    private void addTime(float playheadPosition, String type)
    {
        CueTimes.Add(playheadPosition);
        AttackType.Add(type);
    }

    public void NoteSpawner()
    {
        GameObject ourCircle = Instantiate(beatCircle);
        CueObjects.Add(ourCircle);
        BeatEntity ourEntity = ourCircle.GetComponent<BeatEntity>();
        if(CombatManager.singleton_CombatManager.LockedOut){ourEntity.EnableLockout();}
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
