using NaughtyAttributes;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Sound Collection")]
    public sealed class SoundCollection : BaseSoundCollection
    {
        [ReorderableList] public List<AudioClip> sounds;

        public override IEnumerable<AudioClip> GetSounds() => sounds;
    }
}
