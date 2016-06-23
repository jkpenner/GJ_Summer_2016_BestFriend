using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIUserPanel3 : MonoBehaviour {
    public PlayerId playerId;

    public Transform connectInfo;
    public Transform selectorInfo;
    public Transform scoreInfo;

    public Button btnCharLeft;
    public Button btnCharRight;

    public Image[] imgSelectors;

    public Image imgBorder;

    public Text txtUserScore;

    public int initialSelection = 0;
    public int ActiveSelectionId { get; set; }
    public int ActiveSelectionIndex { get; set; }

    private void Awake() {
        if (SettingManager.ActivePlayerCount == SettingManager.PlayerCount.TwoPlayer &&
            (playerId == PlayerId.Three || playerId == PlayerId.Four)) {
            this.gameObject.SetActive(false);
            return;
        }

        if (SettingManager.ActiveGameMode == SettingManager.GameMode.Random) {
            selectorInfo.gameObject.SetActive(false);
        }

        PlayerManager.AddListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);

        GameManager.AddStateListener(GameManager.StateEventType.Enter, OnGameStateEnter);
        GameManager.AddStateListener(GameManager.StateEventType.Exit, OnGameStateExit);

        ScoreManager.AddListener(ScoreManager.ScoreEventType.Changed, OnScoreChange);
        ScoreManager.AddListener(ScoreManager.ScoreEventType.PlayerAdd, OnPlayerScoreAdd);
        ScoreManager.AddListener(ScoreManager.ScoreEventType.PlayerRemove, OnPlayerScoreRemove);

        var playerInfo = PlayerManager.GetPlayerInfo(playerId);
        if (playerInfo != null) {
            imgBorder.color = playerInfo.Color;
            ToggleUIElements(playerInfo.IsConnected);
            if (playerInfo.IsConnected) {
                txtUserScore.text = ScoreManager.GetPlayerScore(playerId).ToString();
            }
        }

        ActiveSelectionIndex = -1;
        UpdateSelector(0);

        if (SettingManager.ActiveGameMode != SettingManager.GameMode.Random) {
            btnCharLeft.onClick.AddListener(OnCharLeftClick);
            btnCharRight.onClick.AddListener(OnCharRighClick);
        }
    }

    private void OnDestroy() {
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);

        GameManager.RemoveStateListener(GameManager.StateEventType.Enter, OnGameStateEnter);
        GameManager.RemoveStateListener(GameManager.StateEventType.Exit, OnGameStateExit);

        ScoreManager.RemoveListener(ScoreManager.ScoreEventType.Changed, OnScoreChange);
        ScoreManager.RemoveListener(ScoreManager.ScoreEventType.PlayerAdd, OnPlayerScoreAdd);
        ScoreManager.RemoveListener(ScoreManager.ScoreEventType.PlayerRemove, OnPlayerScoreRemove);
    }

    private void OnPlayerScoreAdd(PlayerId playerId) {
        if (playerId == this.playerId) {
            Debug.Log("Player Add Updating score");
            txtUserScore.text = ScoreManager.GetPlayerScore(playerId).ToString();
        }
    }

    private void OnPlayerScoreRemove(PlayerId playerId) {
        if (playerId == this.playerId) {
            txtUserScore.text = "";
        }
    }

    private void OnScoreChange(PlayerId playerId) {
        if (playerId == this.playerId) {
            txtUserScore.text = ScoreManager.GetPlayerScore(playerId).ToString();
        }
    }

    private void OnGameStateExit(GameManager.State state) {
        if (state == GameManager.State.Pause) {
            btnCharLeft.interactable = true;
            btnCharRight.interactable = true;
        }
    }

    private void OnGameStateEnter(GameManager.State state) {
        if (state == GameManager.State.Pause) {
            btnCharLeft.interactable = false;
            btnCharRight.interactable = false;
        }
    }

    private void Update() {
        if (SettingManager.ActiveGameMode != SettingManager.GameMode.Random) {
            if (GameManager.ActiveState != GameManager.State.Pause) {
                if (Input.GetButtonDown(PlayerManager.GetPlayerInputStr(playerId, "LeftBumper"))) {
                    OnCharLeftClick();
                }

                if (Input.GetButtonDown(PlayerManager.GetPlayerInputStr(playerId, "RightBumper"))) {
                    OnCharRighClick();
                }
            }
        }
    }

    private void OnCharRighClick() {
        UpdateSelector(ActiveSelectionIndex + 1);
    }

    private void OnCharLeftClick() {
        UpdateSelector(ActiveSelectionIndex - 1);
    }

    private void OnPlayerDisconnect(PlayerInfo player) {
        if (player.Id == playerId) {
            Debug.LogFormat("[{0}]: Player Disconnected {1}", this.name, player.Id);
            ToggleUIElements(false);
        }
    }

    private void OnPlayerConnect(PlayerInfo player) {
        if (player.Id == playerId) {
            Debug.LogFormat("[{0}]: Player Connected {1}", this.name, player.Id);
            ToggleUIElements(true);
            UpdateSelector(initialSelection);
        }
    }

    private void UpdateSelector(int index) {
        int charCount = CharacterDatabase.Instance.Count;

        index = LoopIndex(index, charCount);

        if (ActiveSelectionIndex != index) {
            ActiveSelectionIndex = index;

            var playerInfo = PlayerManager.GetPlayerInfo(playerId);
            if (playerInfo != null) {
                playerInfo.CharacterSelectionId = CharacterDatabase.Instance.Get(ActiveSelectionIndex).Id;
            }

            // Set the left character icon
            var lChar = CharacterDatabase.Instance.Get(LoopIndex(ActiveSelectionIndex - 1, charCount));
            if (lChar != null) {
                imgSelectors[0].sprite = lChar.Icon;
            }

            // Set the center character icon
            var cChar = CharacterDatabase.Instance.Get(ActiveSelectionIndex);
            if (cChar != null) {
                imgSelectors[1].sprite = cChar.Icon;
            }

            // Set the right charcter icon
            var rChar = CharacterDatabase.Instance.Get(LoopIndex(ActiveSelectionIndex + 1, charCount));
            if (rChar != null) {
                imgSelectors[2].sprite = rChar.Icon;
            }
        }
    }

    private int LoopIndex(int value, int listCount) {
        if (value < 0) return listCount - 1;
        if (value >= listCount) return 0;
        return value;
    }

    private void ToggleUIElements(bool isConnected) {
        connectInfo.gameObject.SetActive(!isConnected);
        selectorInfo.gameObject.SetActive(isConnected && SettingManager.ActiveGameMode != SettingManager.GameMode.Random);
        scoreInfo.gameObject.SetActive(isConnected);
    }
}
