using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public static readonly Dictionary<string, SubtitlesData> Subtitles = [];
    // runtime data
    public static readonly Dictionary<string, Actor[]> ActorTurns = [];

    public static void Register(SubtitlesData data)
    {
        Subtitles[data.key] = data;
        List<Actor> actors = [];

        for (int i = 0; i < data.lines.Count; i++)
        {
            SubtitlesData.SubtitleLine line = data.lines[i];

            actors.Add(line.actor);
            LanguageHandler.SetLanguageLine(line.key, line.text);
        }
        ActorTurns[data.key] = [.. actors];
        CoroutineHost.StartCoroutine(RegisterWhenReady(data));
    }

    private static IEnumerator RegisterWhenReady(SubtitlesData data)
    {
        while (!GameSubtitles._main)
            yield return null;
        GameSubtitles.main.subtitles[data.key] = ActorTurns[data.key];
        foreach (SubtitlesData.SubtitleLine line in data.lines)
        {
            GameSubtitles.main.sounds[line.key] = line.ToSubEntry();
        }
    }

    // TODO fix in nautilus
    public static void RegisterMetadata(SubtitlesData data, string fullText)
    {
        Language.MetaData lang = new(fullText,
            data.lines
                .Select(line => new Language.LineData(line.text, null, null))
                .ToList()
        );
        Language.main.metadata[data.key] = lang;
    }
}
