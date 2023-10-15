using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.Sounds
{
    public abstract class BaseSoundCollection : ScriptableObject
    {
        public abstract IEnumerable<AudioClip> GetSounds();
    }
}
