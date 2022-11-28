#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MarkupDataController
{
    Rhythm rhythm;
    MarkupsDrawer markupsDrawer;
    public MarkupDataController(Rhythm rhythm, MarkupsDrawer markupsDrawer)
    {
        this.rhythm = rhythm;
        this.markupsDrawer = markupsDrawer;
    }

    public void DrawGUI(params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginVertical("Box", options);
        {
            EditorGUIUtility.labelWidth = 120;
            EditorGUILayout.LabelField("Markup data", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold });
            SerializedObject so = new SerializedObject(rhythm);

            if (markupsDrawer.GetSelectedMarkups().Count > 0) {
                Rhythm.Markup markup = markupsDrawer.GetSelectedMarkups()[0];
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.BeginVertical();

                if (GUILayout.Button("Sync")) {
                    Undo.RecordObject(rhythm, "Sync");
                    foreach(Rhythm.Markup m in markupsDrawer.GetSelectedMarkups()) {
                        m.timer = markupsDrawer.GetSelectedMarkups()[0].timer;
                    }
                }

                GUILayout.Space(20);
                RhythmUtility.BCenter();
                float updatedTimer = EditorGUILayout.FloatField("Position (in seconds)", markup.timer, GUI.skin.GetStyle("TextField"), GUILayout.Width(200));
                RhythmUtility.ECenter();

                if (updatedTimer != markup.timer) {
                    markup.timer = updatedTimer;
                    if (markupsDrawer.GetSelectedMarkups().Count > 0) {
                        foreach (Rhythm.Markup m in markupsDrawer.GetSelectedMarkups()) {
                            m.timer = updatedTimer;
                        }
                    }
                }


                RhythmUtility.BCenter();
                float updatedLength = EditorGUILayout.FloatField("Length (in seconds)", markup.length, GUI.skin.GetStyle("TextField"), GUILayout.Width(200));
                RhythmUtility.ECenter();

                if (updatedLength != markup.length) {
                    markup.length = updatedLength;
                    if (markupsDrawer.GetSelectedMarkups().Count > 0) {
                        foreach (Rhythm.Markup m in markupsDrawer.GetSelectedMarkups()) {
                            m.length = updatedLength;
                        }
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();

                EditorGUILayout.LabelField(new GUIContent("Additional parameters", "Additional parameters sent with the event"), new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                EditorGUIUtility.labelWidth = 50;

                if (markup.additionalParameters == null) markup.additionalParameters = new List<RhythmEventData.Primitive>();
                if (RhythmUtility.CenteredButton("Add parameter")) {
                    Undo.RecordObject(rhythm, "Add parameter");
                    markup.additionalParameters.Add(new RhythmEventData.Primitive() { type = 0 });
                }
                so.Update();

                int objToRemove = -1;
                for (int i = 0; i < markup.additionalParameters.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 10;
                    EditorGUILayout.LabelField(i + "", new GUIStyle() { border = new RectOffset(1, 1, 1, 1)}, GUILayout.Width(10));
                    markup.additionalParameters[i].SetValue(ObjectField(markup.additionalParameters[i].GetValue()));
                    markup.additionalParameters[i].SetType(EditorGUILayout.Popup("", markup.additionalParameters[i].type, new string[] { "int", "float", "string", "bool", "gameObject" }));
                    if (GUILayout.Button("X")) {
                        objToRemove = i;
                    }
                    GUILayout.Space(20);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                if (objToRemove != -1) {
                    Undo.RecordObject(rhythm, "Remove parameter");
                    markup.additionalParameters.RemoveAt(objToRemove);
                }

                EditorGUIUtility.labelWidth = 200;

                if (GUI.changed) {
                    so.ApplyModifiedProperties();
                }

                if (markup.length < 0) markup.length = 0;

            }
        }
        EditorGUILayout.EndVertical();

    }

    public void UpdateReference(Rhythm rhythm)
    {
        this.rhythm = rhythm;
    }

    SerializedProperty GetSerializedProperty(Rhythm.Markup markup, SerializedObject so)
    {
        int layer = RhythmUtility.GetLayerIndexFromMarkup(rhythm, markup);
        int mk = rhythm.layers[layer].markups.IndexOf(markup);
        return so.FindProperty("layers").GetArrayElementAtIndex(layer).FindPropertyRelative("markups").GetArrayElementAtIndex(mk);
    }

    object ObjectField(object obj)
    {
        if (typeof(int).IsInstanceOfType(obj)) {
            return EditorGUILayout.IntField((int)obj);
        }
        else if (typeof(float).IsInstanceOfType(obj)) {
            return EditorGUILayout.FloatField((float)obj);
        }
        else if (typeof(string).IsInstanceOfType(obj)) {
            return EditorGUILayout.TextField((string)obj);
        }
        else if (typeof(bool).IsInstanceOfType(obj)) {
            return EditorGUILayout.Toggle((bool)obj);
        }
        return EditorGUILayout.ObjectField((GameObject)obj, typeof(GameObject), false);
    }
}
#endif