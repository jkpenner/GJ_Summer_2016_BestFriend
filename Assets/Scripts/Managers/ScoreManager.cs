using UnityEngine;
using System.Collections.Generic;
using System;

public class ScoreManager : Singleton<ScoreManager> {
    public float roundLength = 30f;
    public float roundDecayRate = 1f;
    public int roundScoreReward = 1;

    private float _pauseDecayMod = 1f;
    private float _roundCounter = 0f;
    public float RoundCounter {
        get { return _roundCounter; }
    }

    public int CurrentRound { get; set; }

    [SerializeField]
    private int _roundsPerGame = 3;
    public int RoundsPerGame {
        get { return _roundsPerGame; }
        set { _roundsPerGame = value; }
    }

    [SerializeField]
    private int _roundsToWin = 2;
    public int RoundsToWin {
        get { return _roundsToWin; }
        set { _roundsToWin = value; }
    }

    public Transform winTransform;

    Dictionary<int, int> _scores = new Dictionary<int, int>();

    public delegate void ScoreEvent(int playerId);
    private ScoreEvent OnScoreChange;
    private ScoreEvent OnPlayerAdd;
    private ScoreEvent OnPlayerRemove;

    public enum ScoreEventType { Changed, PlayerAdd, PlayerRemove }

    public delegate void RoundEvent();
    private RoundEvent OnRoundComplete;
    private RoundEvent OnRoundStart;
    private RoundEvent OnRoundResume;

    public enum RoundEventType { Complete, Start, Resume }

    private void OnEnable() {
        GameManager.AddListener(GameManager.EventType.StateEnter, OnGameStateEnter);
        GameManager.AddListener(GameManager.EventType.StateExit, OnGameStateExit);

        PlayerManager.AddListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);
    }

    private void OnPlayerDisconnect(PlayerManager.PlayerInfo player) {
        RemovePlayer(player.id);
    }

    private void OnPlayerConnect(PlayerManager.PlayerInfo player) {
        AddPlayer(player.id);
    }

    private void OnDisable() {
        GameManager.RemoveListener(GameManager.EventType.StateEnter, OnGameStateEnter);
        GameManager.RemoveListener(GameManager.EventType.StateExit, OnGameStateExit);
    }

    private void Update() {
        if (_roundCounter > 0) {
            _roundCounter -= Time.deltaTime * roundDecayRate * _pauseDecayMod;
        } else if (_roundCounter < 0) {
            _roundCounter = 0;


            if (OnRoundComplete != null) {
                OnRoundComplete();
            }

            

            GameManager.SetState(GameManager.State.Reset);
        }
    }

    private void OnGameStateExit(GameManager.State state) {
        if (state == GameManager.State.Reset) {
            _roundCounter = roundLength;
            CurrentRound++;

            if (CurrentRound > RoundsPerGame) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }

            if (OnRoundStart != null) {
                OnRoundStart();
            }
        }

        if (state == GameManager.State.Pause) {
            _pauseDecayMod = 1;
            if (OnRoundResume != null) {
                OnRoundResume();
            }
        }
    }

    private void OnGameStateEnter(GameManager.State state) {
        if (state == GameManager.State.Pause) {
            _pauseDecayMod = 0;
        }
    }

    static public void AddListener(ScoreEventType e, ScoreEvent func) {
        if (Instance != null) {
            switch (e) {
                case ScoreEventType.PlayerAdd: Instance.OnPlayerAdd += func; break;
                case ScoreEventType.PlayerRemove: Instance.OnPlayerRemove += func; break;
                case ScoreEventType.Changed: Instance.OnScoreChange += func; break;
            }
        }
    }

    static public void AddListener(RoundEventType e, RoundEvent func) {
        if (Instance != null) {
            switch (e) {
                case RoundEventType.Complete: Instance.OnRoundComplete += func; break;
                case RoundEventType.Start: Instance.OnRoundStart += func; break;
                case RoundEventType.Resume: Instance.OnRoundResume += func; break;
            }
        }
    }

    static public void RemoveListener(RoundEventType e, RoundEvent func) {
        if (Instance != null) {
            switch (e) {
                case RoundEventType.Complete: Instance.OnRoundComplete -= func; break;
                case RoundEventType.Start: Instance.OnRoundStart -= func; break;
                case RoundEventType.Resume: Instance.OnRoundResume -= func; break;
            }
        }
    }

    static public void RemoveListener(ScoreEventType e, ScoreEvent func) {
        if (Instance != null) {
            switch (e) {
                case ScoreEventType.PlayerAdd: Instance.OnPlayerAdd -= func; break;
                case ScoreEventType.PlayerRemove: Instance.OnPlayerRemove -= func; break;
                case ScoreEventType.Changed: Instance.OnScoreChange -= func; break;
            }
        }
    }

    static public void EndRound() {
        if (Instance != null) {
            if (Instance.OnRoundComplete != null) {
                Instance.OnRoundComplete();
            }
            Instance.CurrentRound++;
            GameManager.SetState(GameManager.State.Reset);
        }
    }

    static public void AddPlayer(int id) {
        if (!Instance._scores.ContainsKey(id)) {
            Instance._scores.Add(id, 0);
            if (Instance.OnPlayerAdd != null) {
                Instance.OnPlayerAdd(id);
            }
        } else {
            Debug.LogFormat("[{0}]: Attempting to add player with id {1}, " +
                "but id already exists.", Instance.name, id);
        }
    }

    static public void RemovePlayer(int id) {
        if (Instance._scores.ContainsKey(id)) {
            Instance._scores.Remove(id);
            if (Instance.OnPlayerRemove != null) {
                Instance.OnPlayerRemove(id);
            }
        } else {
            Debug.LogFormat("[{0}]: Attempting to remove player with id {1}, " +
                "but id does not exists.", Instance.name, id);
        }
    }

    static public int GetPlayerScore(int id) {
        if (Instance._scores.ContainsKey(id)) {
            return Instance._scores[id];
        }
        return 0;
    }

    static public int GetCombinedScore() {
        int value = 0;
        foreach (var pair in Instance._scores) {
            value += pair.Value;
        }
        return value;
    }

    static public void ModifyPlayerScore(int id, int amount) {
        if (Instance._scores.ContainsKey(id)) {
            if (amount != 0) {
                Instance._scores[id] += amount;
                Instance._scores[id] = Mathf.Max(0, Instance._scores[id]);
                if (Instance.OnScoreChange != null) {
                    Instance.OnScoreChange(id);
                }
            }
        } else {
            Debug.LogFormat("[{0}]: Attempting to modify score of player with id {1}, " +
                "but id does not exists.", Instance.name, id);
        }
    }

    static public void SetPlayerScore(int id, int amount) {
        if (Instance._scores.ContainsKey(id)) {
            if (amount != Instance._scores[id]) {
                Instance._scores[id] += amount;
                Instance._scores[id] = Mathf.Max(0, Instance._scores[id]);
                if (Instance.OnScoreChange != null) {
                    Instance.OnScoreChange(id);
                }
            }
        } else {
            Debug.LogFormat("[{0}]: Attempting to set score of player with id {1}, " +
                "but id does not exists.", Instance.name, id);
        }
    }
}
