using SCHIZO.VFX;

public class GenericVFXComponent : SchizoVFXComponent
{
    private void LateUpdate()
    {
        SendEffect(matWithProps);
    }
}
