/*
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CombatManager : MonoBehaviour
{
  [FormerlySerializedAs("startStateDuration")] [SerializeField]
  private float showMessageDuration;

  [SerializeField] private BeatmapManager beatmapManager;
  [SerializeField] private GameObject heroObjects;

  public static readonly UnityEvent<State> onStateChange = new UnityEvent<State>();

  private float currentWaitTime;
  private bool isWaiting;

  public enum State
  {
    Unspecified, // Should never be used
    Inactive, // Out of combat
    PreStart, // Monsters are arriving on the scene
    Start, // Show start message
    HeroOne, // Hero 1's turn to select command
    HeroTwo, // Hero 2's turn
    HeroThree, // Hero 3's turn
    DelayExecution, // Wait until it's the right time to generate beatmap
    PreExecution, // Generate beatmap
    Execution, // Player executes beatmap
    Win, // Show win message
    Lose, // Show lose message
    EndWin, // Exit out of combat on a win
    EndLose // Exit out of combat on a loss
  }

  /// <summary>
  /// Combatants sorted in initiative order
  /// </summary>
  public static List<Combatant> Combatants { get; private set; }

  public static State CurrentState { get; private set; }

  public static List<Hero> Heroes { get; private set; }

  public static List<Monster> Monsters { get; private set; }

  public static Monster FirstLivingMonster
  {
    get { return Monsters.FirstOrDefault(monster => !monster.IsDead); }
  }

  public static Hero CurrentHero
  {
    get
    {
      return CurrentState switch
      {
        State.HeroOne => Heroes[0],
        State.HeroTwo => Heroes[1],
        State.HeroThree => Heroes[2],
        _ => null
      };
    }
  }

  private void Awake()
  {
    Assert.IsTrue(beatmapManager, "beatmapManager is empty");

    beatmapManager.complete.AddListener(AdvanceState);
    beatmapManager.hit.AddListener(ReadHit);

    Heroes = heroObjects.GetComponentsInChildren<Hero>().ToList();
    Assert.IsTrue(Heroes.Count == 3, "Heroes.Count != 3");

    // Ensure heroes are ordered by ID.
    var isSorted = true;
    for (var i = 0; i < Heroes.Count; i++)
    {
      if (Heroes[i].HeroId == i + 1) continue;
      Debug.LogWarning("Heroes are not ordered by ID! Will sort heroes.");
      isSorted = false;
    }

    if (!isSorted)
    {
      Heroes.Sort(CompareHeroIds);
    }

    foreach (Hero hero in Heroes)
    {
      hero.dead.AddListener(() => beatmapManager.RemoveCombatantNotes(hero));
    }

    Timer.onExpire.AddListener(Lose);

    CurrentState = State.Inactive;
  }

  private void Update()
  {
    if (CurrentState == State.Inactive || CurrentState == State.EndWin || CurrentState == State.EndLose) return;

    if (CurrentState != State.PreStart && CurrentState != State.Start)
    {
      if (AllMonstersDead())
      {
        Win();
      }

      if (AllHeroesDead())
      {
        Lose();
      }
    }

    switch (CurrentState)
    {
      case State.Start:
        if (!isWaiting)
        {
          foreach (Combatant combatant in Combatants)
          {
            combatant.SetInitialPosition();
          }

          currentWaitTime = showMessageDuration;
          isWaiting = true;
        }

        currentWaitTime -= Time.deltaTime;

        if (currentWaitTime <= 0)
        {
          Timer.Activate();
          isWaiting = false;
          ChangeState(State.HeroOne);
        }

        break;
      case State.DelayExecution:
        // Beatmap execution should start at an odd-numbered bar, so generate notes at the bar before it
        // (an even-numbered bar).
        if (GlobalVariables.currentBar % 2 == 0)
        {
          ChangeState(State.PreExecution);
        }

        break;
      case State.PreExecution:
        // For now, each Monster will target a random Hero.
        SetRandomTargets();

        GeneratePatterns();

        ChangeState(State.Execution);
        break;
      case State.Lose:
        DelayEnd(State.EndLose);

        break;
      case State.Win:
        DelayEnd(State.EndWin);

        break;
    }
  }

  private void OnGUI()
  {
#if UNITY_EDITOR
    if (Monsters == null) return;

    for (var i = 0; i < Monsters.Count; i++)
    {
      GUI.Label(new Rect(0, 30 * i, 200, 30),
        Monsters[i].Name + " HP: " + Monsters[i].CurrentHealth + " / " + Monsters[i].MaxHealth);
    }
#endif
  }

  public void Reset()
  {
    foreach (Hero hero in Heroes)
    {
      hero.ResetEverything();
    }

    CurrentState = State.Inactive;
    isWaiting = false;
  }

  public void Begin(Encounter encounter)
  {
    if (CurrentState != State.Inactive)
    {
      Debug.LogError("Failed to start combat. Current state is not inactive (did you call Reset?)");
      return;
    }

    Monsters = encounter.GetComponentsInChildren<Monster>().ToList();

    SortByInitiative();

    foreach (Monster monster in Monsters)
    {
      monster.dead.AddListener(() => beatmapManager.RemoveCombatantNotes(monster));
    }

    encounter.GetComponent<CombatantsMovement>().onComplete.AddListener(StartFight);

    ChangeState(State.PreStart);
  }

  private void StartFight()
  {
    ChangeState(State.Start);
  }

  private void Lose()
  {
    ChangeState(State.Lose);
    Timer.Deactivate();
    beatmapManager.ForceFinish();
  }

  private void Win()
  {
    ChangeState(State.Win);
    Timer.Deactivate();
    beatmapManager.ForceFinish();
  }

  private void DelayEnd(State endState)
  {
    if (!isWaiting)
    {
      currentWaitTime = showMessageDuration;
      isWaiting = true;
    }

    currentWaitTime -= Time.deltaTime;

    if (currentWaitTime <= 0)
    {
      isWaiting = false;
      ChangeState(endState);
    }
  }

  public void SubmitCommand(Command command)
  {
    if (command is Consumable consumable)
    {
      consumable.DecrementAmountOwned();
    }

    CurrentHero.SubmitCommand(command);

    AdvanceState();
  }

  private void AdvanceState()
  {
    switch (CurrentState)
    {
      case State.HeroOne:
        if (Heroes[1].IsDead)
        {
          if (Heroes[2].IsDead)
          {
            DeterminePreExecutionState();
          }
          else
          {
            ChangeState(State.HeroThree);
          }
        }
        else
        {
          ChangeState(State.HeroTwo);
        }

        break;
      case State.HeroTwo:
        if (Heroes[2].IsDead)
        {
          DeterminePreExecutionState();
        }
        else
        {
          ChangeState(State.HeroThree);
        }

        break;
      case State.HeroThree:
        DeterminePreExecutionState();
        break;
      case State.Execution:
        foreach (Hero hero in Heroes)
        {
          hero.ResetCommand();
          hero.CheckTemporaries();
        }

        if (Heroes[0].IsDead)
        {
          ChangeState(Heroes[1].IsDead ? State.HeroThree : State.HeroTwo);
        }
        else
        {
          ChangeState(State.HeroOne);
        }

        break;
    }
  }

  private bool AllHeroesDead()
  {
    return Heroes.All(hero => hero.IsDead);
  }

  private bool AllMonstersDead()
  {
    return Monsters.All(monster => monster.IsDead);
  }

  private static void ChangeState(State state)
  {
    CurrentState = state;

    switch (state)
    {
      case State.HeroOne:
        Heroes[0].Spotlight();
        Heroes[1].ResetPosition();
        Heroes[2].ResetPosition();
        break;
      case State.HeroTwo:
        Heroes[0].ResetPosition();
        Heroes[1].Spotlight();
        Heroes[2].ResetPosition();
        break;
      case State.HeroThree:
        Heroes[0].ResetPosition();
        Heroes[1].ResetPosition();
        Heroes[2].Spotlight();
        break;
      default:
        Heroes[0].ResetPosition();
        Heroes[1].ResetPosition();
        Heroes[2].ResetPosition();
        break;
    }

    onStateChange.Invoke(state);
  }

  private void DeterminePreExecutionState()
  {
    // Beatmap execution should start at an odd-numbered bar, so we should generate notes at the bar before it
    // (an even-numbered bar). Otherwise, wait until we are at an even-numbered bar.
    ChangeState(GlobalVariables.currentBar % 2 == 0 ? State.PreExecution : State.DelayExecution);
  }

  private void SetRandomTargets()
  {
    List<Hero> livingHeroes = Heroes.Where(hero => !hero.IsDead).ToList();
    foreach (Monster monster in Monsters.Where(monster => !monster.IsDead))
    {
      int index = Random.Range(0, livingHeroes.Count);
      monster.SetTarget(livingHeroes[index]);
    }
  }

  private void GeneratePatterns()
  {
    var combatantPatterns = new Dictionary<Combatant, Pattern>();

    foreach (Combatant combatant in Combatants.Where(combatant => !combatant.IsDead))
    {
      switch (combatant)
      {
        case Hero hero:
        {
          Command command = hero.GetSubmittedCommand();
          if (command is Stance stance)
          {
            stance.Execute(hero);
          }
          else
          {
            combatantPatterns[hero] = DataManager.AllPatterns[command.patternId];
          }

          break;
        }
        case Monster monster:
        {
          combatantPatterns[monster] = DataManager.AllPatterns[monster.PatternId];
          break;
        }
        default:
          Debug.LogError(
            $"Could not generate pattern for combatant {combatant.Name}. It's neither a Hero nor Monster!");
          break;
      }
    }

    // We directly take currentBar since we want to generate notes at the next bar. Current bar starts at 1 while time
    // starts at 0. This offset handles the desired behavior for us.
    float executionStartTime = GlobalVariables.currentBar * AudioEvents.secondsPerBar;
    beatmapManager.GenerateBeatmap(combatantPatterns, executionStartTime);
  }

  private void ReadHit(BeatmapManager.Note note, BeatmapManager.AccuracyGrade accuracyGrade)
  {
    Combatant combatant = note.combatant;
    if (combatant.IsDead)
    {
      Debug.LogError("Read hit from a combatant that is dead!");
      return;
    }

    switch (combatant)
    {
      case Monster monster:
      {
        float damageMultiplier = accuracyGrade switch
        {
          BeatmapManager.AccuracyGrade.Perfect => 0f,
          BeatmapManager.AccuracyGrade.Great => 0.25f,
          BeatmapManager.AccuracyGrade.Good => 0.5f,
          _ => 1f
        };

        monster.AttackTarget(damageMultiplier, note.isLastOfCombatant);
        break;
      }
      case Hero hero:
      {
        float effectMultiplier = accuracyGrade switch
        {
          BeatmapManager.AccuracyGrade.Perfect => 1f,
          BeatmapManager.AccuracyGrade.Great => 0.5f,
          BeatmapManager.AccuracyGrade.Good => 0.25f,
          _ => 0f
        };

        Command command = hero.GetSubmittedCommand();
        switch (command)
        {
          case Macro macro:
            macro.Execute(hero, effectMultiplier, note.isLastOfCombatant);
            FXManager.SpawnMacroPulse(combatant);
            break;
          case Attack attack:
            attack.Execute(hero, effectMultiplier, note.isLastOfCombatant);
            break;
          case Consumable consumable:
            consumable.Execute(hero, effectMultiplier, note.isLastOfCombatant);
            break;
        }

        break;
      }
      default:
        Debug.LogError("The combatant is neither a Hero nor a Monster. ReadHit failed.");
        break;
    }
  }

  private static void SortByInitiative()
  {
    Combatants = new List<Combatant>();
    Combatants.AddRange(Heroes);
    Combatants.AddRange(Monsters);
    Combatants.Sort(CompareCombatantSpeeds);
  }

  private static int CompareCombatantSpeeds(Combatant x, Combatant y)
  {
    return y.Speed.CompareTo(x.Speed);
  }

  private static int CompareHeroIds(Hero x, Hero y)
  {
    return y.HeroId.CompareTo(x.HeroId);
  }
}
*/