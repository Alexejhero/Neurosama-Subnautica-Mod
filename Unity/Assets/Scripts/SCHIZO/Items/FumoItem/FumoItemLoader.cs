using UnityEngine;

namespace SCHIZO.Items.FumoItem
{
    public sealed partial class FumoItemLoader : ItemLoader
    {
        [Tooltip("Where to spawn the fumo on a new game.")]
        public Vector3 spawnPosition = new Vector3(-307, 18f, 274f);
        public Vector3 spawnRotation;
    }
}

#if UNITY_EDITOR
namespace SCHIZO.Items.Data
{
    public partial class ItemData
    {
        [ContextMenu("Set Loader/Fumo Item")]
        private void CreateFumoItemLoader() => AssignItemLoader(CreateInstance<FumoItem.FumoItemLoader>());
    }
}
#endif
