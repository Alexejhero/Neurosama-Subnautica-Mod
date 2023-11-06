using SCHIZO.Interop.Subnautica;

namespace SCHIZO.Sounds.Collections
{
    public sealed partial class SoundCollectionInstance : _FMODAsset
    {
        [ReadOnly] public SoundCollection collection;
        public BusPaths bus;
    }
}
