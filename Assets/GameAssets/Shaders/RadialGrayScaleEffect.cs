
using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Color Adjustments/Grayscale")]
    public class RadialGrayScaleEffect : ImageEffectBase
    {
        public Texture textureRamp;

        [Range(-1.0f, 1.0f)]
        public float rampOffset;

        //[Range(-1.0f, 1.0f)]
        public Vector2 radius = new Vector2(0.3F, 0.3F);
        public Vector2 center = new Vector2(0.5F, 0.5F);

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            material.SetTexture("_RampTex", textureRamp);
            material.SetFloat("_RampOffset", rampOffset);
            material.SetVector("_CenterRadius", new Vector4(center.x, center.y, radius.x, radius.y));
            Graphics.Blit(source, destination, material);
        }
    }

