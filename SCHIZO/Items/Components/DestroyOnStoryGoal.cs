using SCHIZO.Helpers;
using Story;

namespace SCHIZO.Items.Components;
partial class DestroyOnStoryGoal : IStoryGoalListener
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

    public void NotifyGoalComplete(string key) => UpdateActive(key);

    public void NotifyGoalReset(string key) => UpdateActive(key);

    public void NotifyGoalsDeserialized() => UpdateActive();

    private void UpdateActive(string key = null)
    {
        if (key is { } && !StoryGoalHelpers.Matches(key, storyGoal))
            return;

        if (StoryGoalHelpers.IsCompleted(storyGoal))
            Destroy(GetComponentInParent<PrefabIdentifier>().gameObject);
    }
}
