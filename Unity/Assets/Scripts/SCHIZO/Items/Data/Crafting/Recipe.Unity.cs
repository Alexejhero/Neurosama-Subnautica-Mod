using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEditor;

namespace SCHIZO.Items.Data.Crafting
{
    partial class Recipe
    {
#if UNITY_EDITOR
        private static readonly FieldInfo _activeGame = AccessTools.Field(AccessTools.TypeByName("TechType_AllDrawer"), "TargetGame");

        [UsedImplicitly]
        private void OnGameChanged()
        {
            if (Selection.activeObject == this) _activeGame.SetValue(null, game);
        }
#endif
    }
}
