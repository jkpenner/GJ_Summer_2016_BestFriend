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
        GameManager.AddRoundListner(GameManager.RoundEventType.Begin, OnRoundStart);
        GameManager.AddRoundListner(GameManager.RoundEventType.Complete, OnRoundComplete);

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void OnDestroy() {
        GameManager.RemoveRoundListener(GameManager.RoundEventType.Complete, OnRoundComplete);
        GameManager.RemoveRoundListener(GameManager.RoundEventType.Begin, OnRoundStart);
    }

    private void OnRoundStart() {
        canvasGroup.alpha = 0f;
    }

    private void OnRoundComplete() {
        canvasGroup.alpha = 1f;

        if(GameManager.ActiveState == GameManager.State.RoundWin) {
            txtDescription.text = "Players Reached The Finish Line!";
        } else if(GameManager.ActiveState == GameManager.State.RoundLost) {
            txtDescription.text = "Round Timer Expired";
        }
        
        counter = GameManager.Instance.roundResetDuration;
    }

    private void Update() {
        if (GameManager.ActiveState == GameManager.State.RoundLost ||
            GameManager.ActiveState == GameManager.State.RoundWin) {
            counter -= Time.deltaTime;
            txtTimeRemaining.text = counter.ToString("F");
        }
    }
}
