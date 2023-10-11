// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items.Gymbag
{
    public sealed class GymbagLoader : ItemLoader
    {
        public override void Load()
        {
#if !UNITY
            new SCHIZO.Items.Gymbag.Gymbag(itemData.ModItem, itemData.CloneTarget).Register();
#endif
        }
    }
}

#if UNITY_EDITOR
namespace SCHIZO.Unity.Items
{
    partial class CloneItemData
    {
        [UnityEngine.ContextMenu("Set Loader/Gymbag")]
        private void CreateGymbagLoader() => AssignItemLoader(CreateInstance<Gymbag.GymbagLoader>());
    }
}
#endif
