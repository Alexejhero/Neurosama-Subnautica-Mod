namespace SCHIZO.Items.Gymbag
{
    public sealed partial class GymbagLoader : CloneItemLoader
    {
    }
}

#if UNITY_EDITOR
namespace SCHIZO.Items.Data
{
    partial class CloneItemData
    {
        [UnityEngine.ContextMenu("Set Loader/Gymbag")]
        private void CreateGymbagLoader() => AssignItemLoader(CreateInstance<Gymbag.GymbagLoader>());
    }
}
#endif
