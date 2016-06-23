using UnityEngine;
using System.Collections.Generic;
using System;

public class ScoreManager : Singleton<ScoreManager> {
    Dictionary<PlayerId, int> _scores = new Dictionary<PlayerId, int>();

    public delegate void ScoreEvent(PlayerId playerId);
    private ScoreEvent OnScoreChange;
    private ScoreEvent OnPlayerAdd;
    private ScoreEvent OnPlayerRemove;

    public enum ScoreEventType { Changed, PlayerAdd, PlayerRemove }

    private bool firstReset = true;

    private void Awake() {
        foreach (var player in PlayerManager.GetAllPlayerInfo()) {
            if (player.IsConnected) {
                AddPlayer(player.Id);
            }
        }
    }

    private void OnEnable() {
        PlayerManager.AddListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);

        GameManager.AddStateListener(GameManager.StateEventType.Enter, OnStateEnter);
    }

    private void OnDisable() {
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);

        GameManager.RemoveStateListener(GameManager.StateEventType.Enter, OnStateEnter);
    }

    private void OnStateEnter(GameManager.State state) {
        if (state == GameManager.State.RoundLost ||
            state == GameManager.State.RoundWin) {

            if (!firstReset) {
                StorageManager.ActiveRoundTimes = Mathf.Clamp(
                    (GameManager.Instance.roundDuration - GameManager.Instance.RoundCounter),
                    0, GameManager.Instance.roundDuration);

                int totalScore = 0;

                var playerInfos = PlayerManager.GetAllPlayerInfo();
                for (int i = 0; i < playerInfos.Length; i++) {
                    var score = GetPlayerScore(playerInfos[i].Id);
                    if (StorageManager.IsNewLeaderboardScoreSolo(score)) {
                        StorageManager.AddNewLeaderboardScoreSolo(score,
                            StorageManager.ActiveRoundTimes);
                    }
                    StorageManager.ActivePlayerScores.Add(score);
                    totalScore += score;
                }

                StorageManager.ActiveRoundScores = totalScore;
                if (StorageManager.IsNewLeaderboardScoreTeam(totalScore)) {
                    StorageManager.AddNewLeaderboardScoreTeam(totalScore,
                        StorageManager.ActiveRoundTimes);
                }
            } else {
                firstReset = false;
            }
        }
    }

    private void OnPlayerDisconnect(PlayerInfo player) {
        //RemovePlayer(player.Id);
    }

    private void OnPlayerConnect(PlayerInfo player) {
        AddPlayer(player.Id);
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

    static public void RemoveListener(ScoreEventType e, ScoreEvent func) {
        if (Instance != null) {
            switch (e) {
                case ScoreEventType.PlayerAdd: Instance.OnPlayerAdd -= func; break;
                case ScoreEventType.PlayerRemove: Instance.OnPlayerRemove -= func; break;
                case ScoreEventType.Changed: Instance.OnScoreChange -= func; break;
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
