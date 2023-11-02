using Nautilus.Handlers;

namespace SCHIZO.Options;

partial class ModOptionPanel
{
    protected override void Register()
    {
        OptionsPanelHandler.RegisterModOptions(new RuntimeModOptions(options));
    }
}
