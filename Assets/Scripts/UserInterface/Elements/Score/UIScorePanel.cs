using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIScorePanel : MonoBehaviour {
    public GameObject userScorePrefab;
    public Transform userScoreParent;

    private CanvasGroup canvasGroup;

    private Dictionary<PlayerId, UIUserScore>  _userScores;
    public Dictionary<PlayerId, UIUserScore> UserScores {
        get {
            if (_userScores == null) {
                _userScores = new Dictionary<PlayerId, UIUserScore>();
            }
            return _userScores;
        }
    }

    public void Start() {
        if (userScoreParent == null) {
            userScoreParent = this.transform;
        }

        ScoreManager.AddListener(ScoreManager.ScoreEventType.PlayerAdd, OnPlayerAdd);
        ScoreManager.AddListener(ScoreManager.ScoreEventType.PlayerRemove, OnPlayerRemove);
        ScoreManager.AddListener(ScoreManager.ScoreEventType.Changed, OnScoreChange);

        ScoreManager.AddPlayer(PlayerId.One);
        ScoreManager.AddPlayer(PlayerId.Two);
        ScoreManager.AddPlayer(PlayerId.Three);
        ScoreManager.AddPlayer(PlayerId.Four);
    }

    

    private void OnScoreChange(PlayerId playerId) {
        if (UserScores.ContainsKey(playerId)) {
            UserScores[playerId].UserScore = ScoreManager.GetPlayerScore(playerId).ToString("D8");
        }
    }

    private void OnPlayerRemove(PlayerId playerId) {
        if (UserScores.ContainsKey(playerId)) {
            Destroy(UserScores[playerId].gameObject);
            UserScores.Remove(playerId);
        }
    }

    private void OnPlayerAdd(PlayerId playerId) {
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
