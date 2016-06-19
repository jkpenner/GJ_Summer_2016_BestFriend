using UnityEngine;
using System.Collections;

public class DashController : MonoBehaviour {

	public float dashVelocity = 40f;

	Rigidbody2D rigidBody;
	string dashInput;
	string dashDirectionInput;
	bool canDash = false;

	// Use this for initialization
	void Start () {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		dashInput = gameObject.GetComponent<InputMapper>().GetMappedInput("Jump");
		dashDirectionInput = gameObject.GetComponent<InputMapper>().GetMappedInput("Horizontal");
	}

	// Update is called once per frame
	void Update () {
		Dash();
	}

	//Collision with other animals
	void OnCollisionEnter2D(Collision2D collision){
		canDash = true;
	}

	void OnTriggerEnter2D(Collider2D collider){
		canDash = false;
	}

	void Dash(){
		float dashDirection = Input.GetAxis(dashDirectionInput);
		if(Input.GetButtonDown(dashInput) && dashDirection != 0 && Mathf.Abs(rigidBody.velocity.y) > 0.1 && canDash){
			Debug.Log("dash");
			rigidBody.velocity = new Vector2(dashVelocity*dashDirection, 0);
			canDash = false;
		}
	}
}
