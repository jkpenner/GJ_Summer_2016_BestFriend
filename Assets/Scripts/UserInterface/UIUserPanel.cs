using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIUserPanel : MonoBehaviour {
    public int playerId;

    public Transform connectInfo;
    public Transform selectorInfo;
    public Transform scoreInfo;

    public Button btnCharLeft;
    public Button btnCharRight;

    public Image[] imgSelectors;

    public Image imgBorder;

    public Text txtUserScore;

    public int initialSelection = 0;
    public int ActiveSelection { get; set; }

    private void Awake() {
        PlayerManager.AddListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);

        GameManager.AddListener(GameManager.EventType.StateEnter, OnGameStateEnter);
        GameManager.AddListener(GameManager.EventType.StateExit, OnGameStateExit);

        ScoreManager.AddListener(ScoreManager.EventType.ScoreChange, OnScoreChange);
        ScoreManager.AddListener(ScoreManager.EventType.PlayerAdd, OnPlayerScoreAdd);
        ScoreManager.AddListener(ScoreManager.EventType.PlayerRemove, OnPlayerScoreRemove);

        var playerInfo = PlayerManager.GetPlayerInfo(playerId);
        if (playerInfo != null) {
            imgBorder.color = playerInfo.color;
            ToggleUIElements(playerInfo.IsConnected);
        }

        ActiveSelection = -1;

        btnCharLeft.onClick.AddListener(OnCharLeftClick);
        btnCharRight.onClick.AddListener(OnCharRighClick);
    }

    private void OnPlayerScoreAdd(int playerId) {
        if (playerId == this.playerId) {
            Debug.Log("Player Add Updating score");
            txtUserScore.text = ScoreManager.GetPlayerScore(playerId).ToString();
        }
    }

    private void OnPlayerScoreRemove(int playerId) {
        if (playerId == this.playerId) {
            txtUserScore.text = "";
        }
    }

    private void OnScoreChange(int playerId) {
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
        if (GameManager.ActiveState != GameManager.State.Pause) {
            if (Input.GetButtonDown(PlayerManager.GetPlayerInputStr(playerId, "LeftBumper"))) {
                OnCharLeftClick();
            }

            if (Input.GetButtonDown(PlayerManager.GetPlayerInputStr(playerId, "RightBumper"))) {
                OnCharRighClick();
            }
        }
    }

    private void OnCharRighClick() {
        UpdateSelector(ActiveSelection + 1);
    }

    private void OnCharLeftClick() {
        UpdateSelector(ActiveSelection - 1);
    }

    private void OnPlayerDisconnect(PlayerManager.PlayerInfo player) {
        if (player.id == playerId) {
            Debug.LogFormat("[{0}]: Player Disconnected {1}", this.name, player.id);
            ToggleUIElements(false);
        }
    }

    private void OnPlayerConnect(PlayerManager.PlayerInfo player) {
        if (player.id == playerId) {
            Debug.LogFormat("[{0}]: Player Connected {1}", this.name, player.id);
            ToggleUIElements(true);
            UpdateSelector(initialSelection);
        }
    }

    private void UpdateSelector(int index) {
        if (index >= imgSelectors.Length) index = 0;
        if (index < 0) index = imgSelectors.Length - 1;

        if (ActiveSelection != index) {
            var playerInfo = PlayerManager.GetPlayerInfo(playerId);
            if (playerInfo != null) {
                playerInfo.CharacterSelection = index;
            }

            if (ActiveSelection >= 0 && ActiveSelection < imgSelectors.Length) {
                imgSelectors[ActiveSelection].enabled = false;
            }

            ActiveSelection = index;

            if (ActiveSelection >= 0 && ActiveSelection < imgSelectors.Length) {
                imgSelectors[ActiveSelection].enabled = true;
            }
        }
    }

    private void ToggleUIElements(bool isConnected) {
        connectInfo.gameObject.SetActive(!isConnected);
        selectorInfo.gameObject.SetActive(isConnected);
        scoreInfo.gameObject.SetActive(isConnected);
    }
}
