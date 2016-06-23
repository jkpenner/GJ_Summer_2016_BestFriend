using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIUserPanel : MonoBehaviour {
    private enum PanelState { Active, Disconnect, Score };

    public PlayerId playerId;
    private PanelState panelState;

    public GameObject connectionInfo;
    public GameObject characterSelector;
    public GameObject playerScoreInfo;
    public GameObject playerScoreBar;

    public Image characterSelectorLeft;
    public Image characterSelectorCenter;
    public Image characterSelectorRight;

    public Image panelBorder;

    public Text playerName;
    public Text playerScore;

    // Score Bar Elements
    public RectTransform scoreBarRect;
    public RectTransform scoreBarParentRect;

    private Image scoreBarImage;

    private int activeSelectionIndex = 0;

    private void Awake() {
        // If player count is two and panel is assigned to player three or four, disable panel.
        if(SettingManager.ActivePlayerCount == SettingManager.PlayerCount.TwoPlayer &&
            (playerId == PlayerId.Three || playerId == PlayerId.Four)) {
            this.gameObject.SetActive(false);
            return;
        }

        // If game mode is random hide the character selector
        if (SettingManager.ActiveGameMode == SettingManager.GameMode.Random) {
            characterSelector.gameObject.SetActive(false);
        }

        PlayerManager.AddListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);

        GameManager.AddStateListener(GameManager.StateEventType.Enter, OnGameStateEnter);
        GameManager.AddStateListener(GameManager.StateEventType.Exit, OnGameStateExit);

        ScoreManager.AddListener(ScoreManager.ScoreEventType.Changed, OnScoreChange);
        ScoreManager.AddListener(ScoreManager.ScoreEventType.PlayerAdd, OnPlayerScoreAdd);

        var playerInfo = PlayerManager.GetPlayerInfo(playerId);
        if (playerInfo != null) {
            panelBorder.color = playerInfo.Color;
            ToggleUIElements(playerInfo.IsConnected);
            if (playerInfo.IsConnected) {
                playerScore.text = ScoreManager.GetPlayerScore(playerId).ToString();
            }
        }

        activeSelectionIndex = -1;
        UpdateSelector(0);
    }

    private void OnDestroy() {
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);

        GameManager.RemoveStateListener(GameManager.StateEventType.Enter, OnGameStateEnter);
        GameManager.RemoveStateListener(GameManager.StateEventType.Exit, OnGameStateExit);

        ScoreManager.RemoveListener(ScoreManager.ScoreEventType.Changed, OnScoreChange);
        ScoreManager.RemoveListener(ScoreManager.ScoreEventType.PlayerAdd, OnPlayerScoreAdd);
    }

    private void OnPlayerScoreAdd(PlayerId playerId) {
        if (playerId == this.playerId) {
            playerScore.text = ScoreManager.GetPlayerScore(playerId).ToString();
        }
    }

    private void OnScoreChange(PlayerId playerId) {
        if (playerId == this.playerId) {
            playerScore.text = ScoreManager.GetPlayerScore(playerId).ToString();
        }
    }

    private void OnGameStateExit(GameManager.State state) {
        if (state == GameManager.State.RoundLost ||
            state == GameManager.State.RoundWin) {
            
        }
    }

    private void OnGameStateEnter(GameManager.State state) {
        if (state == GameManager.State.RoundLost ||
            state == GameManager.State.RoundWin) {

        }
    }

    private void Update() {
        if (SettingManager.ActiveGameMode != SettingManager.GameMode.Random) {
            if (GameManager.ActiveState != GameManager.State.Pause) {
                if (Input.GetButtonDown(PlayerManager.GetPlayerInputStr(playerId, "LeftBumper"))) {
                    UpdateSelector(activeSelectionIndex - 1);
                }

                if (Input.GetButtonDown(PlayerManager.GetPlayerInputStr(playerId, "RightBumper"))) {
                    UpdateSelector(activeSelectionIndex + 1);
                }
            }
        }
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
            UpdateSelector(0);
        }
    }

    private void UpdateSelector(int index) {
        int charCount = CharacterDatabase.Instance.Count;

        index = LoopIndex(index, charCount);

        if (activeSelectionIndex != index) {
            activeSelectionIndex = index;

            var playerInfo = PlayerManager.GetPlayerInfo(playerId);
            if (playerInfo != null) {
                playerInfo.CharacterSelectionId = CharacterDatabase.Instance.Get(activeSelectionIndex).Id;
            }
            
            // Set the left character icon
            var lChar = CharacterDatabase.Instance.Get(LoopIndex(activeSelectionIndex - 1, charCount));
            if (lChar != null) {
                characterSelectorLeft.sprite = lChar.Icon;
            }

            // Set the center character icon
            var cChar = CharacterDatabase.Instance.Get(activeSelectionIndex);
            if (cChar != null) {
                characterSelectorCenter.sprite = cChar.Icon;
            }

            // Set the right charcter icon
            var rChar = CharacterDatabase.Instance.Get(LoopIndex(activeSelectionIndex + 1, charCount));
            if (rChar != null) {
                characterSelectorRight.sprite = rChar.Icon;
            }
        }
    }

    private int LoopIndex(int value, int listCount) {
        if(value < 0) return listCount - 1;
        if(value >= listCount) return 0;
        return value;
    }

    private void ToggleUIElements(bool isConnected) {
        connectionInfo.SetActive(!isConnected);
        characterSelector.SetActive(isConnected && SettingManager.ActiveGameMode != SettingManager.GameMode.Random);
        playerScoreInfo.SetActive(isConnected);
    }

    public void DisplayScoreBar(int maxValue, float displayLength) {
        SetPanelState(PanelState.Score);
        StartCoroutine("PopulateScore", new object[] { maxValue, displayLength });
    }

    IEnumerator PopulateScore(object [] objs) {
        int maxValue = (int)objs[0];
        float duration = (float)objs[1];

        float counter = duration;
        while (counter > 0f) {
            counter -= Time.deltaTime;
            float percentComplete = (duration - counter) / duration;

            float barProgress = (ScoreManager.GetPlayerScore(playerId) / maxValue) * percentComplete;

            scoreBarRect.localScale = new Vector3(
                scoreBarRect.localScale.x,
                barProgress,
                scoreBarRect.localScale.z);

            yield return new WaitForEndOfFrame();
        }
    }

    private void SetPanelState(PanelState state) {
        var playerInfo = PlayerManager.GetPlayerInfo(playerId);
        if (playerInfo != null) {
            if (state == PanelState.Active) {
                connectionInfo.SetActive(false);
                playerScoreBar.SetActive(false);
                characterSelector.SetActive(SettingManager.ActiveGameMode != SettingManager.GameMode.Random);
                playerScoreInfo.SetActive(true);
            } else if (state == PanelState.Disconnect) {
                connectionInfo.SetActive(true);
                playerScoreBar.SetActive(false);
                characterSelector.SetActive(false);
                playerScoreInfo.SetActive(false);
            } else if (state == PanelState.Score) {
                connectionInfo.SetActive(false);
                playerScoreBar.SetActive(true);
                characterSelector.SetActive(false);
                playerScoreInfo.SetActive(true);
            }
        }
    }
}
