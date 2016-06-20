using UnityEngine;
using System.Collections;

public class SettingManager : Singleton<SettingManager> {
    public enum PlayerCount { TwoPlayer, FourPlayer }
    public enum GameMode { Normal, Random }

    [SerializeField]
    private PlayerCount _playerCount;
    static public PlayerCount ActivePlayerCount {
        get {
            if (Instance != null) {
                return Instance._playerCount;
            } else {
                Debug.LogWarning("[SettingManager]: Trying to access ActivePlayerCount, but no instance in scene.");
                return PlayerCount.TwoPlayer;
            }
        }
    }

    [SerializeField]
    private GameMode _gameMode;
    static public GameMode ActiveGameMode {
        get {
            if (Instance != null) {
                return Instance._gameMode;
            } else {
                Debug.LogWarning("[SettingManager]: Trying to access ActiveGameMode, but no instance in scene.");
                return GameMode.Normal;
            }
        }
    }

    static public void Set(PlayerCount countType, GameMode gameType) {
        if (Instance != null) {
            Instance._playerCount = countType;
            Instance._gameMode = gameType;
        } else {
            Debug.LogWarning("[SettingManager]: Trying to set values, but no instance in scene.");
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
