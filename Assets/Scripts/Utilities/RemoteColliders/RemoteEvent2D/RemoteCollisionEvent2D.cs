using UnityEngine;
using System.Collections;

public class RemoteCollisionEvent2D : RemoteColliderObject {
    public override RemoteColliderType RemoteEventType {
        get { return RemoteColliderType.Collision2D; }
    }

    public delegate void CollisionEvent2D(Collision2D other);

    public event CollisionEvent2D OnCollisionEnterEvent;
    public event CollisionEvent2D OnCollisionExitEvent;
    public event CollisionEvent2D OnCollisionStayEvent;

    private void OnCollisionEnter2D(Collision2D other) {
        if (OnCollisionEnterEvent != null) {
            OnCollisionEnterEvent(other);
        }
    }
    private void OnCollisionStay2D(Collision2D other) {
        if (OnCollisionStayEvent != null) {
            OnCollisionStayEvent(other);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (OnCollisionExitEvent != null) {
            OnCollisionExitEvent(other);
        }
    }
}
