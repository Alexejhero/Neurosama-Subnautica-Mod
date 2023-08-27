using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using FMOD;
using Nautilus.Handlers;
using UnityEngine;

namespace SCHIZO
{
    public sealed class LocalSoundCollection
    {
        private readonly List<string> _remainingSounds = new List<string>();
        private readonly List<string> _playedSounds = new List<string>();

        public float lastPlay { get; private set; } = -1;

        private readonly List<Coroutine> _runningCoroutines = new List<Coroutine>();

        public LocalSoundCollection(string dirpath, string bus)
        {
            foreach (string soundFile in Directory.GetFiles(dirpath))
            {
                string id = Guid.NewGuid().ToString();
                Sound s = CustomSoundHandler.RegisterCustomSound(id, soundFile, bus, MODE._3D);
                s.set3DMinMaxDistance(1, 30000);
                _remainingSounds.Add(id);
            }

            _remainingSounds.Shuffle();
        }

        public void Play(FMOD_CustomEmitter emitter, float delay = 0)
        {
            if (!emitter) return;
            if (SchizoPlugin.config.DisableAllNoises) return;

            if (delay == 0)
            {
                PlaySound(emitter);
                return;
            }

            IEnumerator PlayWithDelay(float del)
            {
                yield return new WaitForSeconds(del);
                PlaySound(emitter);
            }

            Coroutine c = GameInput.instance.StartCoroutine(PlayWithDelay(delay));
            _runningCoroutines.Add(c);
        }

        public void CancelAllDelayed()
        {
            foreach (Coroutine c in _runningCoroutines)
            {
                GameInput.instance.StopCoroutine(c);
            }

            _runningCoroutines.Clear();
        }

        private void PlaySound(FMOD_CustomEmitter emitter)
        {
            lastPlay = Time.time;

            if (_remainingSounds.Count == 0)
            {
                _remainingSounds.AddRange(_playedSounds);
                _playedSounds.Clear();
            }

            var asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = _remainingSounds[0];
            emitter.SetAsset(asset);
            emitter.Play();

            _playedSounds.Add(_remainingSounds[0]);
            _remainingSounds.RemoveAt(0);
        }
    }
}
