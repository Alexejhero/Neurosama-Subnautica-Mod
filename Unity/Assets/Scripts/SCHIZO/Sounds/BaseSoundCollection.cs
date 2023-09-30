

// ReSharper disable once CheckNamespace

using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.Unity.Sounds
{
    public abstract class BaseSoundCollection : ScriptableObject
    {
        public abstract IEnumerable<AudioClip> GetSounds();
    }
}
