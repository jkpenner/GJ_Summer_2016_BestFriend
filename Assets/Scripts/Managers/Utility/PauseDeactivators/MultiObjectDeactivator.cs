using UnityEngine;
using System.Collections;
using System;

public class MultiObjectDeactivator : AbstractPauseDeactivator {
    public System.Collections.Generic.List<GameObject> objects = 
        new System.Collections.Generic.List<GameObject>();

    public override void OnPauseEnter() {
        foreach (var go in objects) {
            go.SetActive(false);
        }
    }

    public override void OnPauseExit() {
        foreach (var go in objects) {
            go.SetActive(true);
        }
    }
}
