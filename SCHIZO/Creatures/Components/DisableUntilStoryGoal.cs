using SCHIZO.Helpers;
using Story;

namespace SCHIZO.Creatures.Components;
partial class DisableUntilStoryGoal : IStoryGoalListener
{
    private string storyGoal;
    public void Start()
    {
        storyGoal = RetargetHelpers.Pick(storyGoalSN, storyGoalBZ);
        StoryGoalManager.main.AddListener(this);
        UpdateActive();
    }

    private void OnDestroy()
    {
        StoryGoalManager.main.RemoveListener(this);
    }

    public void NotifyGoalComplete(string key) => UpdateActive();

    public void NotifyGoalReset(string key) => UpdateActive();

    public void NotifyGoalsDeserialized() => UpdateActive();

    private void UpdateActive() => gameObject.SetActive(StoryGoalHelpers.IsCompleted(storyGoal));
}
