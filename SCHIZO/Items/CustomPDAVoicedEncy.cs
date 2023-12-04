using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Helpers;
using UnityEngine;
using UWE;

namespace SCHIZO.Items;

[HarmonyPatch]
partial class CustomPDAVoicedEncy
{
    public static Dictionary<string, CustomPDAVoicedEncy> _customSounds = [];

    protected override void Register()
    {
        if (!RegisterSounds())
        {
            LOGGER.LogError($"Could not register voiced ency {key}!");
            return;
        }

        string path = RetargetHelpers.Pick(encyData.encyPathSN, encyData.encyPathBZ);
        PDAHandler.AddEncyclopediaEntry(key, path,
            encyData.title, encyData.description.text,
            encyData.texture, encyData.unlockSprite,
            voiceLog: AudioUtils.GetFmodAsset("event:/test/test_pda_log", "{204a5e50-855e-40d9-8fc3-782f31f7c489}"));
        _customSounds[key] = this;
    }

    private bool RegisterSounds()
    {
        List<AudioClip> clips = logVO.GetSounds().ToList();
        if (clips.Count != subtitles.lines.Count)
        {
            LOGGER.LogError($"Voiced ency {key} has {clips.Count} VO clips and {subtitles.lines.Count} subtitle lines! These counts must match exactly");
            return false;
        }
        foreach ((AudioClip clip, Subtitles.SubtitlesData.SubtitleLine line) in clips.Zip(subtitles.lines, (clip, line) => (clip, line)))
        {
            CustomSoundHandler.RegisterCustomSound(line.key, clip, AudioUtils.BusPaths.VoiceOvers);
        }
        CoroutineHost.StartCoroutine(RegisterSubLinesWhenReady());
        return true;
    }

    private IEnumerator RegisterSubLinesWhenReady()
    {
        while (!global::Subtitles.main)
            yield return null;

        foreach (Subtitles.SubtitlesData.SubtitleLine line in subtitles.lines)
        {
            global::Subtitles.main.sounds[line.key] = line.ToSubEntry();
        }
    }

    [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.PlayImpl))]
    [HarmonyPrefix, HarmonyBefore("com.snmodding.nautilus")]
    private static bool QueueSubSounds(SoundQueue __instance, string sound, SoundHost host, string subtitles, int subtitlesLine, int timelinePosition)
    {
        if (_customSounds.TryGetValue(sound, out var voicedEncy))
        {
            foreach (var line in voicedEncy.subtitles.lines)
            {
                __instance.queue.Add(new()
                {
                    sound = line.key,
                    host = host,
                    subtitles = line.key,
                    subtitleLine = line.index,
                });
            }
        }
        return true;
    }
}
