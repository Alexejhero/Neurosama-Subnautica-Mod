using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    public sealed partial class DisableUntilStoryGoal : MonoBehaviour
    {
        [LabelText("Story Goal (SN)")]
        public string storyGoalSN;
        [LabelText("Story Goal (BZ)")]
        public string storyGoalBZ;
    }
}