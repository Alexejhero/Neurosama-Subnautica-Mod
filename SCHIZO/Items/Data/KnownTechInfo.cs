using SCHIZO.Helpers;

namespace SCHIZO.Items.Data;

partial class KnownTechInfo
{
    public string UnlockMessage => hasCustomUnlockMessage ? customUnlockMessage : StaticHelpers.GetValue<string>(defaultUnlockMessage);
    public FMODAsset UnlockSound => StaticHelpers.GetValue<FMODAsset>(unlockSound);
}
