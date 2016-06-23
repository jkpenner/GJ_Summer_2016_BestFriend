using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIMainMenu : MonoBehaviour, IUIMenu {
    private CanvasGroup canvasGroup;

    public GameObject mainMenu;
    public GameObject nextMenu;

    bool mainMenuShow = true;


    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();

        PlayerManager.AddListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);

        OnMenuActivate();

        mainMenu.SetActive(true);
        nextMenu.SetActive(false);
    }

    private void Update() {
        if (Input.GetButtonDown(PlayerManager.GetPlayerInputStr(PlayerId.One, "A")) ||
            Input.GetButtonDown(PlayerManager.GetPlayerInputStr(PlayerId.Two, "A")) ||
            Input.GetButtonDown(PlayerManager.GetPlayerInputStr(PlayerId.Three, "A")) ||
            Input.GetButtonDown(PlayerManager.GetPlayerInputStr(PlayerId.Four, "A"))) {
            if (mainMenuShow) {
                Debug.Log("Switch Menu");
                nextMenu.SetActive(true);
                mainMenu.SetActive(false);
                mainMenuShow = false;
            } else {
                Debug.Log("Start Game");
                SettingManager.Set(SettingManager.PlayerCount.FourPlayer, SettingManager.GameMode.Random);
                SceneManager.LoadScene("MainGame");
            }
            //OnMenuDeactivate();
        }
    }

    private void OnDisable() {
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
    }

    private void OnPlayerConnect(PlayerInfo player) {
        //nextMenu.SetActive(true);
        //mainMenu.SetActive(false);
    }

    public void OnMenuActivate() {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnMenuDeactivate() {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
