using System.Text;

namespace SCHIZO.Items.Components;

public abstract partial class CustomPlayerTool
{
    public override string animToolName => data.referenceAnimation.ToString().ToLower();

    protected new void Awake()
    {
        if (data.subnauticaModel) data.subnauticaModel.SetActive(IS_SUBNAUTICA);
        if (data.belowZeroModel) data.belowZeroModel.SetActive(IS_BELOWZERO);

        if (IS_BELOWZERO)
        {
            leftHandIKTarget = data.leftHandIKTargetOverrideBZ;
            rightHandIKTarget = data.rightHandIKTargetOverrideBZ;
        }

        base.Awake();
    }

    private int cachedPrimaryUseTextHash;
    private int cachedSecondaryUseTextHash;
    private int cachedAltUseTextHash;
    private string cachedFullUseText;

    public override string GetCustomUseText()
    {
        string primaryText = hasPrimaryUse ? LanguageCache.GetButtonFormat(primaryUseText, GameInput.Button.RightHand) : "";
        string secondaryText = hasSecondaryUse ? LanguageCache.GetButtonFormat(secondaryUseText, GameInput.Button.LeftHand) : "";
        string altText = hasAltUse ? LanguageCache.GetButtonFormat(altUseText, GameInput.Button.AltTool) : "";

        int primaryHash = primaryText.GetHashCode();
        int secondaryHash = secondaryText.GetHashCode();
        int altHash = altText.GetHashCode();

        if (cachedPrimaryUseTextHash != primaryHash
            || cachedSecondaryUseTextHash != secondaryHash
            || cachedAltUseTextHash != altHash)
        {
            cachedPrimaryUseTextHash = primaryHash;
            cachedSecondaryUseTextHash = secondaryHash;
            cachedAltUseTextHash = altHash;

            StringBuilder sb = new();
            sb.Append(primaryText);
            if (secondaryText.Length > 0 && sb.Length > 0)
                sb.Append(", ");
            sb.Append(secondaryText);
            if (altText.Length > 0 && sb.Length > 0)
                sb.Append(", ");
            sb.Append(altText);

            cachedFullUseText = sb.ToString();
        }

        return cachedFullUseText;
    }

    // TODO sn
#if BELOWZERO
    public override void OnHolsterBegin()
    {
        OnToolAnimHolster();
        base.OnHolsterBegin();
    }
#endif
}
