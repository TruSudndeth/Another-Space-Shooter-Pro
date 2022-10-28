#if UNITY_EDITOR
using UnityEditor;
public abstract class RhythmTool
{
    protected RhythmEditor editor;
    public RhythmTool(RhythmEditor editor)
    {
        this.editor = editor;
    }
    public abstract void GUI(EditorWindow window);
}
#endif