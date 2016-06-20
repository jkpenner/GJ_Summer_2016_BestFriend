using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorageManager : Singleton<StorageManager> {
    private List<int> _roundScores;
    private List<float> _roundTimes;

    private List<int> _playerScores;

    static public List<int> RoundScores {
        get {
            if (Instance._roundScores == null) {
                Instance._roundScores = new List<int>();
            }
            return Instance._roundScores;
        }
    }

    static public List<float> RoundTimes {
        get {
            if (Instance._roundTimes == null) {
                Instance._roundTimes = new List<float>();
            }
            return Instance._roundTimes;
        }
    }

    static public List<int> PlayerScores {
        get {
            if (Instance._playerScores == null) {
                Instance._playerScores = new List<int>();
            }
            return Instance._playerScores;
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
