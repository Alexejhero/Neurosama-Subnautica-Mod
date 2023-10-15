using NaughtyAttributes;

namespace SCHIZO.Utilities
{
    internal static class NAUGHTYATTRIBUTES
    {
        public static DropdownList<string> BusPathDropdown = new DropdownList<string>()
        {
            {"AudioUtils.BusPaths.PDAVoice", "Nautilus.Utility.AudioUtils+BusPaths:PDAVoice"},
            {"AudioUtils.BusPaths.UnderwaterCreatures", "Nautilus.Utility.AudioUtils+BusPaths:UnderwaterCreatures"}
        };
    }
}
