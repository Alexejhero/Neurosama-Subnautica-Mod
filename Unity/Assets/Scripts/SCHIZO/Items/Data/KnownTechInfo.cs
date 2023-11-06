using JetBrains.Annotations;
using SCHIZO.Helpers;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.Data
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Known Tech Info")]
    public partial class KnownTechInfo : ScriptableObject
    {
        [SerializeField]
        private bool hasCustomUnlockMessage;

        [SerializeField, LabelText("Unlock Message"), HideIf(nameof(hasCustomUnlockMessage)), Dropdown(nameof(DefaultUnlockMessages)), UsedImplicitly]
        private string defaultUnlockMessage;

        [SerializeField, LabelText("Unlock Message"), ShowIf(nameof(hasCustomUnlockMessage)), UsedImplicitly]
        private string customUnlockMessage;

        [SerializeField, UsedImplicitly, Dropdown(nameof(UnlockSounds))]
        private string unlockSound;

        [SerializeField, Required]
        public Sprite unlockSprite;

        [StaticHelpers.Cache]
        private static TriDropdownList<string> DefaultUnlockMessages()
        {
            return new TriDropdownList<string>()
            {
                {"Blueprint Scan", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:BlueprintUnlockMessage"},
                {"Blueprint Pickup", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:BlueprintPickupMessage"},
                {"Creature Discovered", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:NewCreatureDiscoveredMessage"},
            };
        }

        [StaticHelpers.Cache]
        private static TriDropdownList<string> UnlockSounds()
        {
            return new TriDropdownList<string>()
            {
                {"Basic", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:BasicUnlockSound"},
                {"Blueprint", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:BlueprintUnlockSound"},
                {"Creature Discovered", "Nautilus.Handlers.KnownTechHandler+DefaultUnlockData:NewCreatureDiscoveredSound"},
            };
        }
    }
}
