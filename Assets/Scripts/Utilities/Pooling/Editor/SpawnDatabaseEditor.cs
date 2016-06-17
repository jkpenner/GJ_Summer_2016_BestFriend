using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SpawnDatabaseEditor : EditorWindow {
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

    [MenuItem("BFGT/Object Pool")]
    static public void ShowWindow() {
        SpawnDatabaseEditor wnd = GetWindow<SpawnDatabaseEditor>();
        wnd.titleContent.text = "Object Pool";
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
        if (SpawnDatabase.Instance.Count > 0) {
            for (int i = 0; i < SpawnDatabase.Instance.Count; i++) {
                var asset = SpawnDatabase.Instance.Get(i);

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
        var asset = SpawnDatabase.Instance.Get(SelectedIndex);
        if (asset != null) {
            EditorGUI.BeginChangeCheck();

            GUILayout.Label(string.IsNullOrEmpty(asset.Name) ? "Empty" : asset.Name, EditorStyles.toolbarButton);

            GUILayout.BeginHorizontal();
            GUILayout.Label("ID: " + asset.Id.ToString("D4"), GUILayout.Width(70));
            asset.Prefab = EditorGUILayout.ObjectField(asset.Prefab, typeof(GameObject), false) as GameObject;

            GUILayout.EndHorizontal();

            DisplaySection("Pooling", () => {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Start", GUILayout.Width(50));
                asset.PoolCount = EditorGUILayout.IntField(asset.PoolCount);
                GUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("The amount of instances that will be created when the game starts", MessageType.None);

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Restrict", GUILayout.Width(50));
                asset.PoolRestricted = EditorGUILayout.Toggle(asset.PoolRestricted);
                GUILayout.EndHorizontal();
                EditorGUILayout.HelpBox("Are instances limited to being created only when the game starts", MessageType.None);
            });

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(SpawnDatabase.Instance);
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
            SpawnDatabase.Instance.Add(new SpawnAsset(SpawnDatabase.Instance.GetNextId()));
            EditorUtility.SetDirty(SpawnDatabase.Instance);
            SelectedIndex = SpawnDatabase.Instance.Count - 1;
        }
        if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(88))) {
            SpawnDatabase.Instance.RemoveAt(SelectedIndex--);
            EditorUtility.SetDirty(SpawnDatabase.Instance);
            if (SelectedIndex == -1 && SpawnDatabase.Instance.Count > 0) {
                SelectedIndex = 0;
            }
        }

        GUILayout.FlexibleSpace();

        GUILayout.Label(string.Format("Objects: {0}", 
            SpawnDatabase.Instance.Count.ToString("D3")),
            EditorStyles.centeredGreyMiniLabel);

        GUILayout.EndHorizontal();

        GUILayout.Space(2);
    }
}
