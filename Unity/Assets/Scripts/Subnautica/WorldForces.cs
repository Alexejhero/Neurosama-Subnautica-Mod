using SCHIZO.TriInspector;
using TriInspector;
using UnityEngine;

[DeclareToggleGroup("gravity", Title = "Gravity")]
[DeclareToggleGroup("drag", Title = "Drag")]
[DeclareToggleGroup("wind", Title = "Wind (BZ only)")]
public class WorldForces : TriMonoBehaviour
{
    [ComponentReferencesGroup, NaughtyAttributes.Required] public Rigidbody useRigidbody;

    public bool moveWithPlatform;

    [Group("gravity")] public bool handleGravity = true;
    [Group("gravity")] public float aboveWaterGravity = 9.81f;
    [Group("gravity")] public float underwaterGravity = 0.0f;

    [Group("drag")] public bool handleDrag = true;
    [Group("drag")] public float aboveWaterDrag = 0.0f; // Base-game default: 0.1f
    [Group("drag")] public float underwaterDrag = 0.1f; // Base-game default: 1.0f

    [Group("wind")] public bool handleWind = false;
    [Group("wind")] public float windScalar = 1f;
}
