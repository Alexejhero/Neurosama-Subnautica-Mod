using UWE;

public partial class VFX_ARGCensor
{
    private void Start()
    {
        Player player = Utils.GetLocalPlayer().GetComponent<Player>();
        player.isUnderwater.changedEvent.AddHandler(this, new Event<Utils.MonitoredValue<bool>>.HandleFunction(UpdateUnderwaterState));
    }

    private void UpdateUnderwaterState(Utils.MonitoredValue<bool> isUnderwater)
    {
        this.isUnderwater = isUnderwater.value;
    }
}
