using NaughtyAttributes;
using SCHIZO.Interop.NaughtyAttributes;

namespace SCHIZO.Attributes.Validation
{
    internal abstract class SwitchDropdownAttribute : DrawerAttribute
    {
        public abstract string GetDropdownListName(SerializedPropertyHolder property);
    }
}
