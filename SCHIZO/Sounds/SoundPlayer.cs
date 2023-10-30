using System;
using SCHIZO.Helpers;

namespace SCHIZO.Sounds;

partial class SoundPlayer
{
    private FMODSoundCollection fmodSounds;

    protected void Awake()
    {
        string busOrFieldPath = string.IsNullOrEmpty(bus) ? DefaultBus : bus;
        if (string.IsNullOrEmpty(busOrFieldPath))
            throw new InvalidOperationException($"No bus assigned to {this}");
        // the string is most likely a field path, but we can also directly accept bus paths (and guids)
        string actualBus = busOrFieldPath.StartsWith("bus:/") || Guid.TryParse(busOrFieldPath, out _)
            ? busOrFieldPath
            : StaticHelpers.GetValue<string>(busOrFieldPath);
        fmodSounds = FMODSoundCollection.For(soundCollection, actualBus);
    }

    public void Play(float delay = 0)
    {
        if (Is3D) fmodSounds.Play((FMOD_CustomEmitter) emitter, delay);
        else fmodSounds.Play2D(delay);
    }
}
