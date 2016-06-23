using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    public enum State { None, Active, RoundLost, RoundWin, Pause, GameWin, GameOver }

    // Game State Related Event Info
    public enum StateEventType { Enter, Exit }
    public delegate void GameStateEvent(State state);
    private GameStateEvent OnGameStateEnter;
    private GameStateEvent OnGameStateExit;

    // Round Reltated Event Info
    public enum RoundEventType { Begin, Complete }
    public delegate void RoundEvent();
    public RoundEvent OnRoundComplete;
    public RoundEvent OnRoundBegin;

    // Active Game State Info
    private State _activeState = State.None;
    public State initialState = State.None;

    private bool firstRoundReset = true;
    public float roundDuration = 120f;
    public float roundResetDuration = 4f;

    [SerializeField]
    private float _roundCounter = 0;
    private float _pauseDecayMod = 1;

    public float RoundCounter {
        get { return _roundCounter; }
    }

    public Transform winTransform;

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
            SetState(State.RoundLost);
        }

        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);
    }

    private void OnDisable() {
        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);

        OnGameStateEnter -= GameStateEnter;
        OnGameStateExit -= GameStateExit;

        OnGameStateEnter = null;
        OnGameStateExit = null;
    }

    private void OnPlayerDisconnect(PlayerInfo player) {
        if (ActiveState == State.Active || ActiveState == State.Pause) {
            bool playerConnected = false;
            var playerInfos = PlayerManager.GetAllPlayerInfo();
            foreach (var playerInfo in playerInfos) {
                if (playerInfo.IsConnected) {
                    playerConnected = true;
                    break;
                }
            }

            if (!playerConnected) {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    private void Update() {
        if (ActiveState == State.Active) {
            if (_roundCounter > 0) {
                _roundCounter -= Time.deltaTime * _pauseDecayMod;
            } else if (_roundCounter < 0) {
                _roundCounter = 0;

                //StorageManager.RoundScores.Add(0);
                //for (int i = 1; i < 5; i++) {
                //    StorageManager.RoundScores[StorageManager.RoundScores.Count - 1] += ScoreManager.GetPlayerScore((PlayerId)i);
                //}
                //
                //for (int i = 0; i < StorageManager.RoundScores.Count - 2; i++) {
                //    StorageManager.RoundScores[StorageManager.RoundScores.Count - 1] -= StorageManager.RoundScores[i];
                //}
                //
                //StorageManager.RoundTimes.Add(roundDuration - _roundCounter);

                GameManager.SetState(GameManager.State.RoundLost);

                if (OnRoundComplete != null) {
                    OnRoundComplete();
                }
            }
        }
    }

    private void GameStateEnter(State state) {
        switch (state) {
            case State.RoundWin:
            case State.RoundLost:
                StartCoroutine("OnResetEnter", roundResetDuration);
                break;
            case State.Pause:
                _pauseDecayMod = 0;
                break;
            case State.Active:
                _roundCounter = roundDuration;
                break;
        }
    }

    private void GameStateExit(State state) {
        switch (state) {
            case State.RoundWin:
            case State.RoundLost:
                if (OnRoundBegin != null) {
                    OnRoundBegin();
                }
                break;
            case State.Pause:
                _pauseDecayMod = 1;
                break;
        }
    }

    private IEnumerator OnResetEnter(float wait) {
        if (firstRoundReset) {
            firstRoundReset = false;
            yield return new WaitForSeconds(0.1f);
            SetState(State.Active);
        } else {
            yield return new WaitForSeconds(wait);
            SceneManager.LoadScene("MainScore");
        }
    }

    static public State ActiveState {
        get { return Instance._activeState; }
        set { SetState(value); }
    }

    static public void SetState(State newState) {
        if (Instance != null) {
            if (Instance._activeState != newState) {
                //Debug.LogFormat("[{0}]: State assigned to {1}",
                //    "GameManager", newState.ToString());

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
        } else {
            Debug.LogWarningFormat("[{0}]: Attempting to set state, but there is no instance in the scene.",
                    "GameManager", newState.ToString());
        }
    }

    static public void AddStateListener(StateEventType type, GameStateEvent func) {
        if (Instance != null) {
            switch (type) {
                case StateEventType.Enter: Instance.OnGameStateEnter += func; break;
                case StateEventType.Exit: Instance.OnGameStateExit += func; break;
            }
        }
    }

    static public void RemoveStateListener(StateEventType type, GameStateEvent func) {
        if (Instance != null) {
            switch (type) {
                case StateEventType.Enter: Instance.OnGameStateEnter -= func; break;
                case StateEventType.Exit: Instance.OnGameStateExit -= func; break;
            }
        }
    }

    static public void AddRoundListner(RoundEventType type, RoundEvent func) {
        if (Instance != null) {
            switch (type) {
                case RoundEventType.Begin: Instance.OnRoundBegin += func; break;
                case RoundEventType.Complete: Instance.OnRoundComplete += func; break;
            }
        }
    }

    static public void RemoveRoundListener(RoundEventType type, RoundEvent func) {
        if (Instance != null) {
            switch (type) {
                case RoundEventType.Begin: Instance.OnRoundBegin -= func; break;
                case RoundEventType.Complete: Instance.OnRoundComplete -= func; break;
            }
        }
    }

    static public void EndRound(bool win) {
        if (win == true) {
            GameManager.SetState(GameManager.State.RoundWin);
        } else {
            GameManager.SetState(GameManager.State.RoundLost);
        }

        if (Instance.OnRoundComplete != null) {
            Instance.OnRoundComplete();
        }
    }

    static public float GetRequireHeightForWin() {
        return Instance.winTransform.position.y;
    }
}
