using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class UIMainMenu : MonoBehaviour {
    public Button btnFindGame;
    public Button btnOptions;
    public Button btnExitGame;

	void Start () {
        btnFindGame.onClick.AddListener(OnFindGameClick);
        btnOptions.onClick.AddListener(OnOptionsClick);
        btnExitGame.onClick.AddListener(OnExitGameClick);	
	}

    private void OnFindGameClick() {
        GameManager.ActiveState = GameManager.State.Active;
        SceneManager.LoadScene(1);
    }

    private void OnOptionsClick() {
        
    }

    private void OnExitGameClick() {
        Application.Quit();
    }

    
}
