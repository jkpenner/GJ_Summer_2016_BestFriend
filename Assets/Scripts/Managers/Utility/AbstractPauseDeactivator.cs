using UnityEngine;
using System.Collections;
using System;

public abstract class AbstractPauseDeactivator : MonoBehaviour {
    public void Start() {
        GameManager.AddListener(GameManager.EventType.StateEnter, StateEnter);
        GameManager.RemoveListener(GameManager.EventType.StateExit, StateExit);
    }

    private void StateExit(GameManager.State state) {
        if (state == GameManager.State.Pause) {
            OnPauseExit();
        }
    }

    private void StateEnter(GameManager.State state) {
        if (state == GameManager.State.Pause) {
            OnPauseEnter();
        }
    }

    public virtual void OnEnable() { }
    public abstract void OnPauseEnter();
    public abstract void OnPauseExit();
}
