#if UNITY_EDITOR
using UnityEditor;

public class BPMController
{
    Rhythm rhythm;
    public BPMController(Rhythm rhythm)
    {
        this.rhythm = rhythm;
    }

    public void UpdateReference(Rhythm rhythm)
    {
        this.rhythm = rhythm;
    }

    public void DrawGUI()
    {
        //BPM controls
        RhythmUtility.BCenter();
        rhythm.BPMenabled = EditorGUILayout.Toggle("Enable BPM?", rhythm.BPMenabled);
        RhythmUtility.ECenter();

        if (rhythm.BPMenabled) {
            RhythmUtility.BCenter();
            rhythm.BPM = EditorGUILayout.FloatField("BPM", rhythm.BPM);
            RhythmUtility.ECenter();

            RhythmUtility.BCenter();
            rhythm.BPMdelay = EditorGUILayout.FloatField("Delay", rhythm.BPMdelay);
            RhythmUtility.ECenter();

            RhythmUtility.BCenter();
            rhythm.snapToBPM = EditorGUILayout.Toggle("Snap to BPM?", rhythm.snapToBPM);
            RhythmUtility.ECenter();
        }

    }

    public bool SnapToBpm()
    {
        return rhythm.snapToBPM;
    }

}
#endif