using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class LayerController
{
    Rhythm rhythm;
    int selectedLayer;
    int renamingIndex;
    Vector2 layerScroll;
    Texture2D highlightTex;
    SerializedProperty layerProperty;
    Texture2D hideIcon;

    public LayerController(Rhythm rhythm)
    {
        this.rhythm = rhythm;
        selectedLayer = -1;
        renamingIndex = -1;
        hideIcon = Resources.Load("ocultar") as Texture2D;
    }

    void Inputs()
    {

    }

    public void DrawGUI(params GUILayoutOption[] options)
    {

        Inputs();

        if (highlightTex == null)
            highlightTex = RhythmUtility.MakeTex(1, 1, Color.yellow);

        layerProperty = new SerializedObject(rhythm).FindProperty("layers");

        EditorGUILayout.BeginVertical("Box", options);
        {
            //Layer window da esquerda
            if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F2 && selectedLayer != -1) {
                Undo.RecordObject(rhythm, "Rename Layer");
                renamingIndex = selectedLayer;
            }

            GUILayout.Label("Layers", new GUIStyle() { fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter});
            bool btn = GUILayout.Button("AddLayer");
            EditorGUIUtility.labelWidth = 80;
            int layerToRemove = -1;

            layerScroll = GUILayout.BeginScrollView(layerScroll);

            for (int i = 0; i < rhythm.layers.Count; i++) {
                Rhythm.MarkupLayer layer = rhythm.layers[i];

                GUIStyle st = new GUIStyle();
                st.normal.background =  (i == selectedLayer) ? highlightTex : st.normal.background;
                Rect r = EditorGUILayout.BeginHorizontal(st);
                {
                    if(RhythmUtility.ToggleButton(!layer.visible, new GUIContent(hideIcon), GUILayout.Width(20), GUILayout.Height(20))) {
                        layer.visible = !layer.visible;
                    }

                    EditorGUIUtility.labelWidth = 1;
                    SerializedProperty p = layerProperty.GetArrayElementAtIndex(i).FindPropertyRelative("color");
                    EditorGUILayout.PropertyField(p);
                    p.serializedObject.ApplyModifiedProperties();

                    if (renamingIndex != i) {
                        EditorGUILayout.LabelField(rhythm.layers[i].layerName, new GUIStyle() { alignment = TextAnchor.MiddleCenter});
                    }
                    else {
                        if (GUILayout.Button("OK")) {
                            renamingIndex = -1;
                        }
                        layer.layerName = EditorGUILayout.TextField(layer.layerName);
                    }
                    if (GUILayout.Button("X")) {
                        layerToRemove = i;
                    }
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();

                //click on layer
                if (r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown) {
                    selectedLayer = i;
                    renamingIndex = -1;
                    GUI.FocusControl(null);
                }

            }

            if (btn) {
                rhythm.layers.Add(new Rhythm.MarkupLayer() {
                    layerName = "Layer " + rhythm.layers.Count,
                    markups = new List<Rhythm.Markup>(),
                    visible = true,
                    color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))
                });
                selectedLayer = rhythm.layers.Count - 1;
            }

            GUILayout.EndScrollView();
            if (layerToRemove != -1) {
                Undo.RecordObject(rhythm, "Remove Layer");
                rhythm.layers.RemoveAt(layerToRemove);
            }
        }
        EditorGUILayout.EndVertical();
    }

    public void UpdateReference(Rhythm rhythm)
    {
        this.rhythm = rhythm;
    }

    public void AddLayer(string layerName)
    {
        rhythm.layers.Add(new Rhythm.MarkupLayer() {
            layerName = layerName,
            markups = new List<Rhythm.Markup>(),
            visible = true,
            color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))
        });
        selectedLayer = rhythm.layers.Count - 1;
    }

    public int GetSelectedLayer()
    {
        return selectedLayer;
    }

    public void SelectLayer(Rhythm.MarkupLayer layer)
    {
        selectedLayer = rhythm.layers.IndexOf(layer);
    }

    public List<Rhythm.MarkupLayer> VisibleLayers()
    {
        List<Rhythm.MarkupLayer> layers = new List<Rhythm.MarkupLayer>();
        if (rhythm == null) return layers;
        if (rhythm.layers == null) rhythm.layers = new List<Rhythm.MarkupLayer>();
        foreach(Rhythm.MarkupLayer l in rhythm.layers) {
            if (l.visible) layers.Add(l);
        }
        return layers;
    }

}
#endif