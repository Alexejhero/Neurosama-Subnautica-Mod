using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Output;
using SCHIZO.Helpers;

namespace SCHIZO.Tweaks.Content;

[Command(Name = "contentalerts",
    DisplayName = "Content Alerts",
    Description = "Content alerts are a blinking red circle & arrow sprite that helps with not missing modded content.",
    RegisterConsoleCommand = true)]
internal sealed class ContentAlertsCommand : CompositeCommand
{
    [SubCommand(NameOverride = "enable", DisplayName = "Enable", Description = "Enable displaying content alerts.")]
    internal void EnableAlerts() => ToggleAlerts(true);
    [SubCommand(NameOverride = "disable", DisplayName = "Disable", Description = "Disable displaying content alerts.")]
    internal void DisableAlerts() => ToggleAlerts(false);
    private void ToggleAlerts(bool enabled) => ContentAlertManager.AlertsEnabled = enabled;

    [SubCommand(Description = "Attach content alerts to a specific item or creature (\"tech type\").")]
    internal void Attach(string techType) => ToggleTechType(techType, true);
    [SubCommand(Description = "Detach content alerts from a specific item or creature (\"tech type\").")]
    internal void Detach(string techType) => ToggleTechType(techType, false);

    private object ToggleTechType(string techTypeName, bool attach)
    {
        if (!UWE.Utils.TryParseEnum(techTypeName, out TechType techType))
            return MessageHelpers.TechTypeNotFound(techTypeName);
        if (attach)
            ContentAlertManager.AttachToTechType(techType);
        else
            ContentAlertManager.DetachFromTechType(techType);
        return CommonResults.OK();
    }

    [SubCommand(Description = "Detach content alerts from all attached tech types.\nPrefabs that have the alert prefab on them (like jukebox disks) will keep the alerts.")]
    internal void Clear() => ContentAlertManager.Clear();
}
