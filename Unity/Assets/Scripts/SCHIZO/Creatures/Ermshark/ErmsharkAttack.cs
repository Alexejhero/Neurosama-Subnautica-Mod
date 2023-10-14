using NaughtyAttributes;
using SCHIZO.Unity.Sounds;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Creatures.Ermshark
{
    public sealed partial class ErmsharkAttack
    {
        [BoxGroup("Sounds"), SerializeField] private BaseSoundCollection attackSounds;
        [BoxGroup("Sounds"), SerializeField, Dropdown(nameof(bus_Dropdown))] private string bus;

        private DropdownList<string> bus_Dropdown = NAUGHTYATTRIBUTES.BusPathDropdown;
    }
}
