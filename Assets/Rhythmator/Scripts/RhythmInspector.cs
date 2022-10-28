#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rhythm))]
public class RhythmInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Use the Rhythm Editor to edit this object");
        if(GUILayout.Button("Open Rhythm Editor")) {
            RhythmEditor.ShowWindow();
        }
    }
}
#endif