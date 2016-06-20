using UnityEngine;
using System.Collections;

public class UITimeCounter : MonoBehaviour {
    public RectTransform progressBar;
    public RectTransform container;

    public void Awake() {
    }

    private void Update() {
        if (GameManager.ActiveState == GameManager.State.Active) {
            float progress = ScoreManager.Instance.RoundCounter / ScoreManager.Instance.roundLength;

            progressBar.sizeDelta = new Vector2(
                progressBar.sizeDelta.x,
                container.rect.height * progress);
        }
    }
}
