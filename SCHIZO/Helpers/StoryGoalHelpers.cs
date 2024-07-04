using System;
using System.Runtime.CompilerServices;
using Story;

namespace SCHIZO.Helpers;

public static class StoryGoalHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStoryEnabled()
    {
#if BELOWZERO
        return GameModeManager.GetOption<bool>(GameOption.Story);
#else
        return true;
#endif
    }

    public static bool Matches(string goal1, string goal2) => string.Equals(goal1, goal2, StringComparison.OrdinalIgnoreCase);
    public static bool IsCompleted(string goal) => !IsStoryEnabled() || string.IsNullOrEmpty(goal) ||
                                                   (StoryGoalManager.main && StoryGoalManager.main.IsGoalComplete(goal));

    /// <summary>
    /// "Trigger" (complete) the given story goal.<br/>
    /// If the goal is new (not already completed), any attached handlers will be notified.
    /// </summary>
    /// <returns>Whether this call is the one that completed the goal - i.e. if nothing else completed it before.</returns>
    public static bool Complete(string goal) => StoryGoalManager.main.OnGoalComplete(goal);

    public static void Reset(string goal)
    {
#if BELOWZERO
        StoryGoalManager.main.OnGoalReset(goal);
#else
        // Subnautica's SGM doesn't support resetting goals but we'll uncomplete it anyway
        StoryGoalManager.main.completedGoals.Remove(goal);
#endif
    }
}
