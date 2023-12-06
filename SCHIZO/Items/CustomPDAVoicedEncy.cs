using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Helpers;

namespace SCHIZO.Items;

[HarmonyPatch]
partial class CustomPDAVoicedEncy
{
    protected override void Register()
    {
        string path = RetargetHelpers.Pick(encyData.encyPathSN, encyData.encyPathBZ);
        PDAHandler.AddEncyclopediaEntry(key, path,
            encyData.title, encyData.description.text,
            encyData.texture, encyData.unlockSprite,
            voiceLog: AudioUtils.GetFmodAsset(encyData.logVO));
        Subtitles.SubtitlesHandler.RegisterMetadata(subtitles, encyData.description.text);
    }
}
