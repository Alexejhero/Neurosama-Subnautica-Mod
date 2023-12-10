using HarmonyLib;

namespace SCHIZO.Items;

[HarmonyPatch]
partial class CustomPDAVoicedEncy
{
    protected override void Register()
    {
        encyData.Register(key);
        Subtitles.SubtitlesHandler.RegisterMetadata(subtitles, encyData.description.text);
    }
}
