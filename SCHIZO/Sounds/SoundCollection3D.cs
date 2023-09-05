using System.Collections;
using FMOD;
using FMODUnity;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.Sounds;

public sealed class SoundCollection3D : SoundCollection
{
    private SoundCollection3D() {}

    public static SoundCollection3D Create(string path, string bus)
    {
        SoundCollection3D result = CreateInstance<SoundCollection3D>();
        result.Initialize(path, bus);
        return result;
    }

    protected override void RegisterSound(string id, string soundFile, string bus)
    {
        Sound s = CustomSoundHandler.RegisterCustomSound(id, soundFile, bus, AudioUtils.StandardSoundModes_3D);
        RuntimeManager.GetBus(bus).unlockChannelGroup();
        s.set3DMinMaxDistance(1, 30);
    }

    public void Play(FMOD_CustomEmitter emitter, float delay = 0)
    {
        if (CONFIG.DisableAllNoises) return;
        if (!emitter) return;

        if (delay == 0)
        {
            PlaySound(emitter);
            return;
        }

        StartSoundCoroutine(PlayWithDelay(delay));
        return;

        IEnumerator PlayWithDelay(float del)
        {
            yield return new WaitForSeconds(del);
            PlaySound(emitter);
        }
    }

    private void PlaySound(FMOD_CustomEmitter emitter)
    {
        LastPlay = Time.time;

        FMODAsset asset = CreateInstance<FMODAsset>();
        asset.path = _sounds.GetRandom();
        emitter.SetAsset(asset);
        emitter.Play();
    }
}
