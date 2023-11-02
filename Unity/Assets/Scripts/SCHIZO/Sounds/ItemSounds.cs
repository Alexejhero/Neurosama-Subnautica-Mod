using SCHIZO.Interop.NaughtyAttributes;
using SCHIZO.Sounds.Collections;
using UnityEngine;

namespace SCHIZO.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Item Sounds")]
    public sealed partial class ItemSounds : NaughtyScriptableObject
    {
        public SoundCollection pickupSounds;
        public SoundCollection dropSounds;

        [Space]

        public SoundCollection drawSounds;
        public SoundCollection holsterSounds;

        [Space]

        public SoundCollection cookSounds;
        public SoundCollection eatSounds;

        [Space]

        public SoundCollection playerDeathSounds;
    }
}
