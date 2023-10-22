using SCHIZO.Helpers;

namespace SCHIZO.Items.Data;

partial class KnownTechInfo
{
    public string UnlockMessage => hasCustomUnlockMessage ? customUnlockMessage : ReflectionHelpers.GetStaticValue<string>(defaultUnlockMessage);
    public FMODAsset UnlockSound => ReflectionHelpers.GetStaticValue<FMODAsset>(unlockSound);
}
