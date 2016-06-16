using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIOptionMenu : MonoBehaviour {
    private CanvasGroup canvasGroup;

    public Button btnIncreaseQuality;
    public Button btnDecreaseQuality;

    public Button btnReturn;

    private GameObject previous;

    private void Start() {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;

        btnIncreaseQuality.onClick.AddListener(OnIncreaseQualityClick);
        btnDecreaseQuality.onClick.AddListener(OnDescreaseQualityClick);
        btnReturn.onClick.AddListener(OnReturnClick);
    }

    private void OnReturnClick() {
        previous.SetActive(true);
        gameObject.SetActive(false);

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }

    private void OnDescreaseQualityClick() {
        QualitySettings.DecreaseLevel();
    }

    private void OnIncreaseQualityClick() {
        QualitySettings.IncreaseLevel();
    }

    public void Display(GameObject enableOnExit) {
        gameObject.SetActive(true);
        previous = enableOnExit;
        previous.SetActive(false);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
    }
}
