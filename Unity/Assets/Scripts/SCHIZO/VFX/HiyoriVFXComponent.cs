using SCHIZO.VFX;
using UnityEngine;

public class HiyoriVFXComponent : SchizoVFXComponent
{
    private readonly int propID = Shader.PropertyToID("_ScreenPosition");

    private void LateUpdate()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        float rnd = Random.Range(-1f, 1f);
        matWithProps.vectorPropertyID = propID;
        matWithProps.vectorPropertyValue = new Vector4(pos.x, pos.y, pos.z, rnd);
        SendEffect(matWithProps);
    }
}
