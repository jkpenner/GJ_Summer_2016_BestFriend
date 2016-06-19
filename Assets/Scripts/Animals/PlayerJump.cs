using UnityEngine;
using System.Collections;

public class PlayerJump : MonoBehaviour {

	public float jumpForce = 200f;

	Rigidbody2D rigidBody;
	string jumpInput;
	bool canJump = true;

	// Use this for initialization
	void Start () {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		jumpInput = gameObject.GetComponent<InputMapper>().GetMappedInput("Jump");
	}

	//Collision with other animals 
	void OnCollisionEnter2D(Collision2D collision){ 
		AllowJump();
	}

	public void AllowJump(){
		canJump = true;
	}
	
	// Update is called once per frame
	void Update () {
		Jump();
	}

	void Jump(){
		if(Input.GetButtonDown(jumpInput) && canJump){
			rigidBody.AddForce(new Vector2(0f, jumpForce));
			canJump = false;
		}
	}
}
