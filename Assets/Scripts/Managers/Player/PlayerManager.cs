using UnityEngine;
using System.Collections;

public class PlayerManager : Singleton<PlayerManager> {
    [System.Serializable]
    public class PlayerInfo {
        public int id;
        public Color color;
        public string postfix;
        public string prefix;
        public bool canAutoDisconnect;
        
        public bool IsConnected { get; set; }
        public float AutoDisconnectCounter { get; set; }
        /// <summary>
        /// Id of character within the Character Database
        /// </summary>
        public int CharacterSelectionId { get; set; }
    }

    public PlayerInfo[] players;

    // Seconds until the controller is disconnected if
    // no input is detected.
    public float disconnectPlayerDelay = 60f;

    public delegate void PlayerEvent(PlayerInfo player);
    private PlayerEvent OnPlayerConnect;
    private PlayerEvent OnPlayerDisconnect;

    public enum EventType { PlayerConnect, PlayerDisconnect }

    private void Awake() {
        if (Instance == this) {
            this.transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }

    private void Update() {
        foreach (var player in players) {
            if (player.IsConnected && player.canAutoDisconnect) {
                if (IsControllerActive(player)) {
                    player.AutoDisconnectCounter = disconnectPlayerDelay;
                } else {
                    player.AutoDisconnectCounter -= Time.deltaTime;
                    if (player.AutoDisconnectCounter <= 0) {
                        Debug.LogFormat("[{0}]: Player {1} Disconnected from game.", this.name, player.id);
                        player.AutoDisconnectCounter = 0;
                        player.IsConnected = false;
                        if (OnPlayerDisconnect != null) {
                            OnPlayerDisconnect(player);
                        }
                    }
                }
            } else if(!player.IsConnected && CheckForConnectKey(player)) {
                Debug.LogFormat("[{0}]: Player {1} Connected to game.", this.name, player.id);
                player.AutoDisconnectCounter = disconnectPlayerDelay;
                player.IsConnected = true;
                if (OnPlayerConnect != null) {
                    OnPlayerConnect(player);
                }
            }
        }
    }

    private bool IsControllerActive(PlayerInfo player) {
        return Input.GetButton(GetPlayerInputStr(player, "A")) ||
            Input.GetButton(GetPlayerInputStr(player, "B")) ||
            Input.GetButton(GetPlayerInputStr(player, "Start")) ||
            Input.GetButton(GetPlayerInputStr(player, "LeftBumper")) ||
            Input.GetButton(GetPlayerInputStr(player, "RightBumper")) ||
            Input.GetButton(GetPlayerInputStr(player, "Start")) ||
            Input.GetAxis(GetPlayerInputStr(player, "DP_Horizontal")) != 0f ||
            Input.GetAxis(GetPlayerInputStr(player, "DP_Vertical")) != 0f ||
            Input.GetAxis(GetPlayerInputStr(player, "LS_Horizontal")) != 0f ||
            Input.GetAxis(GetPlayerInputStr(player, "LS_Vertical")) != 0f;
    }

    private bool CheckForConnectKey(PlayerInfo player) {
        try {
            return Input.GetButton(GetPlayerInputStr(player, "A"));
        } catch (System.Exception e) {
            Debug.LogWarningFormat("[{0}]: {1}", this.name, e.Message);
            return false;
        }
    }

    static public void AddListener(EventType type, PlayerEvent func) {
        if (Instance != null) {
            switch (type) {
                case EventType.PlayerConnect: Instance.OnPlayerConnect += func; break;
                case EventType.PlayerDisconnect: Instance.OnPlayerDisconnect += func; break;
            }
        } else {
            Debug.LogWarningFormat("[{0}]: Attempting to add event listener, but no instance found of {0}", "PlayerManager");
        }
    }

    static public void RemoveListener(EventType type, PlayerEvent func) {
        if (Instance != null) {
            switch (type) {
                case EventType.PlayerConnect: Instance.OnPlayerConnect -= func; break;
                case EventType.PlayerDisconnect: Instance.OnPlayerDisconnect -= func; break;
            }
        } else {
            Debug.LogWarningFormat("[{0}]: Attempting to remove event listener, but no instance found of {0}", "PlayerManager");
        }
    }

    static public PlayerInfo[] GetAllPlayerInfo() {
        if (Instance != null) {
            return Instance.players;
        }
        return null;
    }

    static public PlayerInfo GetPlayerInfo(PlayerId playerId) {
        if (Instance != null) {
            foreach (var player in Instance.players) {
                if (player.Id == playerId) {
                    return player;
                }
            }
        }
        return null;
    }

    static public string GetPlayerInputStr(PlayerId playerId, string inputStr) {
        if (Instance != null) {
            return GetPlayerInputStr(GetPlayerInfo(playerId), inputStr);
        }
        return string.Empty;
    }

    static public string GetPlayerInputStr(PlayerInfo player, string inputStr) {
        if (player != null) {
            return inputStr + player.Postfix;
        }
        return string.Empty;
    }
}
