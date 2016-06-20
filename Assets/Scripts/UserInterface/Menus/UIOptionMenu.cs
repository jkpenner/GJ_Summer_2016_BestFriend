using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIOptionMenu : MonoBehaviour, IUIMenu {
    private CanvasGroup canvasGroup;

    public Button btnIncreaseQuality;
    public Button btnDecreaseQuality;

    public Button btnReturn;

    private IUIMenu previous;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        OnMenuDeactivate();

        btnIncreaseQuality.onClick.AddListener(OnIncreaseQualityClick);
        btnDecreaseQuality.onClick.AddListener(OnDescreaseQualityClick);
        btnReturn.onClick.AddListener(OnReturnClick);
    }

    private void OnReturnClick() {
        previous.OnMenuActivate();
        gameObject.SetActive(false);

        OnMenuDeactivate();
    }

    private void OnDescreaseQualityClick() {
        QualitySettings.DecreaseLevel();
    }

    private void OnIncreaseQualityClick() {
        QualitySettings.IncreaseLevel();
    }

    public void Display(IUIMenu enableOnExit) {
        gameObject.SetActive(true);
        previous = enableOnExit;

        OnMenuActivate();
    }

    public void OnMenuActivate() {
        btnReturn.Select();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
    }

    public void OnMenuDeactivate() {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
    }
}
