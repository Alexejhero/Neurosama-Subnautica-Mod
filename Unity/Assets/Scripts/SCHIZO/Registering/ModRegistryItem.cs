using SCHIZO.TriInspector;

namespace SCHIZO.Registering
{
    public abstract partial class ModRegistryItem : TriScriptableObject
    {
        protected bool IsRegistered
        {
            get
            {
#if UNITY_EDITOR
                return ModRegistry.Instance.registryItems.Contains(this);
#else
                return false;
#endif
            }
        }
    }
}
