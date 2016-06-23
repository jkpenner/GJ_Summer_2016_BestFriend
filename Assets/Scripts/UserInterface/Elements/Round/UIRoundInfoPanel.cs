using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(AudioSource))]
public class UIRoundInfoPanel : MonoBehaviour {
    public Text txtTitle;
    public Text txtDescription;
    public Text txtTimeRemaining;
    private CanvasGroup canvasGroup;
    private AudioSource audioSource;

    [System.Serializable]
    public class RoundEndInfo {
        public string title;
        public string description;
        public AudioClip audioClip;
    }

    public RoundEndInfo[] victoryInfos;
    public RoundEndInfo[] lostInfos;

    private float counter;

    private void Start() {
        GameManager.AddRoundListner(GameManager.RoundEventType.Begin, OnRoundStart);
        GameManager.AddRoundListner(GameManager.RoundEventType.Complete, OnRoundComplete);

        canvasGroup = GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();
        canvasGroup.alpha = 0f;
    }

    private void OnDestroy() {
        GameManager.RemoveRoundListener(GameManager.RoundEventType.Complete, OnRoundComplete);
        GameManager.RemoveRoundListener(GameManager.RoundEventType.Begin, OnRoundStart);
    }

    private void OnRoundStart() {
        canvasGroup.alpha = 0f;
    }

    private void OnRoundComplete() {
        canvasGroup.alpha = 1f;

        RoundEndInfo selected = null;

        if(GameManager.ActiveState == GameManager.State.RoundWin) {
            selected = victoryInfos[Random.Range(0, victoryInfos.Length)];
        } else if(GameManager.ActiveState == GameManager.State.RoundLost) {
            selected = lostInfos[Random.Range(0, lostInfos.Length)];
        }

        txtTitle.text = selected.title;
        txtDescription.text = selected.description;
        audioSource.PlayOneShot(selected.audioClip);

        counter = GameManager.Instance.roundResetDuration;
    }

    private void Update() {
        if (GameManager.ActiveState == GameManager.State.RoundLost ||
            GameManager.ActiveState == GameManager.State.RoundWin) {
            counter -= Time.deltaTime;
            txtTimeRemaining.text = counter.ToString("F");
        }
    }
}
