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

    private List<EventInstance> _playingEvents = [];
    private float _eventCleanupInterval = 1f;
    private float _nextUpdate;
    private List<Coroutine> _runningCoroutines;

    protected virtual void OnDestroy()
    {
        CancelAllDelayed();
        Stop();
    }
    protected virtual void FixedUpdate()
    {
        if (Time.time < _nextUpdate) return;
        _nextUpdate = Time.time + _eventCleanupInterval;
        foreach (EventInstance evt in _playingEvents)
        {
            if (evt.getPlaybackState(out PLAYBACK_STATE state) != FMOD.RESULT.OK
                || state == PLAYBACK_STATE.STOPPED)
            {
                evt.release();
            }
        }
        _playingEvents.RemoveAll(evt => !evt.isValid());
    }

    /// <summary>
    /// Play the <see cref="soundEvent">soundEvent</see>, attached to the <see cref="SoundPlayer"/>.
    /// </summary>
    /// <param name="delay">Delay (in seconds) before the sound will be played.</param>
    public void Play(float delay = 0)
    {
        if (Assets.Mod_Options_DisableAllSounds.Value) return;
        if (delay <= 0)
        {
            PlayAttached();
            return;
        }
        StartSoundCoroutine(PlayWithDelay(delay));
        return;

        IEnumerator PlayWithDelay(float del)
        {
            yield return new WaitForSeconds(del);
            PlayAttached();
        }
    }
    /// <summary>
    /// Play the <see cref="soundEvent">soundEvent</see>, detached from the <see cref="SoundPlayer"/>.
    /// </summary>
    /// <remarks>
    /// Since the event instance is started then detached, the sound player does not own it.<br/>
    /// This means that e.g. it will not stop when <see cref="Stop"/> is called, even if it's called on the <see cref="SoundPlayer"/> that started playing the event.
    /// </remarks>
    /// <param name="delay">Delay (in seconds) before the sound will be played.</param>
    public void PlayOneShot(float delay = 0)
    {
        if (Assets.Mod_Options_DisableAllSounds.Value) return;
        if (delay <= 0)
        {
            PlayDetached();
            return;
        }
        StartSoundCoroutine(PlayWithDelay(delay));
        return;

        IEnumerator PlayWithDelay(float del)
        {
            yield return new WaitForSeconds(del);
            PlayDetached();
        }
    }

    private void StartSoundCoroutine(IEnumerator coroutine)
    {
        // only matters for items given via the 'item' console command
        // since the object is spawned disabled (so no Awake)
        _runningCoroutines ??= [];
        // not started on the object because it might be disabled (inventory sounds)
        _runningCoroutines.Add(CoroutineHost.StartCoroutine(coroutine));
    }

    public void CancelAllDelayed()
    {
        _runningCoroutines?.ForEach(CoroutineHost.StopCoroutine);
        _runningCoroutines?.Clear();
    }

    private void PlayAttached()
    {
        LastPlay = Time.time;

        EventInstance evt;
        if (emitter)
        {
            emitter.PlayPath(soundEvent, Is3D);
            evt = emitter.evt;
        }
        else
        {
            evt = Is3D
                ? FMODHelpers.PlayPath2D(soundEvent)
                : FMODHelpers.PlayPath3DAttached(soundEvent, transform);
            _playingEvents.Add(evt);
        }
        onPlay.Invoke(evt);
    }

    private void PlayDetached()
    {
        LastPlay = Time.time;

        EventInstance evt = Is3D
            ? FMODHelpers.PlayOneShot(soundEvent, transform.position)
            : FMODHelpers.PlayOneShotAttached(soundEvent, Player.main.gameObject);
        onPlay.Invoke(evt);
    }

    public void Stop()
    {
        if (emitter) emitter.Stop();
        _playingEvents.ForEach(FMODHelpers.StopAndRelease);
        _playingEvents.Clear();
    }
}
