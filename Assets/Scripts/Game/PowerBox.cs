using UnityEngine;
using System.Collections;

public class PowerBox : MonoBehaviour {

	public Color32 redColor;
	public Color32 greenColor;
	public Color32 neutralColor;

	Renderer redButton;
	Renderer greenButton;
	Light light;

	// Use this for initialization
	void Start () {
		redButton = transform.Find("RedSwitch").GetComponent<Renderer>();
		greenButton = transform.Find("GreenSwitch").GetComponent<Renderer>();
		light = transform.Find("RedLight").GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.ActiveState == GameManager.State.RoundWin){
			greenButton.material.color = greenColor;
			redButton.material.color = neutralColor;
			light.color = greenColor;
		}else{
			greenButton.material.color = neutralColor;
			redButton.material.color = redColor;
			light.color = redColor;
		}
	}
}
