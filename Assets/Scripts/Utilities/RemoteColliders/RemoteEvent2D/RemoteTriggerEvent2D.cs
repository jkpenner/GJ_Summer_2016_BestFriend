using UnityEngine;
using System.Collections;

public class RemoteTriggerEvent2D : RemoteColliderObject {
    public override RemoteColliderType RemoteEventType {
        get { return RemoteColliderType.Trigger2D; }
    }

    public delegate void TriggerEvent2D(Collider2D other);
    
    public event TriggerEvent2D OnTriggerEnterEvent;
    public event TriggerEvent2D OnTriggerExitEvent;
    public event TriggerEvent2D OnTriggerStayEvent;

    private void OnTriggerEnter2D(Collider2D other) {
        if (OnTriggerEnterEvent != null) {
            OnTriggerEnterEvent(other);
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        if (OnTriggerStayEvent != null) {
            OnTriggerStayEvent(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (OnTriggerExitEvent != null) {
            OnTriggerExitEvent(other);
        }
    }
}
