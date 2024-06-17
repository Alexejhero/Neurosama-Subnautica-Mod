using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
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
    static SubtitlesHandler()
    {
        SaveUtils.RegisterOnStartLoadingEvent(() => CoroutineHost.StartCoroutine(RegisterWhenReady()));
    }

    public static void Register(SubtitlesData data)
    {
        Subtitles[data.key] = data;

        data.lines.ForEach(line => LanguageHandler.SetLanguageLine(line.key, line.text));
    }

    private static IEnumerator RegisterWhenReady()
    {
#if BELOWZERO
        while (!GameSubtitles._main)
            yield return null;
        foreach (SubtitlesData data in Subtitles.Values)
        {
            ActorTurns[data.key] = data.lines.Select(l => (Actor)l.actor).ToArray();
            GameSubtitles.main.subtitles[data.key] = ActorTurns[data.key];

            data.lines.ForEach(line => GameSubtitles.main.sounds[line.key] = line.ToSubEntry());
        }
#else
        yield break;
#endif
    }

    // TODO fix in nautilus
    public static void RegisterMetadata(SubtitlesData data, string fullText)
    {
        Language.main.metadata[data.key] = new(fullText,
            data.lines.ConvertAll(line => new Language.LineData(line.text, null, null))
        );
    }
}
