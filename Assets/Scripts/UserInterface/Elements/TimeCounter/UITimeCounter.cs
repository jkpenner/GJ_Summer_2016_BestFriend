using UnityEngine;
using System.Collections;

public class UITimeCounter : MonoBehaviour {
    public RectTransform progressBar;

    private void Update() {
        progressBar.localScale = new Vector3(
            progressBar.localScale.x,
            ScoreManager.Instance.RoundCounter / ScoreManager.Instance.roundLength,
            progressBar.localScale.z);
    }
}
