using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class VortexEffect : ImageEffectBase
{
    public Vector2 radius = new Vector2(0.3F, 0.3F);
    [Range(0.0f, 360.0f)]
    public float angle = 50;
    public Vector2 center = new Vector2(0.5F, 0.5F);

    Texture2D savedTexture;

    public void PassTheTexture(Texture2D screenScrape)
    {
        savedTexture = screenScrape;
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(savedTexture == null)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            RenderVortex(material, savedTexture, destination, angle, center, radius);
        }
    }

    public static void RenderVortex(Material material, Texture source, RenderTexture destination, float angle, Vector2 center, Vector2 radius)
    {
        bool invertY = source.texelSize.y < 0.0f;
        if (invertY)
        {
            center.y = 1.0f - center.y;
            angle = -angle;
        }

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), Vector3.one);

        material.SetMatrix("_RotationMatrix", rotationMatrix);
        material.SetVector("_CenterRadius", new Vector4(center.x, center.y, radius.x, radius.y));
        material.SetFloat("_Angle", angle * Mathf.Deg2Rad);

        Graphics.Blit(source, destination, material);
    }
}
