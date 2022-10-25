using UnityEditor;
using UnityEngine;

namespace AssetInventory
{
    public sealed class FolderSettingsUI : PopupWindowContent
    {
        private FolderSpec _spec;

        public void Init(FolderSpec spec)
        {
            _spec = spec;
        }

        public override void OnGUI(Rect rect)
        {
            editorWindow.maxSize = new Vector2(300, 130);
            editorWindow.minSize = editorWindow.maxSize;

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(UIStyles.Content("Content", "Type of content to scan for"), EditorStyles.boldLabel, GUILayout.Width(85));
            _spec.folderType = EditorGUILayout.Popup(_spec.folderType, UIStyles.FolderTypes);
            GUILayout.EndHorizontal();

            if (_spec.folderType == 1)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(UIStyles.Content("Find", "File types to search for"), EditorStyles.boldLabel, GUILayout.Width(85));
                _spec.scanFor = EditorGUILayout.Popup(_spec.scanFor, UIStyles.MediaTypes);
                GUILayout.EndHorizontal();

                if (_spec.scanFor == 6)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(UIStyles.Content("Pattern", "e.g. *.jpg;*.wav"), EditorStyles.boldLabel, GUILayout.Width(85));
                    _spec.pattern = EditorGUILayout.TextField(_spec.pattern);
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(UIStyles.Content("Previews", "Recommended. Will generate previews and additional metadata but requires more time during indexing."), EditorStyles.boldLabel, GUILayout.Width(85));
                _spec.createPreviews = EditorGUILayout.Toggle(_spec.createPreviews);
                GUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck()) AssetInventory.SaveConfig();
        }
    }
}