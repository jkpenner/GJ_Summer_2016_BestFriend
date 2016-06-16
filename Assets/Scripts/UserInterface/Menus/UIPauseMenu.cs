using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIPauseMenu : MonoBehaviour {
    private CanvasGroup canvasGroup;

    public Button btnResume;
    public Button btnOptions;
    public Button btnMainMenu;

    public KeyCode pauseKeyCode = KeyCode.Escape;

    private void Start() {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;

        btnResume.onClick.AddListener(OnResumeClick);
        btnOptions.onClick.AddListener(OnOptionsClick);
        btnMainMenu.onClick.AddListener(OnMainMenuClick);

        GameManager.AddListener(GameManager.EventType.StateEnter, OnStateEnter);
        GameManager.AddListener(GameManager.EventType.StateExit, OnStateExit);
    }

    private void OnStateExit(GameManager.State state) {
        Debug.Log("On State Enter: " + state.ToString());
    }

    private void OnStateEnter(GameManager.State state) {
        Debug.Log("On State Exit: " + state.ToString());
    }

    private void Update() {
        if (Input.GetKeyDown(pauseKeyCode)) {
            if (GameManager.ActiveState == GameManager.State.Active ||
                GameManager.ActiveState == GameManager.State.None) {
                GameManager.SetState(GameManager.State.Pause);
                canvasGroup.interactable = true;
                canvasGroup.alpha = 1;
            } else if (GameManager.ActiveState == GameManager.State.Pause) {
                GameManager.SetState(GameManager.State.Active);
                canvasGroup.interactable = false;
                canvasGroup.alpha = 0;
            }
        }
    }

    private void OnMainMenuClick() {
        SceneManager.LoadScene(0);
    }

    private void OnOptionsClick() {
        
    }

    private void OnResumeClick() {
        GameManager.SetState(GameManager.State.Active);
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
    }
}
