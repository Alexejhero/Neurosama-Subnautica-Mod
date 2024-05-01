using SCHIZO.Helpers;
using Story;

namespace SCHIZO.Items.Components;
partial class DisableUntilStoryGoal : IStoryGoalListener
{
    private string _storyGoal;
    public void Start()
    {
        _storyGoal = RetargetHelpers.Pick(storyGoalSN, storyGoalBZ);
        StoryGoalManager.main.AddListener(this);
        UpdateActive();
    }

    private void OnDestroy()
    {
        StoryGoalManager.main.RemoveListener(this);
    }

    private void OnEnable()
    {
        UpdateActive();
    }

    public void NotifyGoalComplete(string key) => UpdateActive(key);
    public void NotifyGoalReset(string key) => UpdateActive(key);
    public void NotifyGoalsDeserialized() => UpdateActive();

    private void UpdateActive(string key = null)
    {
        if (key is { } && !StoryGoalHelpers.Matches(key, _storyGoal))
            return;

        bool shouldBeActive = StoryGoalHelpers.IsCompleted(_storyGoal);
        if (shouldBeActive == gameObject.activeSelf) return;

        Pickupable pickupable = GetComponent<Pickupable>();
        if (pickupable && pickupable.attached) return; // otherwise inventory items swim away/are invisible

        gameObject.SetActive(shouldBeActive);
    }
}
