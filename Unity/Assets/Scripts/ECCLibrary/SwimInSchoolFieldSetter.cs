using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ECCLibrary.Mono
{
    public class SwimInSchoolFieldSetter : MonoBehaviour
    {
        [Foldout("Component References"), ValidateInput(nameof(behaviour_Validate), "Behaviour must be of type SwimInSchool!")]
        public MonoBehaviour behaviour;
        private static bool behaviour_Validate(MonoBehaviour behaviour) => behaviour && behaviour.GetType().Name == "SwimInSchool";

        public float breakDistance = 20;
        [Range(0, 1)] public float percentFindLeaderRespond = 0.5f;
        [Range(0, 1)] public float chanceLoseLeader = 0.1f;
    }
}
