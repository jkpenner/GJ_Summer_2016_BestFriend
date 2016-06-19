using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {
    public override void OnInspectorGUI() {
        var manager = (GameManager)target;

        if (Application.isPlaying) {
            GameManager.SetState((GameManager.State)EditorGUILayout.EnumPopup("Active State", GameManager.ActiveState));
        } else {
            EditorGUI.BeginChangeCheck();

            manager.initialState = (GameManager.State)EditorGUILayout.EnumPopup("Initial State", manager.initialState);

            if(EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
