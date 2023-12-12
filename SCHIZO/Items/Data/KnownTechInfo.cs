using Nautilus.Handlers;
using SCHIZO.Helpers;

namespace SCHIZO.Items.Data;

partial class KnownTechInfo
{
    public string UnlockMessage => hasCustomUnlockMessage ? customUnlockMessage : StaticHelpers.GetValue<string>(defaultUnlockMessage);
    public FMODAsset UnlockSound => StaticHelpers.GetValue<FMODAsset>(unlockSound);

    public void Register(UnityPrefab prefab)
    {
        KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech()
        {
            techType = prefab.ModItem,
            unlockTechTypes = [],
            unlockMessage = UnlockMessage,
            unlockSound = UnlockSound,
            unlockPopup = unlockSprite
        });
    }
}
