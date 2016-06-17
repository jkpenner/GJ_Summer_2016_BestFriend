using UnityEngine;
using System.Collections;
using System;

public class RigidbodyDeactivator : AbstractPauseDeactivator {
    public new Rigidbody rigidbody;

    public override void OnEnable() {
        if (rigidbody == null) {
            rigidbody = GetComponent<Rigidbody>();
        }
    }

    public override void OnPauseEnter() {
        if (rigidbody != null) {
            rigidbody.isKinematic = true;
        }
    }

    public override void OnPauseExit() {
        if (rigidbody != null) {
            rigidbody.isKinematic = false;
        }
    }
}
