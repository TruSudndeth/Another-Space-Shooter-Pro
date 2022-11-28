#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public class RhythmUtility
{
    static Rect auxRect;

    public static Rhythm ReadMidi(Rhythm rhythm, Object midiAsset)
    {
        MidiFileParser.Parse(rhythm, midiAsset);
        return rhythm;
    }

    public static bool ToggleButton(bool value, GUIContent content, params GUILayoutOption[] options)
    {
        GUIStyle ToggleButtonStyleNormal = GUI.skin.button;
        GUIStyle ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
        ToggleButtonStyleToggled.normal.background = ToggleButtonStyleNormal.active.background;
        return GUILayout.Button(content, value ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, options);
    }

    public static bool CenteredButton(string name, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool b = GUILayout.Button(name, style, options);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        return b;
    }

    public static bool CenteredButton(string name, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool b = GUILayout.Button(name, options);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        return b;
    }

    public static void BCenter(params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal(options);
        GUILayout.FlexibleSpace();
    }

    public static void ECenter()
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    public static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    public static Rect CreateRect(float x, float y, float width, float height)
    {
        if (auxRect == null) auxRect = new Rect(0, 0, 0, 0);
        auxRect.Set(x, y, width, height);
        return auxRect;
    }

    public static Rect CreateRect(Vector2 position, Vector2 size)
    {
        if (auxRect == null) auxRect = new Rect(0, 0, 0, 0);
        auxRect.Set(position.x, position.y, size.x, size.y);
        return auxRect;
    }

    public static Rhythm.MarkupLayer GetLayerFromMarkup(Rhythm rhythm, Rhythm.Markup markup)
    {
        int layerIndex = -1;
        for (int i = 0; i < rhythm.layers.Count; i++) {
            int m = rhythm.layers[i].markups.IndexOf(markup);
            if (m != -1) {
                layerIndex = i;
                break;
            }
        }
        if (layerIndex == -1) throw new System.Exception("Could not find layer for specified markup");
        return rhythm.layers[layerIndex];
    }

    public static int GetLayerIndexFromMarkup(Rhythm rhythm, Rhythm.Markup markup)
    {
        int layerIndex = -1;
        for (int i = 0; i < rhythm.layers.Count; i++) {
            int m = rhythm.layers[i].markups.IndexOf(markup);
            if (m != -1) {
                layerIndex = i;
                break;
            }
        }
        if (layerIndex == -1) throw new System.Exception("Could not find layer for specified markup");
        return layerIndex;
    }

    static long startMeasure = 0;

    static long CurrentSpan()
    {
        return ((System.DateTimeOffset)(System.DateTime.UtcNow)).ToUnixTimeMilliseconds();
    }

    public static void StartFrameMeasure()
    {
        startMeasure = CurrentSpan();
    }

    public static long EndFrameMeasure()
    {
        return CurrentSpan() - startMeasure;
    }

}
#endif