using UnityEngine;
using UnityEngine.UI;

public class UIRoundPanel : MonoBehaviour {
    public Animator animator;
    public Text txtRound;

	void Start () {
	    ScoreManager.AddListener(ScoreManager.RoundEventType.Start, OnRoundStart);
        ScoreManager.AddListener(ScoreManager.RoundEventType.Complete, OnRoundComplete);
        txtRound.text = "";
	}

    private void OnRoundComplete() {
        
    }

    private void OnRoundStart() {
        if (ScoreManager.Instance.RoundsPerGame <= ScoreManager.Instance.CurrentRound) {
            txtRound.text = "Final Round!";
        } else {
            txtRound.text = string.Format("Round {0}!", ScoreManager.Instance.CurrentRound.ToString());
        }


        animator.SetTrigger("Reveal");
    }
}
