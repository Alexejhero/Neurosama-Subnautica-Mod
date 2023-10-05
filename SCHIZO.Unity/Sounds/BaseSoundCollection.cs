using System.Collections.Generic;

namespace SCHIZO.Unity.Sounds;

public abstract class BaseSoundCollection : ScriptableObject
{
    public abstract IEnumerable<AudioClip> GetSounds();
}
