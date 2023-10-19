using SCHIZO.Helpers;

namespace SCHIZO.Items.Data;

partial class KnownTechInfo
{
    public string UnlockMessage => hasCustomUnlockMessage ? customUnlockMessage : ReflectionHelpers.GetFieldValue<string>(defaultUnlockMessage);
    public FMODAsset UnlockSound => ReflectionHelpers.GetFieldValue<FMODAsset>(unlockSound);
}
