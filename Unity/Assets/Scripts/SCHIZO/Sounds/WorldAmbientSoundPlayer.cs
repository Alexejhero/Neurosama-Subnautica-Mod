using NaughtyAttributes;
using SCHIZO.Unity.NaughtyExtensions;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Sounds
{
    public class WorldAmbientSoundPlayer : MonoBehaviour
    {
        [Required] public SoundCollection soundCollection;
        [Dropdown(nameof(bus_Dropdown))] public string bus;
        [Required, ValidateType("FMOD_CustomEmitter")] public MonoBehaviour emitter;
        [ValidateType("Pickupable")] public MonoBehaviour pickupable;

#if !UNITY
        private SCHIZO.Sounds.FMODSoundCollection _fmodSoundCollection;
        private float _timer = -1;

        private void Awake()
        {
            _fmodSoundCollection = new SCHIZO.Sounds.FMODSoundCollection(soundCollection, SCHIZO.Sounds.FMODSoundCollection.GetBusPath(bus));
            ResetTimer();
        }

        private void Update()
        {
            if (pickupable && Inventory.main.Contains((Pickupable) pickupable)) return;

            _timer -= Time.deltaTime;

            if (_timer < 0)
            {
                ResetTimer();
                _fmodSoundCollection.Play((FMOD_CustomEmitter) emitter);
            }
        }

        private void ResetTimer()
        {
            _timer = Random.Range(SCHIZO.Sounds.SoundConfig.Provider.MinWorldSoundDelay, SCHIZO.Sounds.SoundConfig.Provider.MaxWorldSoundDelay);
        }
#endif

        #region NaughtyAttributes stuff

        private DropdownList<string> bus_Dropdown = NAUGHTYATTRIBUTES.BusPathDropdown;

        #endregion
    }
}
