#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public class TimelineController
{
    WaveformDrawer waveformDrawer;
    MarkupsDrawer markupsDrawer;
    SongController songController;
    LayerController layerController;
    MarkupVisualDetector markupVisualDetector;
    Rhythm rhythm;
    AudioClip noteData;
    Object midiAsset;
    bool pitchData;


    public TimelineController(Rhythm rhythm, BPMController bpmController, LayerController layerController, float windowWidth, Material material)
    {
        this.rhythm = rhythm;
        waveformDrawer = new WaveformDrawer(rhythm, new Vector2(windowWidth, 200), material);
        markupsDrawer = new MarkupsDrawer(rhythm, waveformDrawer, bpmController, layerController, this);
        songController = new SongController(rhythm, markupsDrawer, waveformDrawer, this);
        markupVisualDetector = new MarkupVisualDetector(rhythm, songController, markupsDrawer);
        this.layerController = layerController;
    }

    public void UpdateReference(Rhythm rhythm)
    {
        waveformDrawer.UpdateReference(rhythm);
        markupsDrawer.UpdateReference(rhythm);
        songController.UpdateReference(rhythm);
        markupVisualDetector.UpdateReference(rhythm);
    }

    public void UpdateMaterial(Material material)
    {
        waveformDrawer.UpdateMaterial(material);
    }

    public void AddMarkup(Rhythm.Markup markup)
    {
        rhythm.layers[layerController.GetSelectedLayer()].markups.Add(markup);
        markupsDrawer.SelectMarkup(markup);
    }

    public void DrawGUI(Rhythm rhythm)
    {
        this.rhythm = rhythm;
        waveformDrawer.DrawGUI();
        markupsDrawer.DrawGUI();

        EditorGUILayout.BeginHorizontal();
        {

            EditorGUILayout.BeginVertical(GUILayout.Width(waveformDrawer.GetRect().width / 3f));
            {

                //Import audio data
                EditorGUILayout.LabelField("Data Import", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                RhythmUtility.BCenter();
                noteData = (AudioClip)EditorGUILayout.ObjectField(noteData, typeof(AudioClip), true);
                RhythmUtility.ECenter();
                RhythmUtility.BCenter();
                pitchData = EditorGUILayout.Toggle("Import pitch data?", pitchData);
                RhythmUtility.ECenter();
                if (RhythmUtility.CenteredButton("Import from channel")) {
                    Rhythm r = AubioNoteDetector.Detect(rhythm, noteData, pitchData);
                    if (!r) {

                        //Erro
                        EditorWindow.GetWindow(typeof(AubioLibraryNotFound));
                    }
                }

                //Import MIDI Data
                RhythmUtility.BCenter();
                midiAsset = EditorGUILayout.ObjectField(midiAsset, typeof(Object), true);
                RhythmUtility.ECenter();
                if (RhythmUtility.CenteredButton("Import from MIDI")) {
                    rhythm = RhythmUtility.ReadMidi(rhythm, midiAsset);
                }

            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.Width(waveformDrawer.GetRect().width * 2/3f));
            {
                EditorGUILayout.BeginHorizontal();
                {
                    RhythmUtility.BCenter(GUILayout.Width(waveformDrawer.GetRect().width / 3f));
                    songController.DrawGUI();
                    RhythmUtility.ECenter();

                    RhythmUtility.BCenter(GUILayout.Width(waveformDrawer.GetRect().width / 3f));
                    markupVisualDetector.DrawGUI();
                    RhythmUtility.ECenter();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Select Rhythm clip", new GUIStyle() { alignment = TextAnchor.MiddleCenter });
                RhythmUtility.BCenter();
                AudioClip c = (AudioClip)EditorGUILayout.ObjectField(rhythm.clip, typeof(AudioClip), true);
                if (rhythm.clip != c) {
                    rhythm.clip = c;
                    waveformDrawer.SetDirty();
                }
                RhythmUtility.ECenter();
            }
            EditorGUILayout.EndVertical();

           

        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20);
    }

    public float GetScroll()
    {
        return waveformDrawer.GetScroll();
    }

    public float GetNeedle()
    {
        return markupsDrawer.GetNeedle();
    }

    public void SetNeedle(float needle)
    {
        markupsDrawer.SetNeedle(needle);
    }

    public MarkupsDrawer GetMarkupsDrawer()
    {
        return markupsDrawer;
    }

    public WaveformDrawer GetWaveformDrawer()
    {
        return waveformDrawer;
    }

    public SongController GetSongController()
    {
        return songController;
    }

    public MarkupVisualDetector GetMarkupVisualDetector()
    {
        return markupVisualDetector;
    }

    public void NotifySongStart()
    {
        markupVisualDetector.NotifySongStart();
    }
}

class AubioLibraryNotFound : EditorWindow
{
    static void Init()
    {
        AubioLibraryNotFound window = CreateInstance<AubioLibraryNotFound>();
        window.position = new Rect((Screen.width - 250)/2f, (Screen.height - 150)/2f, 250, 150);
        window.ShowPopup();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Aubio Library not found, please read the User Guide Section 8| Additional Tools - Audio Analisys Data Import", EditorStyles.wordWrappedLabel);
        GUILayout.Space(30);
        if (GUILayout.Button("Agree!")) {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal("Assets/Rhythmator/Rhythmator User’s Guide.pdf", 100);
            this.Close();
        }
    }
}
#endif