using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {
    public enum State { None, Active, Pause, GameWin, GameOver }

    public enum EventType { StateEnter, StateExit }

    public delegate void GameStateEvent(State state);
    private GameStateEvent OnGameStateEnter;
    private GameStateEvent OnGameStateExit;

    private State _activeState = State.None;
    public State initialState = State.None;

    private void Awake() {
        if (Instance == this) {
            DontDestroyOnLoad(this.gameObject);
            ActiveState = initialState;
        } else {
            Destroy(this.gameObject);
        }
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
        switch (type) {
            case EventType.StateEnter: Instance.OnGameStateEnter += func; break;
            case EventType.StateExit: Instance.OnGameStateExit += func; break;
        }
    }

    static public void RemoveListener(EventType type, GameStateEvent func) {
        switch (type) {
            case EventType.StateEnter: Instance.OnGameStateEnter -= func; break;
            case EventType.StateExit: Instance.OnGameStateExit -= func; break;
        }
    }
}
