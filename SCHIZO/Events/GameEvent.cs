using SCHIZO.Helpers;
using Story;

namespace SCHIZO.Events;
public partial class GameEvent : IStoryGoalListener
{
    public record StoryGoals(GameEvent evt)
    {
        public string Unlock => $"{evt.Name}.UnlockedAutoStart";
        public string FirstTime => $"{evt.Name}.FirstTime";
    }
    public abstract bool IsOccurring { get; }
    protected Player player;
    protected GameEventsManager config;
    protected string RequiredStoryGoal => RetargetHelpers.Pick(requiredStoryGoal_SN, requiredStoryGoal_BZ);
    protected bool IsUnlocked { get; private set; }
    public StoryGoals Goals { get; private set; }
    public string Name => GetType().Name;

    protected void Awake()
    {
        Goals = new StoryGoals(this);
        player = Player.main;
        config = GetComponent<GameEventsManager>();

        StoryGoalManager.main.AddListener(this);
    }

    #region Unlock goals
    // TODO: can/should this be improved?
    public void NotifyGoalComplete(string goal)
    {
        if (StoryGoal.Equals(goal, RequiredStoryGoal)) Unlock();
    }

    public void NotifyGoalReset(string key) => Lock();

    public void NotifyGoalsDeserialized() { }

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

    private void OnDestroy()
    {
        if (StoryGoalManager.main) StoryGoalManager.main.RemoveListener(this);
    }
    #endregion Unlock goals

    public virtual void StartEvent()
    {
        StoryGoalHelpers.Trigger(Goals.FirstTime);
        LOGGER.LogMessage($"{Name} started");
    }

    public virtual void EndEvent()
    {
        LOGGER.LogMessage($"{Name} ended");
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
    /// This method should 
    /// </summary>
    /// <returns>Whether to start the event.</returns>
    protected abstract bool ShouldStartEvent();

    protected void FixedUpdate()
    {
        if (IsOccurring) UpdateLogic();
        else
        {
            bool ableToAutoStart = canAutoStart
                && config.AutoStartEvents
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
