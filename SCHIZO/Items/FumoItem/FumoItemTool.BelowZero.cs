using RuntimeDebugDraw;

namespace SCHIZO.Items.FumoItem;

public partial class FumoItemTool
{
    public void Start()
    {
        Draw.AttachText(uGUI.main.transform, () => usingPlayer?.GetComponent<BodyTemperature>()?.coldResistEquipmentBuff.ToString(), size: 30);
    }

    private void ApplyColdResistBuff(int buff)
    {
        BodyTemperature bodyTemperature = usingPlayer.GetComponent<BodyTemperature>();
        if (!bodyTemperature) return;
        bodyTemperature.coldResistEquipmentBuff += buff;
    }
}
