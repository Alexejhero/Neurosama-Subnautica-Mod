using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.Sounds;

public interface ISoundProvider
{
    IEnumerable<AudioClip> GetSounds();
}
