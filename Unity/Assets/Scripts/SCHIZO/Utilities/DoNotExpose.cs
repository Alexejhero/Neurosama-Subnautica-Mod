using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Utilities
{
    public sealed class DoNotExpose : ScriptableObject
    {
        [ResizableTextArea, ReadOnly]
        public string note = "This scriptable object is used to tell the build script\nto not expose this asset in the CS file.";
    }
}
