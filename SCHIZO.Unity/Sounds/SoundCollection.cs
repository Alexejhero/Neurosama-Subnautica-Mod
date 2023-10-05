using System.Collections.Generic;

namespace SCHIZO.Unity.Sounds;

[CreateAssetMenu(menuName = "SCHIZO/Sounds/Sound Collection")]
public sealed class SoundCollection : BaseSoundCollection
{
    [ReorderableList] public List<AudioClip> sounds;

    public override IEnumerable<AudioClip> GetSounds() => sounds;
}
