using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class UIMainMenu : MonoBehaviour {
    public Button btnFindGame;
    public Button btnOptions;
    public Button btnExitGame;

    public UIOptionMenu optionMenu;

	void Start () {
        btnFindGame.onClick.AddListener(OnFindGameClick);
        btnOptions.onClick.AddListener(OnOptionsClick);
        btnExitGame.onClick.AddListener(OnExitGameClick);	
	}

    void OnEnable() {
        btnFindGame.Select();
    }

    private void OnFindGameClick() {
        GameManager.ActiveState = GameManager.State.Active;
        SceneManager.LoadScene(1);
    }

    private void OnOptionsClick() {
        optionMenu.Display(this.gameObject);
    }

    private void OnExitGameClick() {
        Application.Quit();
    }

    
}
