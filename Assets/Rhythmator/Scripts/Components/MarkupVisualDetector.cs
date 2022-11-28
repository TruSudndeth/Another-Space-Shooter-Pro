#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MarkupVisualDetector
{

    Rhythm rhythm;
    SongController songController;
    MarkupsDrawer markupsDrawer;
    List<Rect> layerRects;
    Color gray;
    List<float> currentAlpha;

    [HideInInspector]
    public List<Rhythm.Markup> processed;
    [HideInInspector]
    public List<Rhythm.Markup> processedLong;

    public MarkupVisualDetector(Rhythm rhythm, SongController songController, MarkupsDrawer markupsDrawer)
    {
        this.songController = songController;
        this.markupsDrawer = markupsDrawer;
        this.rhythm = rhythm;
        layerRects = new List<Rect>();
        gray = new Color(194 / 255f, 194 / 255f, 194 / 255f);
        currentAlpha = new List<float>();
        processed = new List<Rhythm.Markup>();
        processedLong = new List<Rhythm.Markup>();

    }

    public void UpdateReference(Rhythm rhythm)
    {
        this.rhythm = rhythm;
    }

    public void DrawGUI()
    {

        GUILayout.BeginHorizontal();

        //sinca os target alpha com a lista de layer
        while (currentAlpha.Count < rhythm.layers.Count) {
            currentAlpha.Add(0);
        }
        while(currentAlpha.Count > rhythm.layers.Count) {
            currentAlpha.RemoveAt(currentAlpha.Count - 1);
        }

        for(int i = 0; i < currentAlpha.Count; i++) {
            currentAlpha[i] += (0 - currentAlpha[i]) / 10f;
        }

        Update();

        layerRects.Clear();
        for(int i = 0; i < rhythm.layers.Count; i++) {
            Rect r = GUILayoutUtility.GetRect(40, 40);
            layerRects.Add(r);

            r = new Rect(r.x + 2, r.y + 2, r.width - 4, r.height - 4);

            Rect pr = new Rect(r);
            pr.y = r.y + currentAlpha[i] * 20;

            if (songController.IsPlaying()) {
                EditorGUI.DrawTextureTransparent(pr, RhythmUtility.MakeTex(1, 1, rhythm.layers[i].color));
            }
            else {
                EditorGUI.DrawTextureTransparent(pr, RhythmUtility.MakeTex(1, 1, rhythm.layers[i].color));
            }
        }

        GUILayout.EndHorizontal();

    }

    public void Update()
    {
        if (songController.IsPlaying()) {
            //MARCAÇÕES
            int l = 0;
            foreach (Rhythm.MarkupLayer layer in rhythm.layers) {
                foreach (Rhythm.Markup markup in layer.markups) {
                    if (markup.length > 0) {
                        //Hold
                        if (markupsDrawer.GetNeedle() >= markup.timer && !processed.Contains(markup)) {
                            processed.Add(markup);
                        }
                        if (markupsDrawer.GetNeedle() >= markup.timer && markupsDrawer.GetNeedle() < markup.timer + markup.length) {
                            currentAlpha[l] = 1;
                        }
                        if (markupsDrawer.GetNeedle() >= markup.timer + markup.length && !processedLong.Contains(markup)) {
                            processedLong.Add(markup);
                        }
                    }
                    else {
                        //Hit
                        if (markupsDrawer.GetNeedle() >= markup.timer && !processed.Contains(markup)) {
                            currentAlpha[l] = 1;
                            processed.Add(markup);
                        }
                    }
                }
                l++;
            }
        }
    }

    public void NotifySongStart()
    {
        for (int i = 0; i < currentAlpha.Count; i++) {
            currentAlpha[i] = 0;
        }

        processed.Clear();
        processedLong.Clear();

        foreach (Rhythm.MarkupLayer layer in rhythm.layers) {
            foreach (Rhythm.Markup markup in layer.markups) {
                if (markupsDrawer.GetNeedle() > markup.timer) {
                    processed.Add(markup);
                }
                if (markup.length > 0) {
                    if(markupsDrawer.GetNeedle() > markup.timer + markup.length) {
                        processedLong.Add(markup);
                    }
                }
            }
        }
    }

}
#endif