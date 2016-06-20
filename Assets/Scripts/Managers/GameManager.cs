using UnityEngine;
using System.Collections;
using System;

public class GameManager : Singleton<GameManager> {
    public enum State { None, Active, Reset, RoundOver, Pause, GameWin, GameOver }

    public enum EventType { StateEnter, StateExit, RoundComplete }

    public delegate void GameStateEvent(State state);
    private GameStateEvent OnGameStateEnter;
    private GameStateEvent OnGameStateExit;

    private State _activeState = State.None;
    public State initialState = State.None;

    private bool firstReset = true;
    public float resetStateDuration = 4f;

    public UICharacterSelector selector;

    private void Awake() {
        if (Instance == this) {
            this.transform.SetParent(null);
            //DontDestroyOnLoad(this.gameObject);
            ActiveState = initialState;
        } else {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable() {
        OnGameStateEnter += GameStateEnter;
        OnGameStateExit += GameStateExit;

        if (ActiveState == State.None) {
            SetState(State.Reset);
        }
    }

    private void OnDisable() {
        OnGameStateEnter -= GameStateEnter;
        OnGameStateExit -= GameStateExit;

        OnGameStateEnter = null;
        OnGameStateExit = null;
    }

    private void GameStateExit(State state) {
        
    }

    private void GameStateEnter(State state) {
        switch (state) {
            case State.Reset:
                StartCoroutine("OnResetEnter", resetStateDuration);
                break;
        }
    }

    private IEnumerator OnResetEnter(float wait) {
        if (firstReset) {
            firstReset = false;
            yield return new WaitForSeconds(0.1f);
        } else {
            yield return new WaitForSeconds(wait);
        }
        SetState(State.Active);
    }

    static public State ActiveState {
        get { return Instance._activeState; }
        set { SetState(value); } 
    }

    static public void SetState(State newState) {
        if (Instance._activeState != newState) {
            Debug.LogFormat("[{0}]: State assigned to {1}", 
                "GameManager", newState.ToString());

            // Call State Exit for previous ActiveState
            if (Instance.OnGameStateExit != null) {
                Instance.OnGameStateExit(ActiveState);
            }

            Instance._activeState = newState;

            // Call State Enter for new ActiveState
            if (Instance.OnGameStateEnter != null) {
                Instance.OnGameStateEnter(ActiveState);
            }
        }
    }

    static public void AddListener(EventType type, GameStateEvent func) {
        if (Instance != null) {
            switch (type) {
                case EventType.StateEnter: Instance.OnGameStateEnter += func; break;
                case EventType.StateExit: Instance.OnGameStateExit += func; break;
            }
        }
    }

    static public void RemoveListener(EventType type, GameStateEvent func) {
        if (Instance != null) {
            switch (type) {
                case EventType.StateEnter: Instance.OnGameStateEnter -= func; break;
                case EventType.StateExit: Instance.OnGameStateExit -= func; break;
            }
        }
    }
}
