using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorageManager : Singleton<StorageManager> {
    [System.Serializable]
    public class LeaderboardInfo {
        public int score;
        public float time;
    }

    [SerializeField]
    private int _activeRoundScore;

    [SerializeField]
    private float _activeRoundTime;

    [SerializeField]
    private List<int> _activePlayerScores;

    [SerializeField]
    private List<LeaderboardInfo> _leaderBoardTeam;

    [SerializeField]
    private List<LeaderboardInfo> _leaderBoardSolo;

    static public int ActiveRoundScores {
        get { return Instance._activeRoundScore; }
        set { Instance._activeRoundScore = value; }
    }

    static public float ActiveRoundTimes {
        get { return Instance._activeRoundTime; }
        set { Instance._activeRoundTime = value; }
    }

    static public List<int> ActivePlayerScores {
        get {
            if (Instance._activePlayerScores == null) {
                Instance._activePlayerScores = new List<int>();
            }
            return Instance._activePlayerScores;
        }
    }

    static public List<LeaderboardInfo> LeaderboardTeam {
        get {
            if (Instance._leaderBoardTeam == null) {
                Instance._leaderBoardTeam = new List<LeaderboardInfo>();
            }
            return Instance._leaderBoardTeam;
        }
    }

    static public List<LeaderboardInfo> LeaderboardSolo {
        get {
            if (Instance._leaderBoardSolo == null) {
                Instance._leaderBoardSolo = new List<LeaderboardInfo>();
            }
            return Instance._leaderBoardSolo;
        }
    }

    static public bool IsNewLeaderboardScoreTeam(int value) {
        for (int i = 0; i < LeaderboardTeam.Count; i++) {
            if (LeaderboardTeam[i].score < value) {
                return true;
            }
        }
        return false;
    }

    static public bool IsNewLeaderboardScoreSolo(int value) {
        for (int i = 0; i < LeaderboardSolo.Count; i++) {
            if (LeaderboardSolo[i].score < value) {
                return true;
            }
        }
        return false;
    }

    static public void AddNewLeaderboardScoreTeam(int score, float time) {
        // Populate the Leaderboard info
        var info = new LeaderboardInfo();
        info.score = score;
        info.time = time;

        // Check where the score should be placed
        bool wasAdded = false;
        for (int i = 0; i < LeaderboardTeam.Count; i++) {
            if (LeaderboardTeam[i].score < info.score) {
                LeaderboardTeam.Insert(i, info);
                wasAdded = true;
                break;
            }
        }

        // If not greater then other scores at to end of list
        if (wasAdded == false) {
            LeaderboardTeam.Add(info);
        }

        // Remove any scores pass the max of 5
        if (LeaderboardTeam.Count > 5) {
            LeaderboardTeam.RemoveRange(5, LeaderboardTeam.Count - 5);
        }
    }

    static public void AddNewLeaderboardScoreSolo(int score, float time) {
        // Populate the Leaderboard info
        var info = new LeaderboardInfo();
        info.score = score;
        info.time = time;

        // Check where the score should be placed
        bool wasAdded = false;
        for (int i = 0; i < LeaderboardSolo.Count; i++) {
            if (LeaderboardSolo[i].score < info.score) {
                LeaderboardSolo.Insert(i, info);
                wasAdded = true;
                break;
            }
        }

        // If not greater then other scores at to end of list
        if (wasAdded == false) {
            LeaderboardSolo.Add(info);
        }

        // Remove any scores pass the max of 5
        if (LeaderboardSolo.Count > 5) {
            LeaderboardSolo.RemoveRange(5, LeaderboardSolo.Count - 5);
        }
    }

    private void Start() {
        if (Instance == this) {
            this.transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }
}
