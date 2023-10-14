using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Sounds
{
    public class WorldAmbientSoundPlayer : MonoBehaviour
    {
        [Required] public SoundCollection soundCollection;
        [Dropdown(nameof(bus_Dropdown))] public string bus;
        [Required, ValidateInput(nameof(Validate_emitter), "Emitter must be of type FMOD_CustomEmitter")] public Object emitter;
        [ValidateInput(nameof(Validate_pickupable), "Pickupable must be null or a Pickupable component")] public Object pickupable;

#if !UNITY
        private SCHIZO.Sounds.FMODSoundCollection _fmodSoundCollection;
        private float _timer = -1;

        private void Awake()
        {
            string[] splits = bus.Split(':');
            string busPath = (string) HarmonyLib.AccessTools.Field(HarmonyLib.AccessTools.TypeByName(splits[0]), splits[1]).GetRawConstantValue();

            _fmodSoundCollection = new SCHIZO.Sounds.FMODSoundCollection(soundCollection, busPath);
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

        private bool Validate_emitter(Object obj) => !obj || obj.GetType().Name == "FMOD_CustomEmitter";
        private bool Validate_pickupable(Object obj) => !obj || obj.GetType().Name == "Pickupable";

        private DropdownList<string> bus_Dropdown = new DropdownList<string>()
        {
            {"AudioUtils.BusPaths.PDAVoice", "Nautilus.Utility.AudioUtils+BusPaths:PDAVoice"},
            {"AudioUtils.BusPaths.UnderwaterCreatures", "Nautilus.Utility.AudioUtils+BusPaths:UnderwaterCreatures"}
        };

        #endregion
    }
}
