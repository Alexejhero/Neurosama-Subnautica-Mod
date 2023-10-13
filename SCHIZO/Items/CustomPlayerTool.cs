using System.Text;

namespace SCHIZO.Unity.Items;

public abstract partial class CustomPlayerTool : PlayerTool
{
    protected string animName;
    protected TechType animTechType
    {
        set => animName = value.AsString(true);
    }
    public override string animToolName => animName ?? base.animToolName;

    private int cachedPrimaryUseTextHash;
    private int cachedSecondaryUseTextHash;
    private int cachedAltUseTextHash;
    private string cachedFullUseText;

    protected new void Awake()
    {
        TechType animType = (TechType) Helpers.RetargetHelpers.Pick(inheritAnimationsFromSN, inheritAnimationsFrom2);
        if (animType != TechType.None)
            animTechType = animType;
        base.Awake();
    }

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
}
