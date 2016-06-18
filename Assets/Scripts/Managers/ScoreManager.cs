using UnityEngine;
using System.Collections.Generic;
using System;

public class ScoreManager : Singleton<ScoreManager> {
    public float roundLength = 30f;
    public float roundDecayRate = 1f;
    public int roundScoreReward = 1;

    private float _roundCounter = 0f;
    public float RoundCounter {
        get { return _roundCounter; }
    }

    Dictionary<int, int> _scores = new Dictionary<int, int>();

    public delegate void ScoreEvent(int playerId);
    private ScoreEvent OnScoreChange;
    private ScoreEvent OnPlayerAdd;
    private ScoreEvent OnPlayerRemove;

    public enum EventType { ScoreChange, PlayerAdd, PlayerRemove }

    private void OnEnable() {
        GameManager.AddListener(GameManager.EventType.StateEnter, OnGameStateEnter);
        GameManager.AddListener(GameManager.EventType.StateExit, OnGameStateExit);
    }    

    private void OnDisable() {
        GameManager.RemoveListener(GameManager.EventType.StateEnter, OnGameStateEnter);
        GameManager.RemoveListener(GameManager.EventType.StateExit, OnGameStateExit);
    }

    private void Update() {
        if (_roundCounter > 0) {
            _roundCounter -= Time.deltaTime * roundDecayRate;
        } else if(_roundCounter < 0) {
            _roundCounter = 0;
        }
    }

    private void OnGameStateExit(GameManager.State state) {
        if (state == GameManager.State.Reset) {
            _roundCounter = roundLength;
        }
    }

    private void OnGameStateEnter(GameManager.State state) {
        
    }

    static public void AddListener(EventType e, ScoreEvent func) {
        switch (e) {
            case EventType.PlayerAdd: Instance.OnPlayerAdd += func; break;
            case EventType.PlayerRemove: Instance.OnPlayerRemove += func; break;
            case EventType.ScoreChange: Instance.OnScoreChange += func; break;
        }
    }

    static public void RemoveListener(EventType e, ScoreEvent func) {
        switch (e) {
            case EventType.PlayerAdd: Instance.OnPlayerAdd -= func; break;
            case EventType.PlayerRemove: Instance.OnPlayerRemove -= func; break;
            case EventType.ScoreChange: Instance.OnScoreChange -= func; break;
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
