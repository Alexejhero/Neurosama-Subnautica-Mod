using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Items.Data
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Known Tech Info")]
    public partial class KnownTechInfo : ScriptableObject
    {
        [SerializeField]
        private bool hasCustomUnlockMessage;

        [SerializeField, Label("Unlock Message"), HideIf(nameof(hasCustomUnlockMessage)), Dropdown(nameof(defaultUnlockMessages)), UsedImplicitly]
        private string defaultUnlockMessage;

        [SerializeField, Label("Unlock Message"), ShowIf(nameof(hasCustomUnlockMessage)), UsedImplicitly]
        private string customUnlockMessage;

        [SerializeField, UsedImplicitly, Dropdown(nameof(unlockSounds))]
        private string unlockSound;

        [SerializeField, Required]
        public Sprite unlockSprite;

        [StaticHelpers.Cache]
        private static DropdownList<string> defaultUnlockMessages = new DropdownList<string>
        {
            {"Blueprint Scan", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:BlueprintUnlockMessage"},
            {"Blueprint Pickup", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:BlueprintPickupMessage"},
            {"Creature Discovered", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:NewCreatureDiscoveredMessage"},
        };

        [StaticHelpers.Cache]
        private static DropdownList<string> unlockSounds = new DropdownList<string>
        {
            {"Basic", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:BasicUnlockSound"},
            {"Blueprint", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:BlueprintUnlockSound"},
            {"Creature Discovered", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:NewCreatureDiscoveredSound"},
        };
    }
}
