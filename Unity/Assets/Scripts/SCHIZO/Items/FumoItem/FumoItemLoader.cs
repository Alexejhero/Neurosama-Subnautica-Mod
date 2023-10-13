// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items.FumoItem
{
    public sealed class FumoItemLoader : ItemLoader
    {
        public override void Load()
        {
#if !UNITY
            new SCHIZO.Items.FumoItem.FumoItem(itemData.ModItem).Register();
#endif
        }
    }
}

#if UNITY_EDITOR
namespace SCHIZO.Unity.Data
{
    public partial class ItemData
    {
        [UnityEngine.ContextMenu("Set Loader/Fumo Item")]
        private void CreateFumoItemLoader() => AssignItemLoader(CreateInstance<Items.FumoItem.FumoItemLoader>());
    }
}
#endif
