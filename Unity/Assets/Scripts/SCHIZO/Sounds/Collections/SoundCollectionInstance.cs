using SCHIZO.Interop.Subnautica;
using ReadOnlyAttr = TriInspector.ReadOnlyAttribute;

namespace SCHIZO.Sounds.Collections
{
    public sealed partial class SoundCollectionInstance : _FMODAsset
    {
        [ReadOnlyAttr] public SoundCollection collection;
        public BusPaths bus;
    }
}
