namespace SCHIZO.Registering;

partial class ModRegistryItem
{
    private bool _registered;
    private bool _postRegistered;

    public void InvokeRegister()
    {
        if (_registered)
        {
            LOGGER.LogWarning($"Trying to register {GetType().Name} {name} multiple times! Skipping");
            return;
        }

        _registered = true;
        LOGGER.LogInfo($"Registering {GetType().Name} {name}");
        Register();
    }

    public void InvokePostRegister()
    {
        if (_postRegistered)
        {
            LOGGER.LogWarning($"Trying to post-register {GetType().Name} {name} multiple times! Skipping");
            return;
        }

        _postRegistered = true;
        LOGGER.LogInfo($"Post-registering {GetType().Name} {name}");
        PostRegister();
    }

    protected abstract void Register();

    protected virtual void PostRegister() { }
}
