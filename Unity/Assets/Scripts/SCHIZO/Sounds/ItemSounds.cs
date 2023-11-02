using SCHIZO.Interop.NaughtyAttributes;
using SCHIZO.Sounds.Collections;
using UnityEngine;

namespace SCHIZO.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Item Sounds")]
    public sealed partial class ItemSounds : NaughtyScriptableObject
    {
        public SoundCollectionInstance pickupSounds;
        public SoundCollectionInstance dropSounds;

        [Space]

        public SoundCollectionInstance drawSounds;
        public SoundCollectionInstance holsterSounds;

        [Space]

        public SoundCollectionInstance cookSounds;
        public SoundCollectionInstance eatSounds;

        [Space]

        public SoundCollectionInstance playerDeathSounds;
    }
}
