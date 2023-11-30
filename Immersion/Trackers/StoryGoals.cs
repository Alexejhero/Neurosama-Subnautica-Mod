using System.Collections;
using Immersion.Formatting;
using Nautilus.Utility;
using Story;

namespace Immersion.Trackers;

public sealed partial class StoryGoals : Tracker, IStoryGoalListener
{
    protected override void Awake()
    {
        base.Awake();
        SaveUtils.RegisterOnFinishLoadingEvent(() => StartCoroutine(CoAddListener()));
    }

    private IEnumerator CoAddListener()
    {
        while (!StoryGoalManager.main) yield return null;

        LOGGER.LogDebug($"Attached story goal listener");
        StoryGoalManager.main.AddListener(this);
    }

    public void NotifyGoalComplete(string key)
    {
        if (key == "OnPDAClosed") return;
        LOGGER.LogDebug($"Completed story goal {key}");

        if (TryGetDescription(key, out string description))
        {
            description = Format.FormatPlayer(description);
            React(Priority.Low, description);
        }
    }
    public void NotifyGoalReset(string key)
    {
        // goals can only be reset with console commands
        LOGGER.LogDebug($"Reset story goal {key}");
    }
    // called when loading game
    public void NotifyGoalsDeserialized()
    {
        LOGGER.LogDebug($"Deserialized story goals");
        // send "story so far"? (probably not)
    }

    private bool TryGetDescription(string goal, out string description)
        => StoryGoalDescriptions.TryGetValue(goal, out description)
            && !string.IsNullOrEmpty(description);
}