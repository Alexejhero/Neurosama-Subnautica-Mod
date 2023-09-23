using System.Collections.Generic;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Unity.Sounds;

public partial class SoundCollection : ISoundProvider
{
    public IEnumerable<AudioClip> GetSounds() => sounds;
}
