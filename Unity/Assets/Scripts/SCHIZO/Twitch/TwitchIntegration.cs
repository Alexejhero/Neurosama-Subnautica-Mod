using System.Collections.Generic;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Twitch
{
    public sealed partial class TwitchIntegration : MonoBehaviour
    {
        [SerializeField, Required, UsedImplicitly] private string targetChannel = "vedal987";
        [SerializeField, Required, UsedImplicitly] private string commandPrefix = "pls ";
        [SerializeField, ShowIf(nameof(prefixIsCaseSensitive_ShowIf)), UsedImplicitly] private bool prefixIsCaseSensitive = false;
        [SerializeField, ListDrawerSettings, UsedImplicitly] private List<string> whitelistedUsers = new List<string> { "alexejherodev" };

        private bool prefixIsCaseSensitive_ShowIf()
        {
            return commandPrefix.ToLowerInvariant() != commandPrefix.ToUpperInvariant();
        }
    }
}
