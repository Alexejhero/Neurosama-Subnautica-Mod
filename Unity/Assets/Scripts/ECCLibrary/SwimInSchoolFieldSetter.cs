using NaughtyAttributes;
using SCHIZO.Attributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ECCLibrary.Mono
{
    public class SwimInSchoolFieldSetter : MonoBehaviour
    {
        [Foldout("Component References"), Required, ExposedType("SwimInSchool")]
        public MonoBehaviour behaviour;

        public float breakDistance = 20;
        [Range(0, 1)] public float percentFindLeaderRespond = 0.5f;
        [Range(0, 1)] public float chanceLoseLeader = 0.1f;
    }
}
