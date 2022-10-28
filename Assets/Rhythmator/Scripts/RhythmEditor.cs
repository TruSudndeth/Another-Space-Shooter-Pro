using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class RhythmEditor : EditorWindow
{
    public static TimelineController timelineController;
    public static LayerController layerController;
    public static BPMController bpmController;
    public static MarkupDataController markupDataController;
    public static DefaultControls defaultControls;

    public static SelectionTool selectionTool;
    public static MarkupTool markupTool;
    public static RhythmTool selectedTool;

    Texture2D selection_icon;
    Texture2D markup_icon;
    public Rhythm rhythm;

    Material material;

    [MenuItem("Window/Rhythm Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(RhythmEditor));
    }

    private void Initialize(Rhythm rhythm)
    {
        if (timelineController == null) {
            bpmController = new BPMController(rhythm);
            layerController = new LayerController(rhythm);
            timelineController = new TimelineController(rhythm, bpmController, layerController, position.width, material);
            markupDataController = new MarkupDataController(rhythm, timelineController.GetMarkupsDrawer());
            defaultControls = new DefaultControls(this);
        }

        if (material == null) material = Resources.Load("WaveformMaterial") as Material;

        timelineController.UpdateReference(rhythm);
        bpmController.UpdateReference(rhythm);
        layerController.UpdateReference(rhythm);
        markupDataController.UpdateReference(rhythm);

        timelineController.GetWaveformDrawer().SetWidth(position.width);

        if (selection_icon == null) selection_icon = Resources.Load("cursor") as Texture2D;
        if (markup_icon == null) markup_icon = Resources.Load("mais") as Texture2D;

    }

    private void OnGUI()
    {

        try {
            Rhythm r = Selection.activeObject as Rhythm;
            if (r != null || rhythm == null) rhythm = r;
            Initialize(rhythm);
            if (rhythm != null) {

                timelineController.UpdateMaterial(material);
                //Toolbars
                if (selectionTool == null) selectionTool = new SelectionTool(this);
                if (markupTool == null) markupTool = new MarkupTool(this);

                if (selectedTool == null) selectedTool = selectionTool;


                EditorGUILayout.BeginHorizontal();
                if (RhythmUtility.ToggleButton(selectedTool == selectionTool, new GUIContent() { image = selection_icon, tooltip = "Selection Tool" }, GUILayout.Width(30), GUILayout.Height(30))) {
                    selectedTool = selectionTool;
                }
                if (RhythmUtility.ToggleButton(selectedTool == markupTool, new GUIContent() { image = markup_icon, tooltip = "Markup Tool" }, GUILayout.Width(30), GUILayout.Height(30))) {
                    selectedTool = markupTool;
                }
                selectedTool.GUI(this);

                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(rhythm == null ? "No Rhythm selected" : rhythm.name, new GUIStyle() { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                timelineController.DrawGUI(rhythm);


                EditorGUILayout.BeginHorizontal();
                {
                    layerController.DrawGUI(GUILayout.Width(300));
                    markupDataController.DrawGUI(GUILayout.Width(position.width - 300));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("BPM Options")) {
                    BPMPopup.Init();
                }
                EditorGUILayout.EndHorizontal();
                defaultControls.Update();
            }

        }
        catch {

        }
    }


    private void Update()
    {
        Repaint();
    }

}

class BPMPopup : EditorWindow
{
    public static void Init()
    {
        BPMPopup pp = CreateInstance<BPMPopup>();
        pp.position = new Rect(Screen.width / 2f, Screen.height / 2f, 400, 200);
        pp.ShowModalUtility();
    }

    private void OnGUI()
    {
        RhythmEditor.bpmController.DrawGUI();
    }
}
#endif