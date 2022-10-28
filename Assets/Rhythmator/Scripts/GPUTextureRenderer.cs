#if UNITY_EDITOR
using UnityEngine;

public class GPUTextureRenderer
{

    public static Texture2D Render(Texture sampleData, Material material, int width, int height, int numChannels)
    {
        material.SetTexture("_SampleTex", sampleData);
        material.SetInt("_NumChannels", numChannels);
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        RenderTexture o = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(tex, o, material);
        RenderTexture.active = o;
        tex.ReadPixels(RhythmUtility.CreateRect(0, 0, width, height), 0, 0, false);
        tex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(o);
        return tex;
    }

}
#endif