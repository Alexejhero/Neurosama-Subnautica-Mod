using SCHIZO.VFX;
using UnityEngine;

public class HiyoriVFXComponent : SchizoVFXComponent
{
    private void LateUpdate()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        float rnd = Random.Range(-1f, 1f) * Time.timeScale;

        MatWithProps mwp = ScriptableObject.CreateInstance<MatWithProps>();
        mwp.material = matWithProps.material;
        mwp.floatPropertyName = "_Distance";
        mwp.floatPropertyValue = Vector3.SqrMagnitude(Camera.main.transform.position - transform.position);
        mwp.vectorPropertyName = "_ScreenPosition";
        mwp.vectorPropertyValue = new Vector4(pos.x, pos.y, pos.z, rnd);
        SendEffect(mwp);
    }
}
