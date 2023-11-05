using SCHIZO.Interop.Subnautica;
using TriExtensions;
using TriInspector;

public class AggressiveToPilotingVehicle : TriMonoBehaviour
{
    [ComponentReferencesGroup, Required] public LastTarget lastTarget;
    [ComponentReferencesGroup, Required] public _Creature creature;
    public float range = 20;
    public float aggressionPerSecond = 0.5f;
    public float targetPriority = 1;

    // ReSharper disable once Unity.RedundantEventFunction
    private void Start() {}
}
