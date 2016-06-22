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

    Dictionary<PlayerId, int> _scores = new Dictionary<PlayerId, int>();

    public delegate void ScoreEvent(PlayerId playerId);
    private ScoreEvent OnScoreChange;
    private ScoreEvent OnPlayerAdd;
    private ScoreEvent OnPlayerRemove;

    public enum ScoreEventType { Changed, PlayerAdd, PlayerRemove }

    public delegate void RoundEvent();
    private RoundEvent OnRoundComplete;
    private RoundEvent OnRoundStart;
    private RoundEvent OnRoundResume;

    public enum RoundEventType { Complete, Start, Resume }

    private void Awake() {
        foreach (var player in PlayerManager.GetAllPlayerInfo()) {
            if (player.IsConnected) {
                AddPlayer(player.Id);
            }
        }
    }

    private void OnEnable() {
        GameManager.AddListener(GameManager.EventType.StateEnter, OnGameStateEnter);
        GameManager.AddListener(GameManager.EventType.StateExit, OnGameStateExit);

        PlayerManager.AddListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);
    }

    private void OnPlayerDisconnect(PlayerInfo player) {
        RemovePlayer(player.Id);
    }

    private void OnPlayerConnect(PlayerInfo player) {
        AddPlayer(player.Id);
    }

    private void OnDisable() {
        GameManager.RemoveListener(GameManager.EventType.StateEnter, OnGameStateEnter);
        GameManager.RemoveListener(GameManager.EventType.StateExit, OnGameStateExit);

        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);
    }

    private void Update() {
        if (_roundCounter > 0) {
            _roundCounter -= Time.deltaTime * roundDecayRate * _pauseDecayMod;
        } else if (_roundCounter < 0) {
            _roundCounter = 0;

            StorageManager.RoundScores.Add(0);
            for (int i = 1; i < 5; i++) {
                StorageManager.RoundScores[StorageManager.RoundScores.Count - 1] += GetPlayerScore((PlayerId)i);
            }

            for (int i = 0; i < StorageManager.RoundScores.Count - 2; i++) {
                StorageManager.RoundScores[StorageManager.RoundScores.Count - 1] -= StorageManager.RoundScores[i];
            }

            StorageManager.RoundTimes.Add(roundLength - _roundCounter);


            if (OnRoundComplete != null) {
                OnRoundComplete();
            }

            GameManager.SetState(GameManager.State.RoundLost);
        }
    }

    private void OnGameStateExit(GameManager.State state) {
        if (state == GameManager.State.RoundLost) {
            _roundCounter = roundLength;
            CurrentRound++;

            if (CurrentRound > RoundsPerGame) {
                for (int i = 1; i < 5; i++) {
                    StorageManager.PlayerScores.Add(GetPlayerScore((PlayerId)i));
                }
                // Load the Score Scene
                UnityEngine.SceneManagement.SceneManager.LoadScene(4);
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

    static public void EndRound(bool roundWin) {
        if (Instance != null) {
            StorageManager.RoundScores.Add(0);
            for (int i = 1; i < 5; i++) {
                StorageManager.RoundScores[StorageManager.RoundScores.Count - 1] += GetPlayerScore((PlayerId)i);
            }

            for (int i = 0; i < StorageManager.RoundScores.Count - 2; i++) {
                StorageManager.RoundScores[StorageManager.RoundScores.Count - 1] -= StorageManager.RoundScores[i];
            }

            StorageManager.RoundTimes.Add(Instance.roundLength - Instance._roundCounter);

            if (Instance.OnRoundComplete != null) {
                Instance.OnRoundComplete();
            }
            Instance.CurrentRound++;
            if (roundWin == true) {
                GameManager.SetState(GameManager.State.RoundWin);
            } else {
                GameManager.SetState(GameManager.State.RoundLost);
            }
        }
    }

    static public void AddPlayer(PlayerId id) {
        if (Instance != null) {
            if (!Instance._scores.ContainsKey(id)) {
                Instance._scores.Add(id, 0);
                if (Instance.OnPlayerAdd != null) {
                    Instance.OnPlayerAdd(id);
                }
            } else {
                Debug.LogFormat("[{0}]: Attempting to add player with id {1}, " +
                    "but id already exists.", Instance.name, id);
            }
        } else {
            Debug.LogFormat("[{0}]: Attempting to add player, " +
                "but there is not instance in the scene.", "ScoreManager", id);
        }
    }

    static public void RemovePlayer(PlayerId id) {
        if (Instance != null) {
            if (Instance._scores.ContainsKey(id)) {
                Instance._scores.Remove(id);
                if (Instance.OnPlayerRemove != null) {
                    Instance.OnPlayerRemove(id);
                }
            } else {
                Debug.LogFormat("[{0}]: Attempting to remove player with id {1}, " +
                    "but id does not exists.", Instance.name, id);
            }
        } else {
            Debug.LogFormat("[{0}]: Attempting to remove player, " +
                "but there is not instance in the scene.", "ScoreManager", id);
        }
    }

    static public int GetPlayerScore(PlayerId id) {
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

    static public void ModifyPlayerScore(PlayerId id, int amount) {
        if (Instance._scores.ContainsKey(id)) {
            Debug.Log("Modifiying Player Score " + id.ToString() + " by " + amount);
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

    static public void SetPlayerScore(PlayerId id, int amount) {
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
