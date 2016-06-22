using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class UIScoreMenu : MonoBehaviour, IUIMenu {
    [System.Serializable]
    public class ScoreBarInfo {
        public RectTransform rtBarPanel;

        public RectTransform rtSscoreBar;
        public RectTransform rtScoreBarContainer;
        public Text txtScoreValue;

        public Image imgOutline;
        public Image imgScoreBar;
    }

    [System.Serializable]
    public class RoundInfo {
        public Text txtRoundName;
        public Text txtRoundTime;
        public Text txtRoundScore;
    }

    public ScoreBarInfo[] playerScoreBars;
    public ScoreBarInfo teamScoreBar;

    public RoundInfo[] roundInfos;

    public Button btnReplay;
    public Button btnMainMenu;

    public float timeToPopulate = 1f;

    private void Start() {
        btnReplay.onClick.AddListener(OnReplayClick);
        btnMainMenu.onClick.AddListener(OnMainMenuClick);

        OnMenuActivate();

        // Disable the buttons till scores populate
        btnReplay.interactable = false;
        btnMainMenu.interactable = false;

        if (SettingManager.ActivePlayerCount == SettingManager.PlayerCount.TwoPlayer) {
            playerScoreBars[2].rtBarPanel.gameObject.SetActive(false);
            playerScoreBars[3].rtBarPanel.gameObject.SetActive(false);
        }

        StartCoroutine("PopulateScores");
    }

    IEnumerator PopulateScores() {
        float counter = timeToPopulate;
        while (counter > 0f) {
            counter -= Time.deltaTime;

            float percentComplete = (timeToPopulate - counter) / timeToPopulate;

            int players = SettingManager.ActivePlayerCount == SettingManager.PlayerCount.TwoPlayer ? 2 : 4;

            // find the total and max values of player scores
            int max = 0, total = 0;
            for (int i = 0; i < players; i++) {
                total += StorageManager.PlayerScores[i];
                max = Mathf.Max(max, StorageManager.PlayerScores[i]);
            }

            Debug.Log(max);

            // Populate the player scores
            for (int i = 0; i < players; i++) {
                PopulateScoreBar(playerScoreBars[i], StorageManager.PlayerScores[i], max, percentComplete, PlayerManager.GetPlayerInfo((PlayerId)(i + 1)).Color);
            }

            // Populate the Team Score
            PopulateScoreBar(teamScoreBar, total, total, percentComplete, Color.white);

            // Popultate Rounds
            for (int i = 0; i < 3; i++) {
                PopulateRoundInfo(roundInfos[i], StorageManager.RoundTimes[i], StorageManager.RoundScores[i], percentComplete);
            }

            yield return new WaitForEndOfFrame();
        }

        btnReplay.interactable = true;
        btnMainMenu.interactable = true;
        Destroy(StorageManager.Instance.gameObject);
    }

    private void PopulateScoreBar(ScoreBarInfo info, float targetValue, float maxValue, float percent, Color color) {
        float barProgress = (targetValue / maxValue) * percent;
        info.rtSscoreBar.sizeDelta = new Vector2(
            info.rtSscoreBar.sizeDelta.x,
            info.rtScoreBarContainer.rect.height * barProgress);
        info.txtScoreValue.text = (Mathf.RoundToInt(targetValue * percent)).ToString("D8");

        info.imgOutline.color = color;
        info.imgScoreBar.color = color;
    }

    private void PopulateRoundInfo(RoundInfo info, float time, float score, float percent) {
        info.txtRoundTime.text = (time * percent).ToString("F") + "s";
        info.txtRoundScore.text = (Mathf.RoundToInt(score * percent)).ToString("D8");
    }

    private void OnReplayClick() {
        //Reload the Main Game Scene
        SceneManager.LoadScene(1);
    }

    private void OnMainMenuClick() {
        //Load the Main Menu
        SceneManager.LoadScene(0);
    }

    public void OnMenuActivate() {
        btnReplay.Select();
    }

    public void OnMenuDeactivate() {
        
    }
}
