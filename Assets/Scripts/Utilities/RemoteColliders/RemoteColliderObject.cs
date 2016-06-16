using UnityEngine;
using System.Collections;

public class RemoteColliderObject : MonoBehaviour {
    public virtual RemoteColliderType RemoteEventType {
        get { return RemoteColliderType.None; }
    }

    [SerializeField]
    private MonoBehaviour controllingScript = null;
    public MonoBehaviour ControllingScript {
        get { return controllingScript; }
        set { controllingScript = value; }
    }

    public void SetControllingScript(MonoBehaviour script) {
        ControllingScript = script;
    }
}
