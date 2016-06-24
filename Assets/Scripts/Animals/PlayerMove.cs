using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	public float walkSpeed = 10f;
	public float walkForce = 100f;

	Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Move();
	}

	void Move(){
        var inputValue = Input.GetAxis(PlayerManager.GetPlayerInputStr(gameObject.GetComponent<InputMapper>().playerId, "LS_Horizontal"));
        if(inputValue != 0.0f) {
			if((inputValue > 0 && rigidBody.velocity.x < walkSpeed) || (inputValue < 0 && rigidBody.velocity.x > -walkSpeed)){
				rigidBody.AddForce(new Vector2(inputValue * walkForce, 0f));
			}
		}
	}
}
