using SCHIZO.TriInspector;
using SCHIZO.TriInspector.Attributes;
using TriInspector;

public class OnSurfaceMovement : TriMonoBehaviour
{
    [ComponentReferencesGroup, Required] public OnSurfaceTracker onSurfaceTracker;
    [ComponentReferencesGroup, Required] public Locomotion locomotion;
    [UnexploredGroup] public float targetRange = 1;
    [UnexploredGroup] public float onSurfaceDelay;
}
