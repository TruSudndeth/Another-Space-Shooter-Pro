#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class DefaultControls
{
    RhythmEditor editor;

    float initialScrollPosition;
    float initialMousePosition;
    bool draggingScroll;

    public DefaultControls(RhythmEditor editor)
    {
        this.editor = editor;
    }

    public void Update()
    {
        WaveformDrawer wd = RhythmEditor.timelineController.GetWaveformDrawer();
        MarkupsDrawer md = RhythmEditor.timelineController.GetMarkupsDrawer();

        if (Event.current.type == EventType.ScrollWheel) {
            if (Event.current.delta.y > 0) {
                if (!Event.current.control)
                    wd.SetZoom(wd.GetZoom() / 1.1f);
                else
                    wd.SetScroll(wd.GetScroll() +  500);
            }
            else if (Event.current.delta.y < 0) {
                if (!Event.current.control)
                    wd.SetZoom(wd.GetZoom() * 1.1f);
                else
                    wd.SetScroll(wd.GetScroll() - 500);
            }
        }

        if(Event.current.type == EventType.KeyDown) {
            if(Event.current.keyCode == KeyCode.Space) {
                if (RhythmEditor.timelineController.GetSongController().IsPlaying()) {
                    RhythmEditor.timelineController.GetSongController().PauseAudio();
                }
                else {
                    RhythmEditor.timelineController.GetSongController().PlayAudio();
                }
            }
            if(Event.current.keyCode == KeyCode.RightArrow) {
                md.SetNeedle(md.GetNeedle() + 1);
                AudioUtility.SetClipSamplePosition(editor.rhythm.clip, wd.SecondsToSample(md.GetNeedle())); ;
                RhythmEditor.timelineController.NotifySongStart();
            }
            if(Event.current.keyCode == KeyCode.LeftArrow) {
                md.SetNeedle(md.GetNeedle() - 1);
                AudioUtility.SetClipSamplePosition(editor.rhythm.clip, wd.SecondsToSample(md.GetNeedle())); ;
                RhythmEditor.timelineController.NotifySongStart();
            }
            if(Event.current.keyCode == KeyCode.A) {
                RhythmEditor.selectedTool = RhythmEditor.selectionTool;
            }
            if(Event.current.keyCode == KeyCode.S) {
                RhythmEditor.selectedTool = RhythmEditor.markupTool;
            }
        }

        if (wd.GetRect().Contains(Event.current.mousePosition)) {
            //Drag scroll with middle mouse button
            if (Event.current.button == 2) {
                if (Event.current.type == EventType.MouseDown) {
                    initialScrollPosition = wd.GetScroll();
                    initialMousePosition = Event.current.mousePosition.x;
                    draggingScroll = true;
                }
                if(Event.current.type == EventType.MouseDrag) {
                    if (draggingScroll) {
                        float dx = Event.current.mousePosition.x - initialMousePosition;
                        wd.SetScroll(initialScrollPosition - dx);
                    }
                }
                if (Event.current.type == EventType.MouseUp) {
                    draggingScroll = false;
                }
            }
            //resto
            //Seta a agulha pro click do mouse
            if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) && Event.current.button == 1) {
                GUI.FocusControl(null);
                md.SetNeedle(wd.PixelsToSeconds(Event.current.mousePosition.x + wd.GetScroll()));
                if (AudioUtility.IsClipPlaying(editor.rhythm.clip)) {
                    AudioUtility.SetClipSamplePosition(editor.rhythm.clip, wd.SecondsToSample(editor.rhythm.needlePosition));
                    RhythmEditor.timelineController.NotifySongStart();
                }
            }
        }
    }
}
#endif