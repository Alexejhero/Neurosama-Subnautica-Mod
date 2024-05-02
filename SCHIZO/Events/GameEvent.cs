using SCHIZO.Helpers;
using Story;

namespace SCHIZO.Events;
public partial class GameEvent : IStoryGoalListener
{
    public readonly record struct StoryGoals(GameEvent evt)
    {
        public readonly string Unlock => $"{evt.EventName}.UnlockedAutoStart";
        public readonly string FirstTime => $"{evt.EventName}.FirstTime";
    }
    public abstract bool IsOccurring { get; }
    public bool IsFirstTime { get; private set; } = true;

    protected Player player;
    protected string RequiredStoryGoal => RetargetHelpers.Pick(requiredStoryGoal_SN, requiredStoryGoal_BZ);
    protected bool IsUnlocked { get; private set; }
    public StoryGoals Goals { get; private set; }
    public string EventName => GetType().Name;

    protected void Awake()
    {
        Goals = new StoryGoals(this);
        player = Player.main;

        StoryGoalManager.main.AddListener(this);
    }
    protected void OnDestroy()
    {
        if (StoryGoalManager.main)
            StoryGoalManager.main.RemoveListener(this);
    }

    protected virtual void Start()
    {
#if !BELOWZERO
        NotifyGoalsDeserialized();
#endif
    }

    #region Unlock goals
    public void NotifyGoalComplete(string goal)
    {
        if (string.Equals(goal, RequiredStoryGoal, System.StringComparison.OrdinalIgnoreCase))
            Unlock();
    }

    public void NotifyGoalReset(string goal)
    {
        if (string.Equals(goal, RequiredStoryGoal, System.StringComparison.OrdinalIgnoreCase))
            Lock();
    }

    public void NotifyGoalsDeserialized()
    {
        if (StoryGoalHelpers.IsCompleted(Goals.FirstTime)) IsFirstTime = false;
        if (StoryGoalHelpers.IsCompleted(Goals.Unlock)) IsUnlocked = true;
        if (string.IsNullOrEmpty(RequiredStoryGoal) || StoryGoalHelpers.IsCompleted(RequiredStoryGoal))
            Unlock();
    }

    public void Unlock()
    {
        StoryGoalHelpers.Trigger(Goals.Unlock);
        IsUnlocked = true;
    }

    public void Lock()
    {
        StoryGoalHelpers.Reset(Goals.Unlock);
        IsUnlocked = false;
    }
    #endregion Unlock goals

    public virtual void StartEvent()
    {
        string msg = $"{EventName} started";
        IsFirstTime = !StoryGoalHelpers.IsCompleted(Goals.FirstTime);
        if (IsFirstTime)
        {
            StoryGoalHelpers.Trigger(Goals.FirstTime);
            msg += " (first time)";
        }
        LOGGER.LogMessage(msg);
    }

    public virtual void EndEvent()
    {
        LOGGER.LogMessage($"{EventName} ended");
    }

    /// <summary>
    /// Called in FixedUpdate if the event is occurring.
    /// </summary>
    protected abstract void UpdateLogic();
    /// <summary>
    /// Called in Update if the event is occurring.
    /// </summary>
    protected abstract void UpdateRender();
    /// <summary>
    /// Called in FixedUpdate to check whether to auto-start the event.<br/>
    /// This function should check the start conditions specific to this event - random chance, time of day, etc.
    /// </summary>
    /// <returns>Whether to start the event.</returns>
    protected abstract bool ShouldStartEvent();

    protected void FixedUpdate()
    {
        if (IsOccurring) UpdateLogic();
        else
        {
            bool ableToAutoStart = canAutoStart
                && GameEventsManager.AutoStart
                && IsUnlocked;
            if (ableToAutoStart && ShouldStartEvent()) StartEvent();
        }
    }

    protected void Update()
    {
        if (IsOccurring) UpdateRender();
    }

    protected void OnDisable()
    {
        if (IsOccurring) EndEvent();
    }
}
