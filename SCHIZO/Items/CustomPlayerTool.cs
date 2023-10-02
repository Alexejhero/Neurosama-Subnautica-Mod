using System.Text;

namespace SCHIZO.Items;

public abstract class CustomPlayerTool : PlayerTool
{
    protected string animName;
    protected TechType animTechType
    {
        set => animName = value.AsString(true);
    }
    public override string animToolName => animName ?? base.animToolName;

    protected bool hasPrimaryUse; // RMB
    protected bool hasSecondaryUse; // LMB
    protected bool hasAltUse; // F

    protected string primaryUseTextLanguageString;
    protected string secondaryUseTextLanguageString;
    protected string altUseTextLanguageString;

    private int cachedPrimaryUseTextHash;
    private int cachedSecondaryUseTextHash;
    private int cachedAltUseTextHash;
    private string cachedFullUseText;

    public override string GetCustomUseText()
    {
        string primaryText = hasPrimaryUse ? LanguageCache.GetButtonFormat(primaryUseTextLanguageString, GameInput.Button.RightHand) : "";
        string secondaryText = hasSecondaryUse ? LanguageCache.GetButtonFormat(secondaryUseTextLanguageString, GameInput.Button.LeftHand) : "";
        string altText = hasAltUse ? LanguageCache.GetButtonFormat(altUseTextLanguageString, GameInput.Button.AltTool) : "";

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
