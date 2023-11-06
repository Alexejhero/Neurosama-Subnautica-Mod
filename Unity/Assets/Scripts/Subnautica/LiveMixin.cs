using SCHIZO.TriInspector;
using SCHIZO.TriInspector.Attributes;
using TriInspector;

public class LiveMixin : TriMonoBehaviour
{
    [Required, InlineEditor] public LiveMixinData data;
    public float health;

    [UnexploredGroup] public FMODAsset damageSound;
    [UnexploredGroup] public FMODAsset deathSound;
    [UnexploredGroup] public FMODAsset damageClip;
    [UnexploredGroup] public FMODAsset deathClip;
}
