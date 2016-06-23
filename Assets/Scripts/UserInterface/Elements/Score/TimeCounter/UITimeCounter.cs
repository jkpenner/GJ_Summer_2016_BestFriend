using UnityEngine;
using System.Collections;

public class UITimeCounter : MonoBehaviour {
    public RectTransform progressBar;
    public RectTransform container;

    public void Awake() {
    }

    private void Update() {
        if (GameManager.ActiveState == GameManager.State.Active) {
            float progress = GameManager.Instance.RoundCounter / GameManager.Instance.roundDuration;

            progressBar.sizeDelta = new Vector2(
                progressBar.sizeDelta.x,
                container.rect.height * progress);
        }
    }
}
