using FMODUnity;
using UnityEngine;

namespace SCHIZO.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Item Sounds")]
    public sealed partial class ItemSounds : ScriptableObject
    {
        [EventRef] public string pickupSounds;
        [EventRef] public string dropSounds;

        [Space]

        [EventRef] public string drawSounds;
        [EventRef] public string holsterSounds;

        [Space]

        [EventRef] public string cookSounds;
        [EventRef] public string eatSounds;

        [Space]

        [EventRef] public string playerDeathSounds;
    }
}
