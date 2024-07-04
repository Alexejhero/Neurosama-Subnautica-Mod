using Nautilus.Handlers;
using SCHIZO.Helpers;
using SCHIZO.Sounds;

namespace SCHIZO.Items.Data;

partial class PDAEncyclopediaInfo
{
    public void Register(UnityPrefab prefab)
    {
        Register(prefab.Info.ClassID);

        if (!scannable) return;
        PDAHandler.AddCustomScannerEntry(prefab.ModItem, scanTime, false, prefab.Info.ClassID);
        if (!string.IsNullOrEmpty(scanSounds)) ScanSoundHandler.Register(prefab.ModItem, scanSounds);
    }

    // some entries don't unlock from scanning
    // like voiced journals (picking up PDAs) or rooms/equipment (unlocking associated blueprints)
    public void Register(string key)
    {
        string path = RetargetHelpers.Pick(encyPathSN, encyPathBZ);
        FMODAsset vo = FMODHelpers.GetFmodAsset(logVO);
        string desc = description ? description.text : null;
        PDAHandler.AddEncyclopediaEntry(key, path, title, desc,
            image: texture, popupImage: unlockSprite,
            unlockSound: isImportantUnlock ? PDAHandler.UnlockImportant : PDAHandler.UnlockBasic,
            voiceLog: vo);
    }
}
