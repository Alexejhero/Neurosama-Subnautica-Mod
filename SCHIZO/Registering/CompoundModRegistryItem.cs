namespace SCHIZO.Registering;

partial class CompoundModRegistryItem
{
    protected override void Register()
    {
        foreach (ModRegistryItem item in registryItems)
        {
            item.InvokeRegister();
        }
    }

    protected override void PostRegister()
    {
        foreach (ModRegistryItem item in registryItems)
        {
            item.InvokePostRegister();
        }
    }
}
