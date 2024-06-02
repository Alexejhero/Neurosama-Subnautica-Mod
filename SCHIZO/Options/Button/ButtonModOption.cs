using Nautilus.Options;

namespace SCHIZO.Options.Button;

partial class ButtonModOption
{
    protected override OptionItem GetOptionItemInternal()
        => ModButtonOption.Create(id, label, (_) => onPressed.Invoke(), tooltip);
}
