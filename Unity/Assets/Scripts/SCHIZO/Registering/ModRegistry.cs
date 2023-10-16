using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Registering;

[CreateAssetMenu(menuName = "SCHIZO/Registering/Mod Registry")]
public sealed class ModRegistry : ScriptableObject
{
    [ReorderableList] public List<ModRegistryItem> registeredItems = new();
}
