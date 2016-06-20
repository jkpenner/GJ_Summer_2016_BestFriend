using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIRoundInfoPanel : MonoBehaviour {
    public Text txtDescription;
    public Text txtTimeRemaining;
    private CanvasGroup canvasGroup;

    private float counter;

    private void Start() {
        ScoreManager.AddListener(ScoreManager.RoundEventType.Complete, OnRoundComplete);
        ScoreManager.AddListener(ScoreManager.RoundEventType.Start, OnRoundStart);

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void OnDestroy() {
        ScoreManager.RemoveListener(ScoreManager.RoundEventType.Complete, OnRoundComplete);
        ScoreManager.RemoveListener(ScoreManager.RoundEventType.Start, OnRoundStart);
    }

    private void OnRoundStart() {
        canvasGroup.alpha = 0f;
    }

    private void OnRoundComplete() {
        canvasGroup.alpha = 1f;

        if (ScoreManager.Instance.RoundCounter > 0) {
            txtDescription.text = "Players Reacted The Finish Line!";
        } else {
            txtDescription.text = "Round Timer Expired";
        }
        
        counter = GameManager.Instance.resetStateDuration;
    }

    private void Update() {
        if (GameManager.ActiveState == GameManager.State.Reset) {
            counter -= Time.deltaTime;
            txtTimeRemaining.text = counter.ToString("F");
        }
    }
}
