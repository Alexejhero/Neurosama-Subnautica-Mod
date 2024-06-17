using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.Items.FumoItem
{
    public sealed partial class FumoItemLoader : ItemLoader
    {
        protected override HashSet<string> AllowedClassIds => ["neurofumoitem"];
        public string spawnerClassId;
        [SerializeField]
        private GameObject spawnerPrefab;
    }
}
