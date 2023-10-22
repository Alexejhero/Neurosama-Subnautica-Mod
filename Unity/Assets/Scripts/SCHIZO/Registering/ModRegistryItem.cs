using SCHIZO.Interop.NaughtyAttributes;

namespace SCHIZO.Registering
{
    public abstract partial class ModRegistryItem : NaughtyScriptableObject
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
