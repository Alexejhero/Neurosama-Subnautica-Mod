using UnityEngine;
using SCHIZO.VFX;

public class testdummy : MonoBehaviour
{
    public Material material;
    public Transform target;

    #region VFX forom script

    private Material mat;
    private int strengthID = Shader.PropertyToID("_Strength");
    private SchizoVFXComponent svc;

    private void Awake()
    {
        mat = VFXBank.NoisyVignette;
        svc = gameObject.AddComponent<SchizoVFXComponent>();
        svc.usingCustomMaterial = true;
    }

    private void LateUpdate()
    {
        mat.SetFloat(strengthID, Mathf.Sin(Time.time));
        svc.applyEffect(mat);
    }

    #endregion 

    private void Update()
    {
        Vector2 pos = Camera.main.WorldToScreenPoint(target.position);
        float rnd = Random.Range(-1f, 1f);
        material.SetVector("_ScreenPosition", new Vector4(pos.x, pos.y, 1, rnd));
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, null, material);
    }
}
