using System;
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
        public bool autoRegister = false;

        #region Tomfoolery to work around HorizontalLine limitations
#if UNITY
        [HorizontalLine(2, EColor.Green), ShowIf(nameof(autoRegister))]
        [SerializeField, Label("Prefab")] private GameObject _prefab1;

        [HorizontalLine(2, EColor.Red), HideIf(nameof(autoRegister))]
        [SerializeField, Label("Prefab")] private GameObject _prefab2;

        private void OnValidate()
        {
            if (_prefab1 == _prefab2 && prefab != _prefab1) _prefab1 = _prefab2 = prefab;
            if (prefab == _prefab2 && _prefab1 != prefab) prefab = _prefab2 = _prefab1;
            if (prefab == _prefab1 && _prefab2 != prefab) prefab = _prefab1 = _prefab2;
        }
#endif
        #endregion

        [HideInInspector] public GameObject prefab;
        public BaseSoundCollection sounds;

        [BoxGroup("TechType")] public string classId;
        [BoxGroup("TechType")] public string displayName;
        [BoxGroup("TechType")] [ResizableTextArea] public string tooltip;

        [BoxGroup("Additional Properties")] public Sprite icon;
        [BoxGroup("Additional Properties"), HideIf(nameof(isBuildable))] public Vector2Int itemSize = new Vector2Int(1, 1);
        [BoxGroup("Additional Properties")] public bool isBuildable = false;

        [BoxGroup("Subnautica Data"), Label("Recipe"), SerializeField] private Recipe recipeSN;
        [BoxGroup("Subnautica Data"), Label("Tech Group"), SerializeField] private TechGroup_SN techGroupSN = TechGroup_SN.Uncategorized;
        [BoxGroup("Subnautica Data"), Label("Tech Category"), SerializeField, HideIf(nameof(techCategorySN_HideIf))] private TechCategory_SN techCategorySN;
        private bool techCategorySN_HideIf() => techGroupSN == TechGroup_SN.Uncategorized;

        [BoxGroup("Below Zero Data"), Label("Recipe"), SerializeField] private Recipe recipeBZ;
        [BoxGroup("Below Zero Data"), Label("Tech Group"), SerializeField] private TechGroup_BZ techGroupBZ = TechGroup_BZ.Uncategorized;
        [BoxGroup("Below Zero Data"), Label("Tech Category"), SerializeField, HideIf(nameof(techCategoryBZ_HideIf))] private TechCategory_BZ techCategoryBZ;
        private bool techCategoryBZ_HideIf() => techGroupBZ == TechGroup_BZ.Uncategorized;

#if !UNITY
        public SCHIZO.Items.ModItem ModItem { get; set; }

        public TechGroup TechGroup => SCHIZO.Retargeting.TechGroup.Pick(techGroupSN, techGroupBZ);
        public TechCategory TechCategory => SCHIZO.Retargeting.TechCategory.Pick(techCategorySN, techCategoryBZ);
#if BELOWZERO
        public Recipe Recipe => recipeBZ;
#else
        public Recipe Recipe => recipeSN;
#endif
#else
        public Recipe RecipeSN => recipeSN;
        public Recipe RecipeBZ => recipeBZ;
#endif
    }
}
