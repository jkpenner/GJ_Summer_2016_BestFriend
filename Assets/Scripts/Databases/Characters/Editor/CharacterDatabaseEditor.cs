using UnityEngine;
using UnityEditor;
using System.Collections;

public class CharacterDatabaseEditor : EditorWindow {
    private Vector2 selectorScroll;

    private int _selectorIndex = -1;
    public int SelectedIndex {
        get {
            return _selectorIndex;
        }
        set {
            if (_selectorIndex != value) {
                _selectorIndex = value;
                EditorGUI.FocusTextInControl(string.Empty);
            }
        }
    }

    [MenuItem("BFGT/Characters")]
    static public void ShowWindow() {
        CharacterDatabaseEditor wnd = GetWindow<CharacterDatabaseEditor>();
        wnd.titleContent.text = "Characters";
        wnd.Show();
    }

    public void OnGUI() {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        DisplaySelector();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        DisplayContent();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        DisplayFooter();
    }

    private void DisplaySelector() {
        // Scroll view for the listed assets
        selectorScroll = GUILayout.BeginScrollView(selectorScroll, false, true);

        // List all the assets in the database
        GUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
        if (CharacterDatabase.Instance.Count > 0) {
            for (int i = 0; i < CharacterDatabase.Instance.Count; i++) {
                var asset = CharacterDatabase.Instance.Get(i);

                bool isVisible = GUILayout.Toggle(i == SelectedIndex,
                    string.IsNullOrEmpty(asset.Name) ? "Empty" : asset.Name,
                    EditorStyles.toolbarButton);

                if (SelectedIndex == i || isVisible) {
                    SelectedIndex = i;
                }
            }
        } else {
            GUILayout.Label("Empty", EditorStyles.centeredGreyMiniLabel);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

        GUILayout.EndScrollView();
    }

    private void DisplayContent() {
        GUILayout.BeginVertical();
        var asset = CharacterDatabase.Instance.Get(SelectedIndex);
        if (asset != null) {
            EditorGUI.BeginChangeCheck();

            GUILayout.Label(string.IsNullOrEmpty(asset.Name) ? "Empty" : asset.Name, EditorStyles.toolbarButton);

            GUILayout.BeginHorizontal();
            GUILayout.Label("ID: " + asset.Id.ToString("D4"), GUILayout.Width(70));
            asset.Prefab = EditorGUILayout.ObjectField(asset.Prefab, typeof(GameObject), false) as GameObject;

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Icon", GUILayout.Width(50));
            asset.Icon = (Sprite)EditorGUILayout.ObjectField(asset.Icon, typeof(Sprite), false);
            GUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("The icon that will display for the character", MessageType.None);

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(CharacterDatabase.Instance);
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }

    public void DisplaySection(string label, System.Action action) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, EditorStyles.centeredGreyMiniLabel, GUILayout.Width(60));
        GUILayout.BeginVertical("Box");
        action();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    public void DisplayFooter() {
        // Show the add and remove selected buttons
        GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(88))) {
            CharacterDatabase.Instance.Add(new CharacterAsset(CharacterDatabase.Instance.GetNextId()));
            EditorUtility.SetDirty(CharacterDatabase.Instance);
            SelectedIndex = CharacterDatabase.Instance.Count - 1;
        }
        if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(88))) {
            CharacterDatabase.Instance.RemoveAt(SelectedIndex--);
            EditorUtility.SetDirty(CharacterDatabase.Instance);
            if (SelectedIndex == -1 && CharacterDatabase.Instance.Count > 0) {
                SelectedIndex = 0;
            }
        }

        GUILayout.FlexibleSpace();

        GUILayout.Label(string.Format("Objects: {0}",
            CharacterDatabase.Instance.Count.ToString("D3")),
            EditorStyles.centeredGreyMiniLabel);

        GUILayout.EndHorizontal();

        GUILayout.Space(2);
    }
}
