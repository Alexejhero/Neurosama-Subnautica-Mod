using SCHIZO.API.Unity.Sounds;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.API.Unity.Items
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Item Data")]
    public sealed class ItemData : ScriptableObject
    {
        public GameObject prefab;
        public Sprite icon;
        public BaseSoundCollection sounds;
    }
}
