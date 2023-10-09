using NaughtyAttributes;
using SCHIZO.Unity.Retargeting.BelowZero;
using SCHIZO.Unity.Retargeting.Subnautica;
using SCHIZO.Unity.Sounds;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Item Data")]
    public sealed class ItemData : ScriptableObject
    {
        [BoxGroup("TechType")] public string classId;
        [BoxGroup("TechType")] public string displayName;
        [BoxGroup("TechType")] [ResizableTextArea] public string tooltip;

        [BoxGroup("Additional Properties")] public Sprite icon;
        [BoxGroup("Additional Properties"), HideIf(nameof(isBuildable))] public Vector2Int itemSize = new Vector2Int(1, 1);
        [BoxGroup("Additional Properties")] public bool isBuildable = false;

        [BoxGroup("Subnautica Data"), Label("Tech Group"), SerializeField] private TechGroup_SN techGroupSN = TechGroup_SN.Uncategorized;
        [BoxGroup("Subnautica Data"), Label("Tech Category"), SerializeField, HideIf(nameof(techCategorySN_HideIf))] private TechCategory_SN techCategorySN;
        private bool techCategorySN_HideIf() => techGroupSN == TechGroup_SN.Uncategorized;

        [BoxGroup("Below Zero Data"), Label("Tech Group"), SerializeField] private TechGroup_BZ techGroupBZ = TechGroup_BZ.Uncategorized;
        [BoxGroup("Below Zero Data"), Label("Tech Category"), SerializeField, HideIf(nameof(techCategoryBZ_HideIf))] private TechCategory_BZ techCategoryBZ;
        private bool techCategoryBZ_HideIf() => techGroupBZ == TechGroup_BZ.Uncategorized;

        public GameObject prefab;
        public BaseSoundCollection sounds;

#if !UNITY
        public TechGroup TechGroup => SCHIZO.Retargeting.TechGroup.Pick(techGroupSN, techGroupBZ);
        public TechCategory TechCategory => SCHIZO.Retargeting.TechCategory.Pick(techCategorySN, techCategoryBZ);
#endif
    }
}
