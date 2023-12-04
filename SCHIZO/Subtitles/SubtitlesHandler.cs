using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nautilus.Handlers;
using UWE;
using GameSubtitles = Subtitles;

namespace SCHIZO.Subtitles;

[HarmonyPatch]
internal static class SubtitlesHandler
{
    // here for debuggability
    public static readonly Dictionary<string, SubtitlesData> Subtitles = [];
#if BELOWZERO
    // runtime data
    public static readonly Dictionary<string, Actor[]> ActorTurns = [];
#endif

    public static void Register(SubtitlesData data)
    {
        Subtitles[data.key] = data;

        for (int i = 0; i < data.lines.Count; i++)
        {
            SubtitlesData.SubtitleLine line = data.lines[i];

            LanguageHandler.SetLanguageLine(line.key, line.text);
        }
        CoroutineHost.StartCoroutine(RegisterWhenReady(data));
    }

    private static IEnumerator RegisterWhenReady(SubtitlesData data)
    {
#if BELOWZERO
        while (!GameSubtitles._main)
            yield return null;
        GameSubtitles.main.subtitles[data.key] = ActorTurns[data.key];
        foreach (SubtitlesData.SubtitleLine line in data.lines)
        {
            GameSubtitles.main.sounds[line.key] = line.ToSubEntry();
        }
        ActorTurns[data.key] = data.lines.Select(l => (Actor)l.actor).ToArray();
#else
        yield break;
#endif
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
