using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using UnityEngine;
using UWE;

namespace SCHIZO.Sounds.Players;

partial class SoundPlayer
{
    public float LastPlay { get; private set; } = -1;

    private List<Coroutine> _runningCoroutines;
    private void StartSoundCoroutine(IEnumerator coroutine)
    {
        _runningCoroutines.Add(CoroutineHost.StartCoroutine(coroutine));
    }
    public void Play(float delay = 0)
    {
        if (Assets.Mod_Options_DisableAllSounds.Value) return;
        if (delay <= 0)
        {
            PlaySound();
            return;
        }
        StartSoundCoroutine(PlayWithDelay(delay));
        return;

        IEnumerator PlayWithDelay(float del)
        {
            yield return new WaitForSeconds(del);
            PlaySound();
        }
    }

    public void CancelAllDelayed()
    {
        _runningCoroutines.ForEach(CoroutineHost.StopCoroutine);
        _runningCoroutines.Clear();
    }

    private void PlaySound()
    {
        LastPlay = Time.time;

        if (Is3D) emitter.PlayPath(soundEvent);
        else
        {
            Stop();
            _evt = FMODHelpers.PlayPath2D(soundEvent);
        }
    }

    private EventInstance _evt;

    public void Stop()
    {
        if (Is3D) emitter.Stop();
        else
        {
            _evt.stop(STOP_MODE.ALLOWFADEOUT);
            _evt.release();
        }
    }
}
