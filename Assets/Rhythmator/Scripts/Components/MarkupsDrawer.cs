#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class MarkupsDrawer
{
    class NullableRect
    {
        public Rect rect;
    }

    NullableRect selectionRect;

    WaveformDrawer waveformDrawer;
    BPMController bpmController;
    LayerController layerController;

    Rhythm rhythm;
    List<Rhythm.Markup> selectedMarkups;

    float snappedPosition;

    Rhythm.Markup draggingMarkup;

    public MarkupsDrawer(Rhythm rhythm, WaveformDrawer waveformDrawer, BPMController bpmController, LayerController layerController, TimelineController timelineController)
    {
        this.rhythm = rhythm;
        this.waveformDrawer = waveformDrawer;
        this.bpmController = bpmController;
        this.layerController = layerController;

        selectedMarkups = new List<Rhythm.Markup>();
    }

    public void UpdateReference(Rhythm rhythm)
    {
        this.rhythm = rhythm;
    }

    public void SelectMarkup(Rhythm.Markup markup)
    {
        selectedMarkups.Clear();
        selectedMarkups.Add(markup);
    }

    public Rect GetMarkupRect(Rhythm.Markup markup)
    {
        List<Rhythm.MarkupLayer> layers = layerController.VisibleLayers();

        float height = waveformDrawer.GetRect().height / layers.Count;
        float init = waveformDrawer.SecondsToPixels(markup.timer) - waveformDrawer.GetScroll();
        float end = waveformDrawer.SecondsToPixels(markup.timer + markup.length) - waveformDrawer.GetScroll();
        Rect r2;

        Rhythm.MarkupLayer layer = RhythmUtility.GetLayerFromMarkup(rhythm, markup);
        int idx = layers.IndexOf(layer);

        if (idx != -1) {
            if (markup.length > 0)
                r2 = RhythmUtility.CreateRect(init, layers.IndexOf(layer) * height + waveformDrawer.GetRect().y, end - init, height);
            else
                r2 = RhythmUtility.CreateRect(init - 1, layers.IndexOf(layer) * height + waveformDrawer.GetRect().y, 2, height);
            return r2;
        }

        return RhythmUtility.CreateRect(0, 0, -1, -1);
    }

    public Rect GetLeftMarkupRect(Rhythm.Markup markup)
    {
        if(markup.length > 0) {
            Rect r = GetMarkupRect(markup);
            return RhythmUtility.CreateRect(r.x, r.y, 5, r.height);
        }
        return RhythmUtility.CreateRect(0, 0, 0, 0);
    }

    public Rect GetRightMarkupRect(Rhythm.Markup markup)
    {
        if (markup.length > 0) {
            Rect r = GetMarkupRect(markup);
            return RhythmUtility.CreateRect(r.x + r.width-5, r.y, 5, r.height);
        }
        return RhythmUtility.CreateRect(0, 0, 0, 0);
    }

    int NearestExp2(float x)
    {
        int exp = 1;
        while(Mathf.Pow(2, exp) < x) {
            exp++;
        }
        return (int)Mathf.Pow(2, exp);
    }

    public int GetBPMCount(float seconds, bool ceil)
    {
        float bpm = rhythm.BPM * NearestExp2(waveformDrawer.GetZoom()) / NearestExp2(rhythm.clip.length) * 16;
        float spb = 60 / bpm; //Seconds per beat
        if(ceil)
            return (int)Mathf.Ceil(Mathf.Max(0, seconds - rhythm.BPMdelay) / spb);
        return (int)Mathf.Floor(Mathf.Max(0, seconds - rhythm.BPMdelay) / spb);

    }

    public float GetBPMPosition(int beat)
    {
        float bpm = rhythm.BPM * NearestExp2(waveformDrawer.GetZoom()) / NearestExp2(rhythm.clip.length) * 16;
        float spb = 60 / bpm; //Seconds per beat
        return rhythm.BPMdelay + spb * beat;
    }

    public void DrawGUI()
    {
        //Sort markups
        foreach (Rhythm.MarkupLayer l in layerController.VisibleLayers()) {
            l.markups.Sort((a, b) => (int)((a.timer - b.timer)*100f));
        }

        //Desenha os BPM
        if (rhythm.BPMenabled) {

            float bpm = rhythm.BPM * NearestExp2(waveformDrawer.GetZoom()) / NearestExp2(rhythm.clip.length) * 16;

            float spb = 60 / bpm; //Seconds per beat
            float pixelDelay = waveformDrawer.SecondsToPixels(rhythm.BPMdelay);
            float pixelSpace = waveformDrawer.SecondsToPixels(spb);

            float f0 = waveformDrawer.PixelsToSeconds(waveformDrawer.GetScroll());
            float f1 = waveformDrawer.PixelsToSeconds(waveformDrawer.GetScroll() + waveformDrawer.GetRect().width);

            int b0 = GetBPMCount(f0, true);
            int b1 = GetBPMCount(f1, true);

            int local = b1 - b0;

            for(int i = 0; i < local; i++) {
                float beatPixel = pixelDelay + pixelSpace * (i + b0) - waveformDrawer.GetScroll();
                EditorGUI.DrawRect(RhythmUtility.CreateRect(beatPixel, waveformDrawer.GetRect().y, 2, waveformDrawer.GetRect().height), Color.cyan);
            }
        }

        //Draw markups
        foreach (Rhythm.MarkupLayer l in layerController.VisibleLayers()) {

            int m0 = search(l, waveformDrawer.PixelsToSeconds(waveformDrawer.GetScroll()));
            int m1 = search(l, waveformDrawer.PixelsToSeconds(waveformDrawer.GetScroll() + waveformDrawer.GetRect().width));

            if (l.markups.Count == 0) continue;
            for (int i = m0; i <= m1; i++) {
                Rhythm.Markup m = l.markups[i];

                //Voltar aqui dps dos controles de BPM
                Rect markRect = GetMarkupRect(m);
                if (markRect.width != -1 && markRect.Overlaps(waveformDrawer.GetRect())) {
                    //Draw
                    Color cc = new Color(l.color.r, l.color.g, l.color.b, 0.7f);
                    if (selectedMarkups.Contains(m)) {
                        cc = Color.Lerp(cc, Color.white, 0.5f);
                    }

                    if (m == draggingMarkup && rhythm.BPMenabled && bpmController.SnapToBpm()) {
                        markRect = RhythmUtility.CreateRect(snappedPosition, markRect.y, markRect.width, markRect.height);
                    }
                    EditorGUI.DrawRect(markRect, cc);

                    if(m.length > 0) {
                        EditorGUI.DrawRect(RhythmUtility.CreateRect(markRect.x, markRect.y, 1, markRect.height), Color.blue);
                        EditorGUI.DrawRect(RhythmUtility.CreateRect(markRect.x + markRect.width-1, markRect.y, 1, markRect.height), Color.blue);

                        EditorGUI.DrawRect(RhythmUtility.CreateRect(markRect.x, markRect.y, 5, 2), Color.blue);
                        EditorGUI.DrawRect(RhythmUtility.CreateRect(markRect.x, markRect.y+ markRect.height-2, 5, 2), Color.blue);

                        EditorGUI.DrawRect(RhythmUtility.CreateRect(markRect.x + markRect.width-5, markRect.y, 5, 2), Color.blue);
                        EditorGUI.DrawRect(RhythmUtility.CreateRect(markRect.x + markRect.width - 5, markRect.y + markRect.height - 2, 5, 2), Color.blue);

                        EditorGUI.DrawRect(RhythmUtility.CreateRect(markRect.x, markRect.y, 5, markRect.height), new Color(0, 0, 1, 0.2f));
                        EditorGUI.DrawRect(RhythmUtility.CreateRect(markRect.x+markRect.width - 5, markRect.y, 5, markRect.height), new Color(0, 0, 1, 0.2f));
                    }
                }

            }
        }
        //Desenha a agulha
        EditorGUI.DrawRect(RhythmUtility.CreateRect(waveformDrawer.SecondsToPixels(rhythm.needlePosition) - waveformDrawer.GetScroll(), waveformDrawer.GetRect().y, 2, waveformDrawer.GetRect().height), Color.red);

        //Desenha a caixa de seleção, se existir
        if (selectionRect != null) {
            EditorGUI.DrawRect(selectionRect.rect, new Color(0.4f, 0.5f, 0.7f, 0.5f));
        }
    }

    int search(Rhythm.MarkupLayer l, float timer)
    {
        if (l.markups.Count == 0) return -1;
        if (timer < l.markups[0].timer+l.markups[0].length)
            return 0;
        if (timer > l.markups[l.markups.Count - 1].timer + l.markups[l.markups.Count - 1].length)
            return l.markups.Count - 1;

        int lo = 0;
        int hi = l.markups.Count - 1;
        while (lo <= hi) {
            int mid = (hi + lo) / 2;
            if (timer < l.markups[mid].timer + l.markups[mid].length)
                hi = mid - 1;
            else if (timer > l.markups[mid].timer + l.markups[mid].length)
                lo = mid + 1;
            else
                return mid;
        }
        return (l.markups[lo].timer + l.markups[lo].length - timer) < (timer - (l.markups[hi].timer + l.markups[hi].length)) ? lo : hi;
    }

    public List<Rhythm.Markup> GetSelectedMarkups()
    {
        return selectedMarkups;
    }

    public float GetNeedle()
    {
        return rhythm.needlePosition;
    }

    public void SetNeedle(float needle)
    {
        this.rhythm.needlePosition = needle;
    }

    public Rect GetSelectionRect()
    {
        return selectionRect.rect;
    }

    public void SetSelectionRect(float x, float y, float width, float height)
    {
        if (selectionRect == null)
            selectionRect = new NullableRect();
        selectionRect.rect.Set(x, y, width, height);
    }
    public void SetSelectionRect(Vector2 position, Vector2 size)
    {
        SetSelectionRect(position.x, position.y, size.x, size.y);
    }

    public bool HasSelectionRect()
    {
        return selectionRect != null;
    }

    public void DestroySelectionRect()
    {
        selectionRect = null;
    }

    public Rhythm.Markup GetDraggingMarkup()
    {
        return draggingMarkup;
    }

    public void SetDraggingMarkup(Rhythm.Markup markup)
    {
        draggingMarkup = markup;
    }

    public float GetSnappedPosition()
    {
        return snappedPosition;
    }

    public void SetSnappedPosition(float sp)
    {
        snappedPosition = sp;
    }

    public int LayerHover()
    {
        int vi = (int)((Event.current.mousePosition.y - waveformDrawer.GetRect().y) / waveformDrawer.GetRect().height * layerController.VisibleLayers().Count);
        if(vi < layerController.VisibleLayers().Count)
            return rhythm.layers.IndexOf(layerController.VisibleLayers()[vi]);

        return -1;
    }


}
#endif