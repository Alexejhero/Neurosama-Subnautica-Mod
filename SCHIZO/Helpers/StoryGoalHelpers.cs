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


    public static bool IsCompleted(string goal) => !IsStoryEnabled() || string.IsNullOrEmpty(goal) ||
                                                   (StoryGoalManager.main && StoryGoalManager.main.IsGoalComplete(goal));

    /// <returns>Whether the handlers for this goal were notified of completion - i.e. whether the goal wasn't completed before this trigger.</returns>
    public static bool Trigger(string goal) => StoryGoalManager.main.OnGoalComplete(goal);

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
