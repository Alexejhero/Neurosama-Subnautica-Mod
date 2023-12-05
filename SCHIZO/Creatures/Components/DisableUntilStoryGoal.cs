using SCHIZO.Helpers;
using Story;

namespace SCHIZO.Creatures.Components;
partial class DisableUntilStoryGoal : IStoryGoalListener
{
    private string StoryGoal => RetargetHelpers.Pick(storyGoalSN, storyGoalBZ);
    public void Start()
    {
        StoryGoalManager.main.AddListener(this);
        UpdateActive();
    }

    public void NotifyGoalComplete(string key) => UpdateActive();

    public void NotifyGoalReset(string key) => UpdateActive();

    public void NotifyGoalsDeserialized() => UpdateActive();

    private void UpdateActive() => gameObject.SetActive(StoryGoalHelpers.IsCompleted(StoryGoal));
}
