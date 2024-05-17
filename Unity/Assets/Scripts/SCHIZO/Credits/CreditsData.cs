using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Credits
{
    [CreateAssetMenu(menuName = "SCHIZO/Credits/Credits List")]
    public sealed class CreditsData : ScriptableObject
    {
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<CreditsEntry> mainCredits = new();

        [TextArea(1, 10)]
        public string extraCredits;

        #region Nested types

        [AttributeUsage(AttributeTargets.Field)]
        public sealed class CreditAttribute(string sn, string bz) : Attribute
        {
            public readonly string sn = sn;
            public readonly string bz = bz;

            public CreditAttribute(string both) : this(both, both) {}
        }

        [Serializable]
        public sealed class CreditsEntry
        {
            [Space]
            public string name;
            public CreditsType credits;
        }

        [Flags]
        public enum CreditsType
        {
            [Credit("Programming", "Developers")] Programming = 1 << 0,
            [Credit("Contributor", "Contributors")] Contributor = 1 << 1,
            [Credit("3D Modeling", "3D Modelers")] Modeling = 1 << 2,
            [Credit("Animations", "Animators")] Animations = 1 << 3,
            [Credit("2D Art", "2D Artists")] Artist = 1 << 4,
            [Credit("Sounds", "Audio")] Sounds = 1 << 5,
            [Credit("Lore", "Writing & Lore")] Lore = 1 << 6,
            [Credit("Texturing")] Texturing = 1 << 7,
            [Credit("Project Lead")] ProjectLead = 1 << 8,
        }

        #endregion
    }

    public static class CreditsTypeExtensions
    {
        private static readonly Dictionary<CreditsData.CreditsType, CreditsData.CreditAttribute> _cache = new();

        public static IEnumerable<CreditsData.CreditsType> ToList(this CreditsData.CreditsType type)
        {
            return Enum.GetValues(typeof(CreditsData.CreditsType)).Cast<CreditsData.CreditsType>().Where(test => type.HasFlag(test));
        }

        public static string GetSN(this CreditsData.CreditsType type)
        {
            return GetAttribute(type).sn;
        }

        public static string GetBZ(this CreditsData.CreditsType type)
        {
            return GetAttribute(type).bz;
        }

        private static CreditsData.CreditAttribute GetAttribute(CreditsData.CreditsType type)
        {
            return _cache.TryGetValue(type, out CreditsData.CreditAttribute attribute)
                ? attribute
                : _cache[type] = typeof(CreditsData.CreditsType).GetField(type.ToString()).GetCustomAttribute<CreditsData.CreditAttribute>();
        }
    }
}
