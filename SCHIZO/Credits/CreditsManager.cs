using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace SCHIZO.Credits;

partial class CreditsManager
{
    [UsedImplicitly]
    public string GetCreditsTextSN()
    {
        StringBuilder builder = new("<style=h1>Neuro-sama Subnautica Mod</style>");
        builder.AppendLine();
        builder.AppendLine();

        foreach (CreditsData.CreditsEntry entry in creditsData.mainCredits)
        {
            builder.Append("<style=left>");
            builder.Append(entry.name);
            builder.Append("</style>");
            builder.Append("<style=right>");
            builder.Append(string.Join(", ", entry.credits.ToList().Select(CreditsTypeExtensions.GetSN)));
            builder.Append("</style>");
            builder.AppendLine();
        }

        builder.AppendLine();
        builder.AppendLine();

        // TODO: add extra credits

        return builder.ToString();
    }

    [UsedImplicitly]
    public string GetCreditsTextBZ()
    {
        StringBuilder builder = new("<style=h1>Neuro-sama Subnautica Mod</style>");
        builder.AppendLine();
        builder.AppendLine();

        foreach (CreditsData.CreditsType credit in Enum.GetValues(typeof(CreditsData.CreditsType)))
        {
            builder.Append("<style=role>");
            builder.Append(credit.GetBZ());
            builder.Append("</style>");
            builder.AppendLine();

            foreach (CreditsData.CreditsEntry entry in creditsData.mainCredits.Where(c => c.credits.HasFlag(credit)))
            {
                builder.Append(entry.name);
                builder.AppendLine();
            }

            builder.AppendLine();
        }

        builder.AppendLine();
        builder.AppendLine();

        // TODO: add extra credits

        return builder.ToString();
    }
}
