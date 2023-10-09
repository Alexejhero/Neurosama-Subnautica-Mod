using NaughtyAttributes;
using SCHIZO.Unity.Sounds;
using UnityEngine;
using TechCategory_BZ = SCHIZO.Unity.Retargeting.BelowZero.TechCategory;
using TechCategory_SN = SCHIZO.Unity.Retargeting.Subnautica.TechCategory;
using TechGroup_BZ = SCHIZO.Unity.Retargeting.BelowZero.TechGroup;
using TechGroup_SN = SCHIZO.Unity.Retargeting.Subnautica.TechGroup;

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
        [BoxGroup("Additional Properties")] public Vector2Int itemSize = new Vector2Int(1, 1);

        [BoxGroup("Subnautica Data"), Label("Tech Group")] public TechGroup_SN techGroupSN = TechGroup_SN.Uncategorized;
        [BoxGroup("Subnautica Data"), Label("Tech Category"), HideIf(nameof(techCategorySN_HideIf))] public TechCategory_SN techCategorySN;
        private bool techCategorySN_HideIf() => techGroupSN == TechGroup_SN.Uncategorized;

        [BoxGroup("Below Zero Data"), Label("Tech Group")] public TechGroup_BZ techGroupBZ = TechGroup_BZ.Uncategorized;
        [BoxGroup("Below Zero Data"), Label("Tech Category"), HideIf(nameof(techCategoryBZ_HideIf))] public TechCategory_BZ techCategoryBZ;
        private bool techCategoryBZ_HideIf() => techGroupBZ == TechGroup_BZ.Uncategorized;

        public GameObject prefab;
        public BaseSoundCollection sounds;
    }
}
