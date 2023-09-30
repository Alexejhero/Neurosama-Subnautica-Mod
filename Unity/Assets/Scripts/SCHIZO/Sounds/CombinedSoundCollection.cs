using NaughtyAttributes;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Combined Sound Collection")]
    public sealed class CombinedSoundCollection : BaseSoundCollection
    {
        [SerializeField, ReorderableList] private List<SoundCollection> combineWith;

        public override IEnumerable<AudioClip> GetSounds() => combineWith.SelectMany(s => s.GetSounds());
    }
}
