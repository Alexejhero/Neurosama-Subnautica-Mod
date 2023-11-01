using System;
using SCHIZO.Helpers;

namespace SCHIZO.Sounds.Players;

partial class SoundPlayer
{
    protected virtual void Awake()
    {
        string busOrFieldPath = string.IsNullOrEmpty(bus) ? DefaultBus : bus;
        if (string.IsNullOrEmpty(busOrFieldPath)) throw new InvalidOperationException($"No bus assigned to {this}");
        // the string is most likely a field path, but we can also directly accept bus paths (and guids)
        string actualBus = busOrFieldPath.StartsWith("bus:/") || Guid.TryParse(busOrFieldPath, out _)
            ? busOrFieldPath
            : StaticHelpers.GetValue<string>(busOrFieldPath);

        soundCollection = soundCollection.Initialize(actualBus);
    }

    public void Play(float delay = 0)
    {
        if (Is3D) soundCollection.Play(emitter, delay);
        else soundCollection.Play2D(delay);
    }
}
