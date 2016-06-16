using UnityEngine;
using System.Collections;

public class RemoteTriggerEvent : RemoteColliderObject {
    public override RemoteColliderType RemoteEventType {
        get { return RemoteColliderType.Trigger; }
    }

    public delegate void TriggerEvent(Collider other);

    public event TriggerEvent OnTriggerEnterEvent;
    public event TriggerEvent OnTriggerExitEvent;
    public event TriggerEvent OnTriggerStayEvent;

    private void OnTriggerEnter(Collider other) {
        if (OnTriggerEnterEvent != null) {
            OnTriggerEnterEvent(other);
        }
    }
    private void OnTriggerStay(Collider other) {
        if (OnTriggerStayEvent != null) {
            OnTriggerStayEvent(other);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (OnTriggerExitEvent != null) {
            OnTriggerExitEvent(other);
        }
    }
}
