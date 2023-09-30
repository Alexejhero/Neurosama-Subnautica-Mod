

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Sounds
{
    public abstract class BaseSoundCollection : ScriptableObject
    {
        public abstract IEnumerable<AudioClip> GetSounds();
    }
}
