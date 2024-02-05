using System;

namespace SCHIZO.Registering;

partial class CompoundModRegistryItem
{
    protected override void Register()
    {
        foreach (ModRegistryItem item in registryItems)
        {
            try
            {
                if (item) item.InvokeRegister();
            }
            catch (Exception ex)
            {
                LOGGER.LogError($"Could not register {item}! {ex}");
            }
        }
    }

    protected override void PostRegister()
    {
        foreach (ModRegistryItem item in registryItems)
        {
            try
            {
                if (item) item.InvokePostRegister();
            }
            catch (Exception ex)
            {
                LOGGER.LogError($"Could not register {item}! {ex}");
            }
        }
    }
}
