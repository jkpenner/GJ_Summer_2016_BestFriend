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
        public Text txtRoundTime;
        public Text txtRoundScore;
    }

    public ScoreBarInfo[] playerScoreBars;
    public ScoreBarInfo teamScoreBar;

    public RoundInfo[] leaderboardTeam;
    public RoundInfo[] leaderboardSolo;

    public Text txtTimeTillNext;

    public float timeToPopulate = 1f;
    public float timeToTransition = 5f;
    private float timeCounter = 0;

    private void Start() {
        OnMenuActivate();

        if (SettingManager.ActivePlayerCount == SettingManager.PlayerCount.TwoPlayer) {
            playerScoreBars[2].rtBarPanel.gameObject.SetActive(false);
            playerScoreBars[3].rtBarPanel.gameObject.SetActive(false);
        }

        StartCoroutine("PopulateScores");

        timeCounter = timeToTransition;
    } 

    public void Update() {
        timeCounter -= Time.deltaTime;
        if (timeCounter < 0) {
            timeCounter = 0;
            if (PlayerManager.IsPlayerConnected()) {
                SceneManager.LoadScene("MainGame");
            } else {
                SceneManager.LoadScene("MainMenu");
            }
        }

        txtTimeTillNext.text = string.Format("Next Round Starts in... {0}s",
            timeCounter.ToString("F")); 
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
                total += StorageManager.ActivePlayerScores[i];
                max = Mathf.Max(max, StorageManager.ActivePlayerScores[i]);
            }

            Debug.Log(max);

            // Populate the player scores
            for (int i = 0; i < players; i++) {
                PopulateScoreBar(playerScoreBars[i], StorageManager.ActivePlayerScores[i], max, percentComplete, PlayerManager.GetPlayerInfo((PlayerId)(i + 1)).Color);
            }

            // Populate the Team Score
            PopulateScoreBar(teamScoreBar, total, total, percentComplete, Color.white);

            // Popultate Rounds
            for (int i = 0; i < 5; i++) {
                PopulateRoundInfo(leaderboardTeam[i], StorageManager.LeaderboardTeam[i].time, StorageManager.LeaderboardTeam[i].score, percentComplete);
            }

            for (int i = 0; i < 5; i++) {
                PopulateRoundInfo(leaderboardSolo[i], StorageManager.LeaderboardSolo[i].time, StorageManager.LeaderboardSolo[i].score, percentComplete);
            }

            yield return new WaitForEndOfFrame();
        }

        StorageManager.ActivePlayerScores.Clear();
        StorageManager.ActiveRoundScores = 0;
        StorageManager.ActiveRoundTimes = 0f;
        //Destroy(StorageManager.Instance.gameObject);
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

    public void OnMenuActivate() {
        
    }

    public void OnMenuDeactivate() {
        
    }
}
