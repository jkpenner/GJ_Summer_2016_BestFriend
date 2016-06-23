using UnityEngine;
using UnityEngine.UI;

public class UIRoundPanel : MonoBehaviour {
    public Animator animator;
    public Text txtRound;

	void Start () {
        GameManager.AddRoundListner(GameManager.RoundEventType.Begin, OnRoundStart);
        GameManager.AddRoundListner(GameManager.RoundEventType.Complete, OnRoundComplete);
        txtRound.text = "";
	}

    private void OnRoundComplete() {
        
    }

    private void OnRoundStart() {
        //if (ScoreManager.Instance.RoundsPerGame <= ScoreManager.Instance.CurrentRound) {
        //    txtRound.text = "Final Round!";
        //} else {
        //    txtRound.text = string.Format("Round {0}!", ScoreManager.Instance.CurrentRound.ToString());
        //}
        //
        //
        //animator.SetTrigger("Reveal");
    }
}
