namespace SCHIZO.Items.Gymbag
{
    public sealed class GymbagLoader : ItemLoader
    {
        public override void Load()
        {
#if !UNITY
            new Gymbag(itemData.ModItem, itemData.CloneTarget).Register();
#endif
        }
    }
}

#if UNITY_EDITOR
namespace SCHIZO.Items
{
    partial class CloneItemData
    {
        [UnityEngine.ContextMenu("Set Loader/Gymbag")]
        private void CreateGymbagLoader() => AssignItemLoader(CreateInstance<Gymbag.GymbagLoader>());
    }
}
#endif
