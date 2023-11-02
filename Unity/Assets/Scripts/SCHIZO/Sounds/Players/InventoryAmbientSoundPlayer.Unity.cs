using SCHIZO.Options.Float;
using UnityEngine;

namespace SCHIZO.Sounds.Players
{
    partial class InventoryAmbientSoundPlayer
    {
        [SerializeField, HideInInspector] private SliderModOption minDelayDefault;
        [SerializeField, HideInInspector] private SliderModOption maxDelayDefault;

        private void Reset()
        {
            MinDelay = new ConfigurableValueFloat
            {
                modOption = minDelayDefault
            };

            MaxDelay = new ConfigurableValueFloat
            {
                modOption = maxDelayDefault
            };
        }
    }
}
