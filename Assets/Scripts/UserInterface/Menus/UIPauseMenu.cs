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
        GameManager.AddListener(GameManager.EventType.StateEnter, OnStateEnter);
        GameManager.AddListener(GameManager.EventType.StateExit, OnStateExit);
    }

    private void OnDisable() {
        GameManager.RemoveListener(GameManager.EventType.StateEnter, OnStateEnter);
        GameManager.RemoveListener(GameManager.EventType.StateExit, OnStateExit);
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
        if (Input.GetButtonDown(PlayerManager.GetPlayerInputStr(1, "Start")) ||
            Input.GetButtonDown(PlayerManager.GetPlayerInputStr(2, "Start")) ||
            Input.GetButtonDown(PlayerManager.GetPlayerInputStr(3, "Start")) ||
            Input.GetButtonDown(PlayerManager.GetPlayerInputStr(4, "Start"))){
            if (GameManager.ActiveState == GameManager.State.Active ||
                GameManager.ActiveState == GameManager.State.None) {
                GameManager.SetState(GameManager.State.Pause);                
            } else if (GameManager.ActiveState == GameManager.State.Pause) {
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
