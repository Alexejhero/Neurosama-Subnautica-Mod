using SCHIZO.Interop.Subnautica;
using TriInspector;

namespace SCHIZO.Sounds.Collections
{
    public sealed partial class SoundCollectionInstance : _FMODAsset
    {
        [ReadOnly] public SoundCollection collection;
        public BusPaths bus;
    }
}
