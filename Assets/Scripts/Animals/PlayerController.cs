using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	enum State{
		ACTIVE,
		DISABLED
	}

	State _state;
	PlayerJump playerJump;
	PlayerMove playerMove;
	Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
		playerJump = gameObject.GetComponent<PlayerJump>();
		playerMove = gameObject.GetComponent<PlayerMove>();
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		ChangeState(State.ACTIVE);
	}
	
	// Update is called once per frame
	void Update () {

	}

	//Collision with other animals
	void OnCollisionEnter2D(Collision2D collision){
		if(collision.transform.tag == "Anchor" && _state == State.ACTIVE){
			StickObject(collision);
			ChangeState(State.DISABLED);
			ResetPlayer();
		}
	}

	//Collision with "Grinder"
	void OnTriggerEnter2D(Collider2D collider){
		if(_state == State.ACTIVE){
			ResetPlayer();
		}
		GrindPlayer();
		ChangeState(State.DISABLED);
	}

	void ChangeState(State state){
		_state = state;
	}

	//Spawn a new, controllable character
	void ResetPlayer(){
		playerMove.enabled = false;
		playerJump.enabled = false;
		gameObject.transform.tag = "Anchor";

		int playerNumber = gameObject.GetComponent<InputMapper>().playerNumber;
		GameObject.Find("Spawner_P"+playerNumber).gameObject.GetComponent<PlayerSpawner>().CreateNewPlayer();
	}

	void GrindPlayer(){
		rigidBody.isKinematic = true;
		rigidBody.velocity = new Vector2(0,-10f*Time.deltaTime);
	}

	/*  TODO: Move to own class */
	void StickObject(Collision2D collision){
		GameObject stickyObject = collision.gameObject;

		FixedJoint2D endHinge = gameObject.AddComponent<FixedJoint2D>() as FixedJoint2D;
		endHinge.connectedBody = stickyObject.GetComponent<Rigidbody2D>();
		endHinge.enableCollision = true;
		endHinge.frequency = 5f;
		endHinge.dampingRatio = 1;

		endHinge.anchor = GetVectorOffset(gameObject, collision.gameObject, transform.eulerAngles.z) * 3f;
		endHinge.connectedAnchor = GetVectorOffset(collision.gameObject, gameObject, collision.transform.eulerAngles.z) * 3f;
	}
		
	Vector2 GetVectorOffset(GameObject object1, GameObject object2, float angle){
		var dir = object2.transform.position - object1.transform.position;
		var newDir = Quaternion.AngleAxis(-angle, Vector3.forward) * dir.normalized;
		return newDir;
	}
	/* */

}
