using NaughtyAttributes;
using SCHIZO.Items.Data.Crafting;
using UnityEngine;

namespace SCHIZO.Items.Data
{
    partial class ItemData
    {
        #region Tomfoolery to work around HorizontalLine limitations
        [HorizontalLine(2, EColor.Green), ShowIf(nameof(autoRegister))]
        [SerializeField, Label("Prefab")] private GameObject _prefab1;

        [HorizontalLine(2, EColor.Red), HideIf(nameof(autoRegister))]
        [SerializeField, Label("Prefab")] private GameObject _prefab2;

        protected virtual void OnValidate()
        {
            if (_prefab1 == _prefab2 && prefab != _prefab1) _prefab1 = _prefab2 = prefab;
            if (prefab == _prefab2 && _prefab1 != prefab) prefab = _prefab2 = _prefab1;
            if (prefab == _prefab1 && _prefab2 != prefab) prefab = _prefab1 = _prefab2;
        }
        #endregion

        public Recipe RecipeSN => recipeSN;
        public Recipe RecipeBZ => recipeBZ;
    }
}
