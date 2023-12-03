using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Nautilus.Handlers;
using UWE;
using GameSubtitles = Subtitles;
using GameSubtitlesData = SubtitlesData;

namespace SCHIZO.Subtitles;

[HarmonyPatch]
internal static class SubtitlesHandler
{
    // compile-time data
    // dict key is the subtitle key
    public static Dictionary<string, SubtitlesData> Subtitles = [];
    // dict key is the line key
    public static Dictionary<string, SubtitlesData.SubtitleLine> Lines = [];

    // runtime data
    public static Dictionary<string, GameSubtitlesData.Entry> Sounds = [];
    public static Dictionary<string, Actor[]> ActorTurns = [];

    public static void Register(SubtitlesData data)
    {
        Subtitles[data.key] = data;
        List<Actor> actors = [];
        for (int i = 0; i < data.lines.Count; i++)
        {
            SubtitlesData.SubtitleLine line = data.lines[i];

            Lines[line.key] = line;
            ActorTurns[line.key] = [line.actor]; // also register individual lines for some reason?
            actors.Add(line.actor);
            LanguageHandler.SetLanguageLine(line.key, line.text);
            CoroutineHost.StartCoroutine(RegisterWhenReady(line.key));
        }
        ActorTurns[data.key] = [.. actors];
    }

    private static IEnumerator RegisterWhenReady(string key)
    {
        while (!GameSubtitles._main)
            yield return null;
        GameSubtitles.main.subtitles[key] = ActorTurns[key];
    }

    private static void QueueRegister(string soundId, SubtitlesData.SubtitleLine line)
    {
        Sounds[soundId] = line.ToSubEntry();
    }
}
