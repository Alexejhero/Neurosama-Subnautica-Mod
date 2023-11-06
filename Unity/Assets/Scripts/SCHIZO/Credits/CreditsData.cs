using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Credits
{
    [CreateAssetMenu(menuName = "SCHIZO/Credits/Credits List")]
    public sealed class CreditsData : ScriptableObject
    {
        [SerializeField, ListDrawerSettings(AlwaysExpanded = true)]
        private List<CreditsEntry> mainCredits = new();

        [SerializeField, TextArea(1, 10)]
        private string extraCredits;

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
            [Credit("Sounds", "Audio Compilation & Cleaning")] Sounds = 1 << 5,
            [Credit("Lore", "Writing & Lore")] Lore = 1 << 6,
            [Credit("Texturing")] Texturing = 1 << 7,
            [Credit("Project Lead")] ProjectLead = 1 << 8,
        }

        #endregion
    }
}
