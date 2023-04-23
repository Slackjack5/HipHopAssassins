
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class AudioEvents : MonoBehaviour
{
  public static AudioEvents singleton_AudioEvents;
  public class SegmentPosition
  {
    internal float value;
    internal bool isValid;

    public float Value => value;
    public bool IsValid => isValid;
  }
  //Wwise
  public AK.Wwise.Event rhythmHeckinEvent;
  public UnityEvent OnLevelEnded;
  public float secondsPerBeat;
  public float secondsPerBar;
  public float barAheadTime;
  public static float bpm;
  public float masterCurrentPosition;
  

  //Unity Events
  public UnityEvent OnEveryGrid;
  public UnityEvent OnEveryBeat;
  public UnityEvent OnEveryOffbeat;
  public UnityEvent OnEvery2ndBeat;
  public UnityEvent OnEveryBar;

  public UnityEvent OnSubtractTime;

  private bool isOnEveryOffbeatInvoked;

  //Functions
  public int GridCount = 0;
  public int gridCounter = 0;

  public bool startCounting = false;

  //Timing
  public int currentBar = GlobalVariables.currentBar;
  public int currentBeat = GlobalVariables.currentBeat;
  public int currentGrid = GlobalVariables.currentGrid;
  public bool gameStarted = GlobalVariables.songStarted;

  private static AkSegmentInfo currentSegment;
  private static int currentBarStartTime; // The time, in milliseconds, that the current bar started at
  private static int currentBeatStartTime; // The time, in milliseconds, that the current beat started at
  private static int currentLoop = -1; // Counts the amount of times the song has looped
  private static int songLength; // The length of the song in milliseconds
  private static bool isSegmentPositionReady;
  private static GameObject Self;

  //id of the wwise event - using this to get the playback position
  static uint playingID;

  /// <summary>
  /// The time elapsed, in seconds, since the current bar started
  /// </summary>
  public static float CurrentBarTime =>
    // iCurrentPosition is in milliseconds.
    (float) (currentSegment.iCurrentPosition - currentBarStartTime) / 1000;

  private void Awake()
  {
    if (singleton_AudioEvents != null && singleton_AudioEvents != this)
    {
      Destroy(this);
    }
    else
    {
      singleton_AudioEvents = this;
    }
  }

  private void Start()
  {
    playingID = rhythmHeckinEvent.Post(gameObject,(uint) (AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition), MusicCallbackFunction);
    //AkSoundEngine.SetRTPCValue("Intensity", 1);
    currentSegment = new AkSegmentInfo();
    GlobalVariables.songStarted = false;
    isSegmentPositionReady = true;
    Self = gameObject;
  }

  private void Update()
  {

    barAheadTime = masterCurrentPosition + secondsPerBar;
    if (!isOnEveryOffbeatInvoked && currentSegment.iCurrentPosition >= currentBeatStartTime + secondsPerBeat * 1000 / 2)
    {
      OnEveryOffbeat.Invoke();
      isOnEveryOffbeatInvoked = true;
    }
  }

  private void FixedUpdate()
  {
    AkSoundEngine.GetPlayingSegmentInfo(playingID, currentSegment);
    Debug.Log(secondsPerBeat);
    //Segment Info
    AkSegmentInfo segmentInfo = new AkSegmentInfo();
    AkSoundEngine.GetPlayingSegmentInfo(playingID, segmentInfo, true);
    masterCurrentPosition = (float) segmentInfo.iCurrentPosition/1000f; // --> position in ms
    
    currentBar = GlobalVariables.currentBar;
    currentBeat = GlobalVariables.currentBeat;
    currentGrid = GlobalVariables.currentGrid;

  }

  private void OnGUI()
  {
#if UNITY_EDITOR
#endif
  }

  void MusicCallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
  {
    AkMusicSyncCallbackInfo _musicInfo = (AkMusicSyncCallbackInfo) in_info;

    switch (_musicInfo.musicSyncType)
    {
      case AkCallbackType.AK_MusicSyncUserCue:

        CustomCues(_musicInfo.userCueName, _musicInfo);

        secondsPerBeat = _musicInfo.segmentInfo_fBeatDuration;
        secondsPerBar = _musicInfo.segmentInfo_fBarDuration;
        bpm = _musicInfo.segmentInfo_fBeatDuration * 60f;
        break;
      case AkCallbackType.AK_MusicSyncBeat:
        IncreaseBeat();

        currentBeatStartTime = currentSegment.iCurrentPosition;
        OnEveryBeat.Invoke();
        isOnEveryOffbeatInvoked = false;
        

        if (GlobalVariables.currentBeat % 2 == 0)
        {
          OnEvery2ndBeat.Invoke();
        }

        break;
      case AkCallbackType.AK_MusicSyncBar:
        IncreaseBar();
        //MusicManager.singleton_MusicManager.GridSpawner();
        //I want to make sure that the secondsPerBeat is defined on our first measure.
        if (GlobalVariables.songStarted == false)
        {
          // If the game hasn't started yet, start it on beat 1
          GlobalVariables.songStarted = true;
        }

        secondsPerBeat = _musicInfo.segmentInfo_fBeatDuration;
        secondsPerBar = _musicInfo.segmentInfo_fBarDuration;

        currentBarStartTime = currentSegment.iCurrentPosition;

        OnEveryBar.Invoke();
        Debug.Log("Calling Every Bar");
        break;
      case AkCallbackType.AK_MusicSyncGrid:
        IncreaseGrid();
        OnEveryGrid.Invoke();
        break;
      case AkCallbackType.AK_MusicSyncEntry:
        currentLoop++;

        if (currentLoop > 0)
        {
          songLength = currentSegment.iCurrentPosition;
        }

        isSegmentPositionReady = true;

        GlobalVariables.currentBeat = 0;
        GlobalVariables.currentGrid = 0;

        break;
      case AkCallbackType.AK_MusicSyncExit:
        // Exit is called after bar, beat, and grid callbacks.
        // Decrement currentBar so that it doesn't double-count.
        GlobalVariables.currentBar -= 1;

        isSegmentPositionReady = false;
        break;
    }
  }

  private void Listening()
  {
    Debug.Log("Stinger Heard");
  }

  private void IncreaseBar()
  {
    if (GlobalVariables.currentBar < 4) //Insert Time Signature
    {
      GlobalVariables.currentBar += 1;
    }
    else if (GlobalVariables.currentBar==4)
    {
      GlobalVariables.currentBar = 1;
    }
  }

  private void IncreaseBeat()
  {
    if (GlobalVariables.currentBeat < 4) //Insert Time Signature
    {
      GlobalVariables.currentBeat += 1;
    }
    else if (GlobalVariables.currentBeat==4)
    {
      GlobalVariables.currentBeat = 1;
    }
  }

  private void IncreaseGrid()
  {
    if (GlobalVariables.currentGrid < 4) //Insert Time Signature
    {
      GlobalVariables.currentGrid += 1;
    }
    else if (GlobalVariables.currentGrid==4)
    {
      GlobalVariables.currentGrid = 1;
    }
    
  }

  /// <summary>
  /// When isValid is true, value is the current time elapsed, in seconds, since the song started.
  /// When isValid is false, value should not be used.
  /// </summary>
  public static SegmentPosition GetCurrentSegmentPosition()
  {
    // iCurrentPosition is in milliseconds.
    return new SegmentPosition
    {
      value = (float) (currentLoop * songLength + currentSegment.iCurrentPosition) / 1000,
      isValid = isSegmentPositionReady
    };
  }
  
  

  public void CustomCues(string cueName, AkMusicSyncCallbackInfo _musicInfo)
  {
    switch (cueName)
    {
      case "Example Case":
        break;
      case "Q":
        Debug.Log("Found our Q!");
        break;
      
      default:
        break;
    }
  }

  private static void ResetTiming()
  {
    currentLoop = -1;
    GlobalVariables.currentBar = 0;
    GlobalVariables.currentBeat = 0;
    GlobalVariables.currentGrid = 0;
  }
}

public static class GlobalVariables
{
  public static int currentBar;
  public static int currentBeat;
  public static int currentGrid;
  public static bool songStarted;
}

