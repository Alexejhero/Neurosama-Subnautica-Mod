using System.Collections.Generic;
using NaughtyAttributes;
using SCHIZO.Interop.NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Twitch
{
    public sealed partial class TwitchIntegration : NaughyMonoBehaviour
    {
        [SerializeField, Required_string] private string targetChannel = "vedal987";
        [SerializeField, Required_string] private string commandPrefix = "pls ";
        [SerializeField, ShowIf(nameof(prefixIsCaseSensitive_ShowIf))] private bool prefixIsCaseSensitive = false;
        [SerializeField, ReorderableList] private List<string> whitelistedUsers = new List<string> { "alexejherodev" };

        private bool prefixIsCaseSensitive_ShowIf()
        {
            return commandPrefix.ToLowerInvariant() != commandPrefix.ToUpperInvariant();
        }
    }
}
