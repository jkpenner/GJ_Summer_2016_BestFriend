using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIPauseMenu : MonoBehaviour, IUIMenu {
    private CanvasGroup canvasGroup;

    public Button btnResume;
    public Button btnOptions;
    public Button btnMainMenu;

    public UIOptionMenu optionMenu;

    public KeyCode pauseKeyCode = KeyCode.Escape;

    private void Start() {
        canvasGroup = GetComponent<CanvasGroup>();
        OnMenuDeactivate();

        btnResume.onClick.AddListener(OnResumeClick);
        btnOptions.onClick.AddListener(OnOptionsClick);
        btnMainMenu.onClick.AddListener(OnMainMenuClick);
    }

    private void OnEnable() {
        GameManager.AddStateListener(GameManager.StateEventType.Enter, OnStateEnter);
        GameManager.AddStateListener(GameManager.StateEventType.Exit, OnStateExit);
    }

    private void OnDisable() {
        GameManager.RemoveStateListener(GameManager.StateEventType.Enter, OnStateEnter);
        GameManager.RemoveStateListener(GameManager.StateEventType.Exit, OnStateExit);
    }

    private void OnStateExit(GameManager.State state) {
        if (state == GameManager.State.Pause) {
            OnMenuDeactivate();
        }
    }

    private void OnStateEnter(GameManager.State state) {
        if (state == GameManager.State.Pause) {
            OnMenuActivate();
        }
    }

    private void Update() {
        if (Input.GetButtonDown(PlayerManager.GetPlayerInputStr(PlayerId.One, "Start")) ||
            Input.GetButtonDown(PlayerManager.GetPlayerInputStr(PlayerId.Two, "Start")) ||
            Input.GetButtonDown(PlayerManager.GetPlayerInputStr(PlayerId.Three, "Start")) ||
            Input.GetButtonDown(PlayerManager.GetPlayerInputStr(PlayerId.Four, "Start"))){
            if (GameManager.ActiveState == GameManager.State.Active ||
                GameManager.ActiveState == GameManager.State.None) {
                GameManager.SetState(GameManager.State.Pause);                
            } else if (GameManager.ActiveState == GameManager.State.Pause &&
                canvasGroup.alpha == 1) {
                GameManager.SetState(GameManager.State.Active);
            }
        }
    }

    private void OnMainMenuClick() {
        SceneManager.LoadScene(0);
    }

    private void OnOptionsClick() {
        optionMenu.Display(this);
        OnMenuDeactivate();
    }

    private void OnResumeClick() {
        GameManager.SetState(GameManager.State.Active);
        OnMenuDeactivate();
    }

    public void OnMenuActivate() {
        btnResume.Select();
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1;
    }

    public void OnMenuDeactivate() {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
    }
}
