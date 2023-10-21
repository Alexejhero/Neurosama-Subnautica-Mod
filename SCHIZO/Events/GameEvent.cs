using SCHIZO.Helpers;
using Story;

namespace SCHIZO.Events;
public partial class GameEvent
{
    public abstract bool IsOccurring { get; }
    protected Player player;
    protected GameEventsConfig config;
    protected void Awake()
    {
        player = Player.main;
        config = GetComponent<GameEventsConfig>();
    }

    public virtual void StartEvent()
    {
        LOGGER.LogDebug($"{GetType().Name} started");
    }

    public virtual void EndEvent()
    {
        LOGGER.LogDebug($"{GetType().Name} ended");
    }

    protected abstract void UpdateLogic();
    protected abstract void UpdateRender();
    protected abstract bool ShouldStartEvent();

    protected string RequiredStoryGoal => RetargetHelpers.Pick(requiredStoryGoal_SN, requiredStoryGoal_BZ);
    protected bool IsStoryGoalMet()
    {
        return !RetargetHelpers.IsStoryEnabled()
            && (string.IsNullOrEmpty(RequiredStoryGoal)
            || StoryGoalManager.main && StoryGoalManager.main.IsGoalComplete(RequiredStoryGoal));
    }

    protected void FixedUpdate()
    {
        if (IsOccurring) UpdateLogic();
        else
        {
            bool shouldStart = canAutoStart
                && config.AutoStartEvents
                && ShouldStartEvent()
                && IsStoryGoalMet();
            if (shouldStart) StartEvent();
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
