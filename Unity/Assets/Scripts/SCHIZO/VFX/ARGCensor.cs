using SCHIZO.VFX;
using TriInspector;
using UnityEngine;

public class ARGCensor : MonoBehaviour
{
    [InlineEditor]
    public Material effectMaterial;

    public float fadeOutStartDistance = 35f;
    public float scale = 1.3f;
    public float frameChangeInterval = 0.08f;

    private MatPassID matPassID;

    private float lastUpdate;
    private float lastRnd = 0f;
    private int texID = Shader.PropertyToID("_Images");
    private float arrayDepth;

    public void Awake()
    {
        _ = SchizoVFXStack.VFXStack;

        matPassID = new MatPassID(effectMaterial);

        lastUpdate = Time.time;
        arrayDepth = ((Texture2DArray) effectMaterial.GetTexture(texID)).depth;
    }

    public void LateUpdate()
    {
        Vector3 dirToCam = (Camera.main.transform.position - transform.position).normalized;
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position + dirToCam);

        float dot = Vector3.Dot(transform.forward, dirToCam);

        bool isBelowWaterLevel = Camera.main.transform.position.y < 0f;

        if (isBelowWaterLevel && pos.z > 0 && dot > -0.3f)
        {
            if (Time.time - lastUpdate > frameChangeInterval)
            {
                lastRnd = Random.Range(0, arrayDepth) * Time.timeScale;
                lastUpdate = Time.time;
            }

            float opacity = Mathf.Clamp01((1f / pos.z) * fadeOutStartDistance);

            matPassID.SetVector("_ScreenPosition", new Vector4(pos.x, pos.y, pos.z, lastRnd));
            matPassID.SetFloat("_Strength", opacity);
            matPassID.SetFloat("_Scale", scale);

            SchizoVFXStack.RenderEffect(matPassID);
        }
    }
}
