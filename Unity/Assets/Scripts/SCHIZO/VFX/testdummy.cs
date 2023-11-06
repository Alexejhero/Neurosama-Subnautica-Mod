using UnityEngine;

public class testdummy : MonoBehaviour
{
    public Material material;
    public Transform target;

    private void LateUpdate()
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
