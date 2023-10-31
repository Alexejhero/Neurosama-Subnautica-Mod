namespace SCHIZO.Options.Generic;

partial class ConfigurableValue<TRaw, TModOption>
{
    public virtual TRaw GetValue()
    {
        return isHardCoded ? value : modOption.Value;
    }
}
