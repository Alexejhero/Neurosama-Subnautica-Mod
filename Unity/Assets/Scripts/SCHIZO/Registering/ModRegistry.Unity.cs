using System;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;

namespace SCHIZO.Registering
{
    partial class ModRegistry
    {
        private static ModRegistry _instance;

        public static ModRegistry Instance
        {
            get
            {
                if (_instance) return _instance;

#if UNITY_EDITOR
                string registryGUID = AssetDatabase.FindAssets($"t:{nameof(ModRegistry)}").Single();
                _instance = AssetDatabase.LoadAssetAtPath<ModRegistry>(AssetDatabase.GUIDToAssetPath(registryGUID));
                return _instance;
#else
                throw new InvalidOperationException("Cannot get ModRegistry instance outside of Editor context");
#endif
            }
        }

        [Button]
        public void Sort()
        {
            registryItems.RemoveAll(i => !i);
            registryItems = registryItems.Distinct().ToList();
            registryItems.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
        }
    }
}
