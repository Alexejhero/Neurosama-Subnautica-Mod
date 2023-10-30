using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO._old
{
    public abstract class BaseSoundCollection : ScriptableObject
    {
        public abstract IEnumerable<AudioClip> GetSounds();
    }
}
