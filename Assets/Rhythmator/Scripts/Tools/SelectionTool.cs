#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
public class SelectionTool : RhythmTool
{
    
    Vector2 selectionOrigin;

    //Dragging
    float draggingMarkupOffset;
    List<float> selectedMarkupsOffset;
    bool lastRightClickWasDragged;

    List<Rhythm.Markup> markupsCopied;

    Rhythm.Markup changingLength;
    int side = 0;

    public SelectionTool(RhythmEditor editor) : base(editor)
    {
        selectedMarkupsOffset = new List<float>();
        markupsCopied = new List<Rhythm.Markup>();
    }

    // This is called for each window that your tool is active in. Put the functionality of your tool here.
    public override void GUI(EditorWindow window)
    {
        WaveformDrawer wd = RhythmEditor.timelineController.GetWaveformDrawer();
        MarkupsDrawer md = RhythmEditor.timelineController.GetMarkupsDrawer();

        if (Event.current.type == EventType.MouseDown && Event.current.button == 1) {
            lastRightClickWasDragged = false;
        }

        if (Event.current.type == EventType.MouseDrag && Event.current.button == 1) {
            lastRightClickWasDragged = true;
        }


        //Selection rect
        if (wd.GetRect().Contains(Event.current.mousePosition)) {
            if (Event.current.button == 0) {

                Rhythm.Markup over = null;
                Rect leftSide = RhythmUtility.CreateRect(0, 0, 0, 0);
                Rect rightSide = RhythmUtility.CreateRect(0, 0, 0, 0);
                if (md.LayerHover() != -1) {
                    foreach (Rhythm.Markup m in editor.rhythm.layers[md.LayerHover()].markups) {
                        if (md.GetLeftMarkupRect(m).Contains(Event.current.mousePosition)) {
                            leftSide = md.GetLeftMarkupRect(m);
                        }
                        if (md.GetRightMarkupRect(m).Contains(Event.current.mousePosition)) {
                            rightSide = md.GetRightMarkupRect(m);
                        }
                        Rect r = md.GetMarkupRect(m);
                        if (m.length == 0) {
                            r.x -= 5;
                            r.width += 10;
                        }
                        if (r.Contains(Event.current.mousePosition)) {
                            over = m;
                            break;
                        }
                    }
                }
                
                if (Event.current.type == EventType.MouseDown) {
                    if (over == null) {
                        UnityEngine.GUI.FocusControl(null);
                        selectionOrigin = Event.current.mousePosition;
                        md.SetSelectionRect(Event.current.mousePosition, new Vector2(0, 0));
                    }
                    else if(leftSide.width == 0 && rightSide.width == 0){
                        if (!md.GetSelectedMarkups().Contains(over)) {
                            md.SelectMarkup(over);
                        }
                        //Begin drag
                        if (md.GetDraggingMarkup() == null && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)) {
                            UnityEngine.GUI.FocusControl(null);
                            md.SetDraggingMarkup(over);
                            draggingMarkupOffset = (Event.current.mousePosition.x + wd.GetScroll()) - wd.SecondsToPixels(over.timer);
                            md.SetSnappedPosition(wd.SecondsToPixels(md.GetDraggingMarkup().timer) - wd.GetScroll());
                            selectedMarkupsOffset.Clear();
                            foreach (Rhythm.Markup s in md.GetSelectedMarkups()) {
                                selectedMarkupsOffset.Add(over.timer - s.timer);
                            }
                            Undo.RecordObject(editor.rhythm, "Move Markups");
                        }
                    }
                    else {
                        changingLength = over;
                        if (leftSide.width > 0)
                            side = -1;
                        else if (rightSide.width > 0)
                            side = 1;
                        Undo.RecordObject(editor.rhythm, "Change markup length");
                    }

                }
                //Dragging
                if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) {
                    if (md.HasSelectionRect()) {
                        UnityEngine.GUI.FocusControl(null);
                        Vector2 diff = Event.current.mousePosition - selectionOrigin;
                        md.SetSelectionRect(selectionOrigin.x, selectionOrigin.y, diff.x, diff.y);

                        if (diff.x < 0)
                            md.SetSelectionRect(Event.current.mousePosition.x, md.GetSelectionRect().y, -diff.x, md.GetSelectionRect().height);
                        if (diff.y < 0)
                            md.SetSelectionRect(md.GetSelectionRect().x, Event.current.mousePosition.y, md.GetSelectionRect().width, -diff.y);

                        //Seleciona os q tão intersectando
                        md.GetSelectedMarkups().Clear();
                        for (int j = 0; j < editor.rhythm.layers.Count; j++) {
                            Rhythm.MarkupLayer l = editor.rhythm.layers[j];
                            for (int i = 0; i < l.markups.Count; i++) {
                                Rhythm.Markup m = l.markups[i];
                                Rect mRect = md.GetMarkupRect(m);
                                if (mRect.width != -1) {
                                    if (md.GetSelectionRect().Overlaps(mRect)) {
                                        md.GetSelectedMarkups().Add(m);
                                    }
                                }
                            }
                        }
                    }

                    if(md.GetDraggingMarkup() != null) {
                        UnityEngine.GUI.FocusControl(null);
                        //Voltar aqui dps do controle de BPM
                        if (editor.rhythm.BPMenabled && RhythmEditor.bpmController.SnapToBpm()) {
                            md.SetSnappedPosition(wd.SecondsToPixels(md.GetDraggingMarkup().timer) - wd.GetScroll());
                            
                            int low = md.GetBPMCount(md.GetDraggingMarkup().timer, false);
                            int high = md.GetBPMCount(md.GetDraggingMarkup().timer, true);

                            if (Mathf.Abs(wd.SecondsToPixels(md.GetBPMPosition(low)) - wd.SecondsToPixels(md.GetDraggingMarkup().timer)) < 10) {
                                md.SetSnappedPosition(wd.SecondsToPixels(md.GetBPMPosition(low)) - wd.GetScroll());
                            }
                            if (Mathf.Abs(wd.SecondsToPixels(md.GetBPMPosition(high)) - wd.SecondsToPixels(md.GetDraggingMarkup().timer)) < 10) {
                                md.SetSnappedPosition(wd.SecondsToPixels(md.GetBPMPosition(high)) - wd.GetScroll());
                            }
                        }
                        md.GetDraggingMarkup().timer = wd.PixelsToSeconds(Event.current.mousePosition.x - draggingMarkupOffset + wd.GetScroll());

                        for (int i = 0; i < md.GetSelectedMarkups().Count; i++) {
                            Rhythm.Markup ss = md.GetSelectedMarkups()[i];
                            float offset = selectedMarkupsOffset[i];
                            ss.timer = wd.PixelsToSeconds(Event.current.mousePosition.x - draggingMarkupOffset + wd.GetScroll()) - offset;
                        }
                    }
                    //Arrastando o tamanho pelo lado direito
                    else if(side == 1 && changingLength != null) {
                        float dx = wd.PixelsToSeconds(Event.current.mousePosition.x + wd.GetScroll()) - changingLength.timer;
                        dx = Mathf.Max(dx, 0);
                        changingLength.length = dx;
                    }
                    //Arrastando o tamanho pelo lado esquerdo
                    else if(side == -1 && changingLength != null) {
                        float dx = (changingLength.timer+changingLength.length) - wd.PixelsToSeconds(Event.current.mousePosition.x + wd.GetScroll());
                        dx = Mathf.Max(dx, 0);
                        changingLength.length = dx;
                        if(dx != 0)
                        changingLength.timer = wd.PixelsToSeconds(Event.current.mousePosition.x + wd.GetScroll());
                    }
                }
            }
            if (Event.current.type == EventType.MouseUp) {
                md.DestroySelectionRect();
                if (md.GetDraggingMarkup() != null) {
                    if (editor.rhythm.BPMenabled && RhythmEditor.bpmController.SnapToBpm())
                        md.GetDraggingMarkup().timer = wd.PixelsToSeconds(md.GetSnappedPosition() + wd.GetScroll());
                    md.SetDraggingMarkup(null);
                }
                changingLength = null;
                side = 0;

                if (Event.current.button == 1 && !lastRightClickWasDragged) {
                    if (md.GetSelectedMarkups().Count > 0) {
                        Rhythm.Markup m = md.GetSelectedMarkups()[md.GetSelectedMarkups().Count - 1];
                        md.GetSelectedMarkups().Clear();
                        md.GetSelectedMarkups().Add(m);
                    }

                }
            }
        }
        CopyPasteDeleteInput();
    }

    public void CopyPasteDeleteInput()
    {
        WaveformDrawer wd = RhythmEditor.timelineController.GetWaveformDrawer();
        MarkupsDrawer md = RhythmEditor.timelineController.GetMarkupsDrawer();
        //Copy selected markups
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C && Event.current.control) {
            if (md.GetSelectedMarkups().Count > 0) {
                markupsCopied.Clear();
                foreach (Rhythm.Markup markup in md.GetSelectedMarkups()) {
                    if (!markupsCopied.Contains(markup)) {
                        markupsCopied.Add(markup);
                    }
                }
            }
        }
        //Paste copied markups
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.V && Event.current.control) {
            if (markupsCopied.Count > 0) {
                //Pega o primeiro markup
                Rhythm.Markup lowestMarkup = markupsCopied[0];
                for (int i = 1; i < markupsCopied.Count; i++) {
                    if (markupsCopied[i].timer < lowestMarkup.timer) {
                        lowestMarkup = markupsCopied[i];
                    }
                }
                md.GetSelectedMarkups().Clear();
                Undo.RecordObject(editor.rhythm, "Paste markups");
                foreach (Rhythm.Markup m in markupsCopied) {
                    float offset = m.timer - lowestMarkup.timer;

                    List<RhythmEventData.Primitive> objs = new List<RhythmEventData.Primitive>();
                    foreach (RhythmEventData.Primitive p in m.additionalParameters) {
                        objs.Add(p.Clone());
                    }

                    Rhythm.Markup cpy = new Rhythm.Markup() {
                        timer = md.GetNeedle() + offset,
                        length = m.length,
                        additionalParameters = objs,
                    };

                    RhythmUtility.GetLayerFromMarkup(editor.rhythm, m).markups.Add(cpy);
                    md.GetSelectedMarkups().Add(cpy);
                }
            }
        }

        //Delete selected markups
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete) {
            Undo.RecordObject(editor.rhythm, "Remove Markups");
            if (md.GetSelectedMarkups().Count > 0) {
                md.GetSelectedMarkups().Sort((a, b) => (int)((b.timer - a.timer) * 100));
                foreach (Rhythm.Markup m in md.GetSelectedMarkups()) {
                    RhythmUtility.GetLayerFromMarkup(editor.rhythm, m).markups.Remove(m);
                }
                md.GetSelectedMarkups().Clear();
            }
        }
    }


    public void IterateOverMarkups(Func<Rhythm.Markup, bool> a)
    {
        foreach(Rhythm.MarkupLayer l in editor.rhythm.layers) {
            foreach(Rhythm.Markup m in l.markups) {
                if (a(m)) {
                    break;
                }
            }
        }
    }
}
#endif