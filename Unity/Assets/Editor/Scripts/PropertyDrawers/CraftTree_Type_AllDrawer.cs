using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.Registering;
using UnityEditor;

namespace PropertyDrawers
{
    [CustomPropertyDrawer(typeof(CraftTree_Type_All))]
    public sealed class CraftTree_Type_AllDrawer : GameSpecificEnumDrawer
    {
        private static readonly List<string> SubnauticaCraftTreeTypes = typeof(CraftTree_Type_All).GetEnumNames()
            .Where(n => typeof(CraftTree_Type_All).GetField(n).GetCustomAttribute<GameAttribute>().game.HasFlag(Game.Subnautica)).ToList();

        private static readonly List<string> BelowZeroCraftTreeTypes = typeof(CraftTree_Type_All).GetEnumNames()
            .Where(n => typeof(CraftTree_Type_All).GetField(n).GetCustomAttribute<GameAttribute>().game.HasFlag(Game.BelowZero)).ToList();

        protected override bool IsValueAcceptable(string entry, string propertyPath)
        {
            if (propertyPath.ToLower().Contains("sn")) return SubnauticaCraftTreeTypes.Contains(entry);
            if (propertyPath.ToLower().Contains("bz")) return BelowZeroCraftTreeTypes.Contains(entry);
            return SubnauticaCraftTreeTypes.Contains(entry) || BelowZeroCraftTreeTypes.Contains(entry);
        }
    }
}
