using UnityEngine;
using System.Collections;
using System;

public class Rigidbody2DDeactivator : AbstractPauseDeactivator {
    public new Rigidbody2D rigidbody2d;

    public override void OnEnable() {
        if (rigidbody2d == null) {
            rigidbody2d = GetComponent<Rigidbody2D>();
        }
    }

    public override void OnPauseEnter() {
        if (rigidbody2d != null) {
            rigidbody2d.isKinematic = true;
        }
    }

    public override void OnPauseExit() {
        if (rigidbody2d != null) {
            rigidbody2d.isKinematic = false;
        }
    }
}
