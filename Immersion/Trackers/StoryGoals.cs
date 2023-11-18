using System.Collections;
using Story;

namespace Immersion.Trackers;

public sealed partial class StoryGoals : Tracker, IStoryGoalListener
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
        if (StoryGoalManager.main.IsGoalComplete(key)) return;

        LOGGER.LogWarning($"Completed {key}");
        string description = TryGetDescription(key, out string desc) ? desc : key;
        Send("story", new { description });
    }
    public void NotifyGoalReset(string key)
    {
        // goals can only be reset with console commands
        LOGGER.LogWarning($"Reset {key}");
    }
    // called when loading game
    public void NotifyGoalsDeserialized()
    {
        LOGGER.LogWarning($"Deserialized");
        // send "story so far"? (probably not)
    }

    private bool TryGetDescription(string goal, out string description)
    {
        description = Descriptions.StoryGoals.ResourceManager.GetString(goal);
        return !string.IsNullOrEmpty(description);
    }
}
