﻿using SCHIZO.Attributes;
using TriInspector;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ECCLibrary.Mono
{
    public sealed class SwimInSchoolFieldSetter : MonoBehaviour
    {
        [Required, ExposedType("SwimInSchool")]
        public MonoBehaviour behaviour;

        public float breakDistance = 20;
        [Range(0, 1)] public float percentFindLeaderRespond = 0.5f;
        [Range(0, 1)] public float chanceLoseLeader = 0.1f;
    }
}
