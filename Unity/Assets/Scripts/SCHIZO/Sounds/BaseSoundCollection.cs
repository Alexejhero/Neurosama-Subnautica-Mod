using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.API.Unity.Sounds
{
    public abstract class BaseSoundCollection : ScriptableObject
    {
        public abstract IEnumerable<AudioClip> GetSounds();
    }
}
