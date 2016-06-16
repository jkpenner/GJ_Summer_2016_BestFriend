using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RemoteColliderObject), true)]
public class RemoteColliderEditor : Editor {
    public override void OnInspectorGUI() {
        var remoteObj = (RemoteColliderObject)target;

        EditorGUI.BeginChangeCheck();

        // Display read-only field of the remote type. Value set by the script.
        EditorGUILayout.LabelField("Remote Type", remoteObj.RemoteEventType.ToString());

        // The script that is controlling the remote collider
        remoteObj.ControllingScript = (MonoBehaviour)EditorGUILayout.ObjectField(
            "Controller", remoteObj.ControllingScript, typeof(MonoBehaviour), true);

        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(remoteObj);
        }
    }
}
