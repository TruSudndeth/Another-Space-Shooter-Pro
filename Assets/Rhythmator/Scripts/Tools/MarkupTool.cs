#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MarkupTool : RhythmTool
{

    Rhythm.Markup beignCreated;
    float initialCreationPosition;

    public MarkupTool(RhythmEditor editor) :  base(editor)
    {
    }

    // This is called for each window that your tool is active in. Put the functionality of your tool here.
    public override void GUI(EditorWindow window)
    {
        EditorGUIUtility.AddCursorRect(RhythmEditor.timelineController.GetWaveformDrawer().GetRect(), Event.current.control ? MouseCursor.ArrowMinus : MouseCursor.ArrowPlus);

        WaveformDrawer wd = RhythmEditor.timelineController.GetWaveformDrawer();
        MarkupsDrawer md = RhythmEditor.timelineController.GetMarkupsDrawer();

        if (wd.GetRect().Contains(Event.current.mousePosition)) {
            if (Event.current.button == 0) {
                if (!Event.current.control) {
                    if (Event.current.type == EventType.MouseDown) {
                        if (editor.rhythm.layers.Count == 0) {
                            editor.rhythm.layers.Add(new Rhythm.MarkupLayer() {
                                layerName = "Default",
                                color = Color.blue,
                                visible = true,
                                markups = new List<Rhythm.Markup>()
                            });
                            Debug.Log("Non existing layer, creating");
                        }
                        Undo.RecordObject(editor.rhythm, "Add markup");
                        Rhythm.Markup m = new Rhythm.Markup() { timer = wd.PixelsToSeconds(Event.current.mousePosition.x + wd.GetScroll()), length = 0, additionalParameters = new List<RhythmEventData.Primitive>() };
                        editor.rhythm.layers[md.LayerHover()].markups.Add(m);

                        editor.rhythm.layers[md.LayerHover()].markups.Sort((a, b) => (int)((a.timer - b.timer) * 100f));

                        beignCreated = m;
                        initialCreationPosition = m.timer;
                        Debug.Log("Created markup at position " + m.timer);
                    }

                    if (Event.current.type == EventType.MouseDrag) {
                        if (beignCreated != null) {
                            float at = wd.PixelsToSeconds(Event.current.mousePosition.x + wd.GetScroll());
                            if (at > initialCreationPosition) {
                                beignCreated.length = at - initialCreationPosition;
                            }
                            else {
                                beignCreated.length = initialCreationPosition - at;
                                beignCreated.timer = at;
                            }
                        }
                    }
                }
                else {
                    if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) {
                        Rhythm.Markup over = null;
                        if (md.LayerHover() != -1) {
                            foreach (Rhythm.Markup m in editor.rhythm.layers[md.LayerHover()].markups) {
                                if (md.GetMarkupRect(m).Contains(Event.current.mousePosition)) {
                                    over = m;
                                    break;
                                }
                            }
                            Undo.RecordObject(editor.rhythm, "Remove Markups");
                            editor.rhythm.layers[md.LayerHover()].markups.Remove(over);
                        }


                    }
                }
            }
        }
    }
}
#endif