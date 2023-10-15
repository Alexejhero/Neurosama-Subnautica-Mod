using System;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures._old
{
    // [CreateAssetMenu(menuName = "SCHIZO/Creatures/Creature Sound Data")]
    [Obsolete]
    public sealed class CreatureSoundData : ScriptableObject
    {
        [Space]

        public BaseSoundCollection ambientItemSounds;
        public BaseSoundCollection ambientWorldSounds;

        [Space]

        public BaseSoundCollection pickupSounds;
        public BaseSoundCollection dropSounds;

        [Space]

        public BaseSoundCollection drawSounds;
        public BaseSoundCollection holsterSounds;

        [Space]

        public BaseSoundCollection cookSounds;
        public BaseSoundCollection eatSounds;

        [Space]

        public BaseSoundCollection hurtSounds;
        public BaseSoundCollection attackSounds;

        [Space]

        public BaseSoundCollection scanSounds;
    }
}
