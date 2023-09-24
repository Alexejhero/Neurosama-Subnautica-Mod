using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;

namespace SCHIZO.Credits;

[HarmonyPatch]
public static class CreditsPatches
{
    private readonly struct Credits
    {
        private static readonly List<Credits> All = new();

        public static readonly Credits Programming = new("Programming", "Developers");
        public static readonly Credits Modeling = new("3D Modeling", "3D Modelers");
        public static readonly Credits Animations = new("Animations", "Animators");
        public static readonly Credits Artist = new("2D Art", "2D Artists");
        public static readonly Credits Sounds = new("Sounds", "Audio Compilation & Cleaning");
        public static readonly Credits Lore = new("Lore", "Writing & Lore");
        public static readonly Credits ProjectLead = new("Project Lead", "Project Lead");

        public static IReadOnlyList<Credits> GetAll() => All;

        private readonly List<string> _soFar;

        private Credits(string sn, string bz)
        {
            _soFar = new List<string>
            {
                IS_BELOWZERO ? bz : sn
            };

            All.Add(this);
        }

        private Credits(Credits a, Credits b)
        {
            _soFar = a._soFar.Concat(b._soFar).ToList();
        }

        public IReadOnlyList<string> ToStringList() => _soFar;

        public static Credits operator +(Credits a, Credits b)
        {
            return new Credits(a, b);
        }
    }

    private static readonly Dictionary<string, Credits> _credits = new()
    {
        ["2Pfrog"] = Credits.Artist,
        ["AlexejheroDev"] = Credits.Programming + Credits.ProjectLead,
        ["Azit"] = Credits.Artist,
        ["baa14453"] = Credits.Lore,
        ["budwheizzah"] = Credits.Programming + Credits.Artist + Credits.Sounds,
        ["chrom"] = Credits.Artist,
        ["CJMAXiK"] = Credits.Artist + Credits.Sounds,
        ["darkeew"] = Credits.Programming,
        ["FutabaKuuhaku"] = Credits.Modeling,
        ["Govorunb"] = Credits.Programming,
        ["greencap"] = Credits.Modeling,
        ["Hakuhan"] = Credits.Artist,
        ["Kat"] = Credits.Modeling,
        ["Kaz"] = Credits.Artist,
        ["Lorx"] = Credits.Artist,
        ["Moloch"] = Credits.Artist,
        ["MyBraza"] = Credits.Artist + Credits.Sounds,
        ["NetPlayz"] = Credits.Artist,
        ["P3R"] = Credits.Artist,
        ["paccha"] = Credits.Artist,
        ["Rune"] = Credits.Artist,
        ["SADecsSs"] = Credits.Artist,
        ["Sandro"] = Credits.Artist,
        ["SomeOldGuy"] = Credits.Artist,
        ["sugarph"] = Credits.Artist,
        ["Troobs"] = Credits.Artist,
        ["Vaalmyr"] = Credits.Modeling,
        ["w1n7er"] = Credits.Modeling + Credits.Animations,
        ["yamplum"] = Credits.Artist + Credits.Lore,
        ["YuG"] = Credits.Modeling,
    };

    [HarmonyPatch(typeof(EndCreditsManager), nameof(EndCreditsManager.Start))]
    public static class UpdateCreditsTextTranspiler
    {
        private static readonly MethodInfo _target =
#if BELOWZERO
            AccessTools.Method(typeof(UnityEngine.MonoBehaviour), nameof(UnityEngine.MonoBehaviour.Invoke));
#else
            AccessTools.Method(typeof(TMPro.TMP_Text), nameof(TMPro.TMP_Text.SetText), new[] { typeof(string), typeof(bool) });
#endif

        [HarmonyTranspiler, UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;

                if (instruction.Calls(_target))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UpdateCreditsTextTranspiler), nameof(Patch)));
                }
            }
        }

        private static void Patch(EndCreditsManager __instance)
        {
#if BELOWZERO
            __instance.centerText.SetText(GetCreditsTextBZ() + __instance.centerText.text);
#else
            EasterEggPatches.easterEggAdjusted = false;

            float oldHeight = 14100;//__instance.textField.preferredHeight;
            __instance.textField.SetText(GetCreditsTextSN() + __instance.textField.text);
            __instance.scrollSpeed = __instance.textField.preferredHeight * __instance.scrollSpeed / oldHeight;
            __instance.scrollStep = __instance.textField.preferredHeight * __instance.scrollStep / oldHeight;
#endif
        }
    }

    [UsedImplicitly]
    private static string GetCreditsTextSN()
    {
        StringBuilder builder = new("<style=h1>Neuro-sama Subnautica Mod</style>");
        builder.AppendLine();
        builder.AppendLine();

        foreach (KeyValuePair<string, Credits> kvp in _credits)
        {
            builder.Append("<style=left>");
            builder.Append(kvp.Key);
            builder.Append("</style>");
            builder.Append("<style=right>");
            builder.Append(string.Join(", ", kvp.Value.ToStringList()));
            builder.Append("</style>");
            builder.AppendLine();
        }

        builder.AppendLine();
        builder.AppendLine();

        return builder.ToString();
    }

    [UsedImplicitly]
    private static string GetCreditsTextBZ()
    {
        StringBuilder builder = new("<style=h1>Neuro-sama Subnautica Mod</style>");
        builder.AppendLine();
        builder.AppendLine();

        foreach (string credit in Credits.GetAll().Select(c => c.ToStringList()[0]))
        {
            builder.Append("<style=role>");
            builder.Append(credit);
            builder.Append("</style>");
            builder.AppendLine();

            foreach (KeyValuePair<string, Credits> kvp in _credits.Where(c => c.Value.ToStringList().Contains(credit)))
            {
                builder.Append(kvp.Key);
                builder.AppendLine();
            }

            builder.AppendLine();
        }

        builder.AppendLine();
        builder.AppendLine();

        return builder.ToString();
    }
}
