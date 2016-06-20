using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {
    public override void OnInspectorGUI() {
        var manager = (GameManager)target;

        base.OnInspectorGUI();

        if (Application.isPlaying) {
            GameManager.SetState((GameManager.State)EditorGUILayout.EnumPopup("Active State", GameManager.ActiveState));
        }
    }
}
