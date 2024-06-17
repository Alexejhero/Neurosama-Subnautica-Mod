using System.Collections.Generic;
using JetBrains.Annotations;
using SCHIZO.Items.Data;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.ErmBed
{
    [UsedImplicitly]
    public sealed partial class ErmBedLoader : ItemLoader
    {
        protected override HashSet<string> AllowedClassIds => ["ermbed"];
        [Required]
        public Texture2D blanketTexture;
    }
}
