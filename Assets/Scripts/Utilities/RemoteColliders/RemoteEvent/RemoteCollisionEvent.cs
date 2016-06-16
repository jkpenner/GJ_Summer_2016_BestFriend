using UnityEngine;
using System.Collections;

public class RemoteCollisionEvent : RemoteColliderObject {
    public override RemoteColliderType RemoteEventType {
        get { return RemoteColliderType.Collision; }
    }

    public delegate void CollisionEvent(Collision other);

    public event CollisionEvent OnCollisionEnterEvent;
    public event CollisionEvent OnCollisionExitEvent;
    public event CollisionEvent OnCollisionStayEvent;

    private void OnCollisionEnter(Collision other) {
        if (OnCollisionEnterEvent != null) {
            OnCollisionEnterEvent(other);
        }
    }
    private void OnCollisionStay(Collision other) {
        if (OnCollisionStayEvent != null) {
            OnCollisionStayEvent(other);
        }
    }

    private void OnCollisionExit(Collision other) {
        if (OnCollisionExitEvent != null) {
            OnCollisionExitEvent(other);
        }
    }
}
