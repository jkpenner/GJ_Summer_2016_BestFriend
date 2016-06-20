using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIMainMenu : MonoBehaviour, IUIMenu {
    public Button btnGame2PNormal;
    public Button btnGame2PRandom;
    public Button btnGame4PNormal;
    public Button btnGame4PRandom;

    public Button btnOptions;
    public Button btnExitGame;

    public UIOptionMenu optionMenu;

    private CanvasGroup canvasGroup;

    void Start () {
        btnGame2PNormal.onClick.AddListener(OnGame2PNormalClick);
        btnGame2PRandom.onClick.AddListener(OnGame2PRandomClick);
        btnGame4PNormal.onClick.AddListener(OnGame4PNormalClick);
        btnGame4PRandom.onClick.AddListener(OnGame4PRandomClick);
        btnOptions.onClick.AddListener(OnOptionsClick);
        btnExitGame.onClick.AddListener(OnExitGameClick);	

        canvasGroup = GetComponent<CanvasGroup>();
        OnMenuActivate();
	}
    void OnEnable() {
        
    }

    private void OnGame2PNormalClick() {
        SettingManager.Set(SettingManager.PlayerCount.TwoPlayer, SettingManager.GameMode.Normal);
        SceneManager.LoadScene("MainGame");
    }

    private void OnGame2PRandomClick() {
        SettingManager.Set(SettingManager.PlayerCount.TwoPlayer, SettingManager.GameMode.Random);
        SceneManager.LoadScene("MainGame");
    }

    private void OnGame4PNormalClick() {
        SettingManager.Set(SettingManager.PlayerCount.FourPlayer, SettingManager.GameMode.Normal);
        SceneManager.LoadScene("MainGame");
    }

    private void OnGame4PRandomClick() {
        SettingManager.Set(SettingManager.PlayerCount.FourPlayer, SettingManager.GameMode.Random);
        SceneManager.LoadScene("MainGame");
    }

    //private void OnFindGameClick() {
    //    GameManager.ActiveState = GameManager.State.Active;
    //    SceneManager.LoadScene(1);
    //}

    private void OnOptionsClick() {
        optionMenu.Display(this);
        OnMenuDeactivate();
    }

    private void OnExitGameClick() {
        Application.Quit();
    }

    public void OnMenuActivate() {
        btnGame2PNormal.Select();
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
