using System.Collections;
using Story;

namespace SCHIZO.Telemetry;

partial class StoryGoals : IStoryGoalListener
{
    protected override void Awake()
    {
        base.Awake();
        GameStatus.Loaded += () => StartCoroutine(CoAddListener());
    }

    private IEnumerator CoAddListener()
    {
        while (!StoryGoalManager.main) yield return null;

        LOGGER.LogDebug($"Attached story goal listener");
        StoryGoalManager.main.AddListener(this);
    }

    public void NotifyGoalComplete(string key)
    {
        LOGGER.LogWarning($"Completed {key}");
        SendTelemetry("story", new { completedGoal = key });
    }
    public void NotifyGoalReset(string key)
    {
        LOGGER.LogWarning($"Reset {key}");
        SendTelemetry("story", new { resetGoal = key });
    }

    public void NotifyGoalsDeserialized()
    {
        LOGGER.LogWarning($"Deserialized");
        // SendTelemetry("story", new { deserialized = "" });
    }
}
