using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "SCHIZO/Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Prefab Info")]
        public string classId;
        public string displayName;
        public string tooltip;
        public Sprite icon;
        public Vector2Int size = new Vector2Int(1, 1);
    }
}
