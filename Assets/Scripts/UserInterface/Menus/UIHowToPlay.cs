using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class UIHowToPlay : MonoBehaviour, IUIMenu {
    private CanvasGroup canvasGroup;

    public IUIMenu nextMenu;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        OnMenuDeactivate();
    }

    private void Update() {
        for (int i = 1; i < 5; i++) {
            if (canvasGroup.alpha == 1 && (Input.GetButtonDown(PlayerManager.GetPlayerInputStr((PlayerId)i, "A")) || Input.anyKeyDown)) {
                SettingManager.Set(SettingManager.PlayerCount.FourPlayer, SettingManager.GameMode.Normal);
                SceneManager.LoadScene("MainGame");
            }
        }
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
