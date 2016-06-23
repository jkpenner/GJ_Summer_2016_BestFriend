using UnityEngine;
using System.Collections;

public class UIFadeOnState : MonoBehaviour {
    private CanvasGroup canvasGroup;

    public float activeFadeValue;
    public float pauseFadeValue;
    public float gameOverFadeValue;
    public float gameWinFadeValue;

    void Awake () {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable() {
        GameManager.AddStateListener(GameManager.StateEventType.Enter, OnGameStateEnter);
    }

    private void OnDisable() {
        Debug.Log("On Disable Called");
        GameManager.RemoveStateListener(GameManager.StateEventType.Enter, OnGameStateEnter);
    }

    private void OnGameStateEnter(GameManager.State state) {
        switch (state) {
            case GameManager.State.Active: canvasGroup.alpha = activeFadeValue; break;
            case GameManager.State.Pause: canvasGroup.alpha = pauseFadeValue; break;
            case GameManager.State.GameOver: canvasGroup.alpha = gameOverFadeValue; break;
            case GameManager.State.GameWin: canvasGroup.alpha = gameWinFadeValue; break;
            default: canvasGroup.alpha = 1f; break;
        }
    }
}
