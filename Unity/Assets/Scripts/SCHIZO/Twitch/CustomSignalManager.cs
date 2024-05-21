using SCHIZO.Items.Data.Crafting;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Twitch
{
    public partial class CustomSignalManager : MonoBehaviour
    {
        [ValidateInput("ValidateItem")]
        public Item signalPrefab;

#if UNITY_EDITOR
        private TriValidationResult ValidateItem()
        {
            if (!signalPrefab.ItemData || !signalPrefab.ItemData.prefab
                || !signalPrefab.ItemData.prefab.GetComponent("SignalPing"))
                return TriValidationResult.Error("Signal prefab must be linked to custom item data with SignalPing component on its prefab");
            return TriValidationResult.Valid;
        }
#endif
    }
}
