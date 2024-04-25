#if UNITY_EDITOR
using SCHIZO.Interop.Subnautica.Enums;
#endif
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.Components
{
    [AddComponentMenu("SCHIZO/Sea Monkey Held Item Overrides")]
    public sealed partial class SeaMonkeyHeldItemOverrides : MonoBehaviour
    {
        [InfoBox("When a sea monkey grabs this object, temporarily set the target transform's local position and rotation.\n"
               + "Use this to fix misaligned items.")]
        public Transform overrideTransform;
        public Vector3 localPosition;
        public Vector3 localRotation;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!enabled) return;

            Component ecoTarget = GetComponent("EcoTarget");
            EcoTargetType_All type = (EcoTargetType_All)HarmonyLib.AccessTools.Field(ecoTarget.GetType(), "type").GetValue(ecoTarget);
            if (type != EcoTargetType_All.Shiny)
                Debug.LogWarning("Sea monkeys only steal Shiny items, so these overrides are useless");
        }
        private void OnDisable() {}
        private void OnEnable() {}
#endif
    }
}
