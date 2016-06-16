using UnityEngine;
using System.Collections.Generic;
using System;

public class UIScorePanel : MonoBehaviour {
    public GameObject userScorePrefab;
    public Transform userScoreParent;

    private Dictionary<int, UIUserScore>  _userScores;
    public Dictionary<int, UIUserScore> UserScores {
        get {
            if (_userScores == null) {
                _userScores = new Dictionary<int, UIUserScore>();
            }
            return _userScores;
        }
    }

    public void Start() {
        if (userScoreParent == null) {
            userScoreParent = this.transform;
        }

        ScoreManager.AddListener(ScoreManager.EventType.PlayerAdd, OnPlayerAdd);
        ScoreManager.AddListener(ScoreManager.EventType.PlayerRemove, OnPlayerRemove);
        ScoreManager.AddListener(ScoreManager.EventType.ScoreChange, OnScoreChange);
    }

    private void OnScoreChange(int playerId) {
        if (UserScores.ContainsKey(playerId)) {
            UserScores[playerId].UserScore = ScoreManager.GetPlayerScore(playerId).ToString("D8");
        }
    }

    private void OnPlayerRemove(int playerId) {
        if (UserScores.ContainsKey(playerId)) {
            Destroy(UserScores[playerId].gameObject);
            UserScores.Remove(playerId);
        }
    }

    private void OnPlayerAdd(int playerId) {
        var go = GameObject.Instantiate(userScorePrefab);
        var userScore = go.GetComponent<UIUserScore>();
        userScore.UserName = "Player " + playerId.ToString();
        userScore.UserScore = ScoreManager.GetPlayerScore(playerId).ToString("D8");
        if (userScore != null) {
            go.transform.SetParent(userScoreParent, true);
            UserScores.Add(playerId, userScore);
        }
    }
}
