using UnityEngine;
using System.Collections;

public class PlayerJump : MonoBehaviour {

	public float jumpForce = 200f;

	Rigidbody2D rigidBody;
	string jumpInput;

	// Use this for initialization
	void Start () {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		jumpInput = gameObject.GetComponent<InputMapper>().GetMappedInput("Jump");
	}
	
	// Update is called once per frame
	void Update () {
		Jump();
	}

	void Jump(){
		if(Input.GetButtonDown(jumpInput)){
			Debug.Log("jump");
			rigidBody.AddForce(new Vector2(0f, jumpForce));
		}
	}
}
