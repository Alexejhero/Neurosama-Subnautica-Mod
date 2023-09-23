using System.Collections.Generic;
using System.Linq;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Unity.Sounds;

public partial class CombinedSoundCollection : ISoundProvider
{
    public IEnumerable<AudioClip> GetSounds() => combineWith.SelectMany(s => s.GetSounds());
}
