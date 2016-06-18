using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	public float walkSpeed = 10f;
	public float walkForce = 100f;

	Rigidbody2D rigidBody;
	string moveInput;

	// Use this for initialization
	void Start () {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		moveInput = gameObject.GetComponent<InputMapper>().GetMappedInput("Horizontal");
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}

	void Move(){
		if(Input.GetButton(moveInput)){
			var inputValue = Input.GetAxis(moveInput);
			if((inputValue > 0 && rigidBody.velocity.x < walkSpeed) || (inputValue < 0 && rigidBody.velocity.x > -walkSpeed)){
				rigidBody.AddForce(new Vector2(inputValue * walkForce, 0f));
			}
		}
	}
}
