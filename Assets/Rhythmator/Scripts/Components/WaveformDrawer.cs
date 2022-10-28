#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[System.Serializable]
public class WaveformDrawer
{
    Texture2D tex;
    Rect textureRect;
    bool dirty;
    Rhythm rhythm;
    Material material;

    public WaveformDrawer(Rhythm rhythm, Vector2 size, Material material)
    {
        this.material = material;
        if (rhythm.timelineZoom == 0) rhythm.timelineZoom = 1;
        this.rhythm = rhythm;
        textureRect = new Rect(new Vector2(0, 0), size);
        dirty = true;
        tex = new Texture2D((int)textureRect.width, (int)textureRect.height, TextureFormat.RGBA32, false);
    }

    public void SetWidth(float width)
    {
        if(textureRect.width != width) {
            textureRect.width = width;
            dirty = true;
        }
    }

    public void UpdateReference(Rhythm rhythm)
    {
        if(rhythm != this.rhythm) {
            dirty = true;
        }
        this.rhythm = rhythm;
    }

    public void SetDirty()
    {
        dirty = true;
    }

    public void UpdateMaterial(Material material)
    {
        this.material = material;
    }

    public void DrawGUI()
    {
        Rect r = GUILayoutUtility.GetRect(textureRect.width, textureRect.height);
        textureRect.x = r.x;
        textureRect.y = Mathf.Max(r.y, textureRect.y);
        if (dirty || tex == null) {
            //GenerateTexture(0);//quality);
            GenerateSampleTex(100);
            tex = GPUTextureRenderer.Render(tex, material, (int)textureRect.width, (int)textureRect.height, ClipChannels());
            dirty = false;
        }
        EditorGUI.DrawTextureTransparent(textureRect, tex);
        if (textureRect.width * rhythm.timelineZoom - textureRect.width > 0) {
            float sc = EditorGUILayout.Slider(rhythm.timelineScroll, 0, textureRect.width * rhythm.timelineZoom - textureRect.width, GUILayout.Width(textureRect.width));
            if (sc != rhythm.timelineScroll) {
                SetScroll(sc);
            }
        }
    }

    public Rect GetRect()
    {
        return textureRect;
    }

    public void SetScroll(float scroll)
    {
        if (scroll < 0) scroll = 0;
        if(scroll > textureRect.width * rhythm.timelineZoom - textureRect.width) {
            scroll = textureRect.width * rhythm.timelineZoom - textureRect.width;
        }
        rhythm.timelineScroll = scroll;
        dirty = true;
    }

    public float GetScroll()
    {
        return rhythm.timelineScroll;
    }

    public void SetZoom(float zoom)
    {
        if (zoom < 1) zoom = 1;

        float fac = (rhythm.timelineScroll + Event.current.mousePosition.x) / (textureRect.width * rhythm.timelineZoom);
        rhythm.timelineZoom = zoom;

        SetScroll(fac * (textureRect.width * rhythm.timelineZoom) - Event.current.mousePosition.x);

        dirty = true;
    }

    public float GetZoom()
    {
        return rhythm.timelineZoom;
    }

    void GenerateSampleTex(int sampleQuality)
    {
        if(tex == null || tex.height != 1)
            tex = new Texture2D((int)textureRect.width, 1);

        float[] samples;
        if (rhythm.clip != null) {
            samples = new float[ClipSamples() * ClipChannels()];
            rhythm.clip.GetData(samples, 0);
        }
        else {
            samples = new float[1];
        }

         


        int startSample = SecondsToSample(PixelsToSeconds(rhythm.timelineScroll));
        int endSample = SecondsToSample(PixelsToSeconds(rhythm.timelineScroll + textureRect.width));
        int channels = ClipChannels();

        int cc = (int)Mathf.Lerp(startSample, endSample, 1 / (float)textureRect.width) - startSample;
        cc = Mathf.Min(cc, sampleQuality);

        for (int x = 0; x < textureRect.width-1; x++) {

            float fac0 = x / textureRect.width;
            float fac1 = (x+1) / textureRect.width;

            int sampleIndexLow = (int)Mathf.Lerp(startSample, endSample, fac0); //the first sample of the group
            int sampleIndexHigh = (int)Mathf.Lerp(startSample, endSample, fac1); //the last sample of the group

            Color p = new Color(0, 0, 0, 0);

            for (int c = 0; c < channels; c++) {

                int sampleIndex = sampleIndexLow; //calculates the correct sample based on quality;
                float step = (sampleIndexHigh - sampleIndexLow)/(float)cc;
                for (float i = sampleIndexLow; i <= sampleIndexHigh; i += step) {
                    try {
                        if (Mathf.Abs(samples[(int)(i * channels + c)]) > Mathf.Abs(samples[sampleIndex * channels + c])) {
                            sampleIndex = (int)i;
                        }
                    }
                    catch {}
                }
                float s = 0;
                try { s = Mathf.Abs(samples[sampleIndex * channels + c]); } catch { }
                if (c == 0) p.r = s;
                if (c == 1) p.g = s;
            }
            tex.SetPixel(x, 0, p);
        }
        tex.Apply();
    }
    //CONVERSÕES

    public int SecondsToSample(float seconds)
    {
        return (int)(seconds * (ClipSamples() / ClipLength()));
    }

    public float SampleToSeconds(int sample)
    {
        return sample / (float)ClipSamples() * ClipLength();
    }

    public float SecondsToPixels(float seconds)
    {
        return seconds / ClipLength() * (textureRect.width * rhythm.timelineZoom);
    }

    public float PixelsToSeconds(float pixels)
    {
        return pixels / (textureRect.width * rhythm.timelineZoom) * ClipLength();
    }

    float ClipLength()
    {
        if (rhythm.clip != null) {
            return rhythm.clip.length;
        }
        else return 1;
    }

    int ClipSamples()
    {
        if (rhythm.clip != null) {
            return rhythm.clip.samples;
        }
        else return 1;
    }

    int ClipChannels()
    {
        if (rhythm.clip != null) {
            return rhythm.clip.channels;
        }
        else return 0;
    }
}
#endif