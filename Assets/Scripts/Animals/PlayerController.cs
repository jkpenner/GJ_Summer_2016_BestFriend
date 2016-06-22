using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
	enum State{
		ACTIVE,
		DISABLED
	}

	public enum Animal{
		CHICKEN,
		COW,
		PIG
	}

	public Animal animal;
	public float flightForce = 400f; 
	public float grindSpeed = 15f;
    public int scoreValue = 1;

    private bool firstStick = true;
    private bool firstGrind = true;
    private float activeGrindSpeed = 0;
    private Vector2 pauseVelocityStored;

    State _state;
	PlayerJump playerJump;
	PlayerMove playerMove;
	PlayerDive playerDive;
	PlayerFly playerFly;
	PlayerDash playerDash;
	Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
		playerJump = gameObject.GetComponent<PlayerJump>();
		playerMove = gameObject.GetComponent<PlayerMove>();
		playerDive = gameObject.GetComponent<PlayerDive>();
		playerFly = gameObject.GetComponent<PlayerFly>();
		playerDash = gameObject.GetComponent<PlayerDash>();
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		ChangeState(State.ACTIVE);

        activeGrindSpeed = grindSpeed;

        // Listen to Manager Events
        GameManager.AddListener(GameManager.EventType.StateEnter, OnGameStateEnter);
        GameManager.AddListener(GameManager.EventType.StateExit, OnGameStateExit);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);
        ScoreManager.AddListener(ScoreManager.RoundEventType.Complete, OnRoundComplete);
	}

    private void OnRoundComplete() {
        // When the round complete kill everything
        // Updates the active grind speed for fast killing
        activeGrindSpeed = grindSpeed * 15;
        rigidBody.velocity = Vector2.down * activeGrindSpeed * Time.deltaTime;

        // if the animal is not in the grinder
        if (_state == State.ACTIVE) {
            // Spawn a partical effect
            Destroy(this.gameObject);
        }
    }

    private void OnGameStateExit(GameManager.State state) {
        if (state == GameManager.State.Pause) {
            rigidBody.isKinematic = false;
            rigidBody.velocity = pauseVelocityStored;            
        }

        if (state == GameManager.State.RoundLost) {
            activeGrindSpeed = grindSpeed;
        }
    }

    private void OnGameStateEnter(GameManager.State state) {
        if (state == GameManager.State.Pause) {
            rigidBody.isKinematic = true;
            pauseVelocityStored = rigidBody.velocity;
            rigidBody.velocity = Vector2.zero;
        }
    }

    private void OnPlayerDisconnect(PlayerInfo player) {
        if (player.Id == GetComponent<InputMapper>().playerId) {
            if (_state == State.ACTIVE) {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnDestroy() {
        GameManager.RemoveListener(GameManager.EventType.StateEnter, OnGameStateEnter);
        GameManager.RemoveListener(GameManager.EventType.StateExit, OnGameStateExit);
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);
        ScoreManager.RemoveListener(ScoreManager.RoundEventType.Complete, OnRoundComplete);
    }

    // Update is called once per frame
    void Update () {
		//TODO: move chicken flight to own class
		if(_state == State.DISABLED && animal == Animal.CHICKEN){
			rigidBody.AddForce(Vector2.up * flightForce);
		}
	}

	//Collision with other animals
	void OnCollisionEnter2D(Collision2D collision){
		if(collision.transform.tag == "Anchor"){
			if(_state == State.ACTIVE){
				ChangeState(State.DISABLED);
				ResetPlayer();
			}
			foreach(FixedJoint2D fixedJoint in gameObject.GetComponents<FixedJoint2D>()){
				if(fixedJoint.connectedBody == collision.gameObject) return;
			}
			StickObject(collision);
		}
	}

	//Collision with "Grinder"
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.CompareTag("Destroyer")){
			Destroy(gameObject);
		}else if(collider.CompareTag("Grinder")){
            if (_state == State.ACTIVE)
            {
                ResetPlayer();
            }
            GrindPlayer();
		}
		
		ChangeState(State.DISABLED);
	}

	void ChangeState(State state){
		_state = state;
	}

	//Spawn a new, controllable character
	void ResetPlayer(){
		playerMove.enabled = false;
		playerJump.enabled = false;
		if(playerDive) playerDive.enabled = false;
		if(playerFly) playerFly.enabled = false;
		if(playerDash) playerDash.enabled = false;
		gameObject.transform.tag = "Anchor";

		
		//GameObject.Find("Spawner_P"+(int)playerId).gameObject.GetComponent<PlayerSpawner>().CreateNewPlayer();
        SpawnManager.SpawnPlayer(this.GetComponent<InputMapper>().playerId);
	}

	void GrindPlayer(){
		rigidBody.isKinematic = true;
		rigidBody.velocity = Vector2.down * activeGrindSpeed * Time.deltaTime;
		gameObject.GetComponentInChildren<Animator>().SetBool("isGrinding", true);
        if (firstGrind && GameManager.ActiveState == GameManager.State.Active) {
            firstGrind = false;
            ScoreManager.ModifyPlayerScore(GetComponent<InputMapper>().playerId, -scoreValue);
        }
    }

	/*  TODO: Move to own class */
	void StickObject(Collision2D collision){
		GameObject stickyObject = collision.gameObject;
		gameObject.GetComponentInChildren<Animator>().SetBool("isStuck", true);

		FixedJoint2D endHinge = gameObject.AddComponent<FixedJoint2D>() as FixedJoint2D;
		endHinge.connectedBody = stickyObject.GetComponent<Rigidbody2D>();
		endHinge.enableCollision = true;
		endHinge.frequency = 5f;
		endHinge.dampingRatio = 1;

		endHinge.anchor = GetVectorOffset(gameObject, collision.gameObject, transform.eulerAngles.z) * gameObject.GetComponent<CircleCollider2D>().radius;
		endHinge.connectedAnchor = GetVectorOffset(collision.gameObject, gameObject, collision.transform.eulerAngles.z) * collision.gameObject.GetComponent<CircleCollider2D>().radius;

        if (firstStick == true) {
            firstStick = false;
            if (ScoreManager.Instance.winTransform.position.y < transform.position.y) {
                ScoreManager.ModifyPlayerScore(GetComponent<InputMapper>().playerId, scoreValue * 10);
                ScoreManager.EndRound(true);
            } else {
                ScoreManager.ModifyPlayerScore(GetComponent<InputMapper>().playerId, scoreValue);
            }
        }
	}
		
	Vector2 GetVectorOffset(GameObject object1, GameObject object2, float angle){
		var dir = object2.transform.position - object1.transform.position;
		var newDir = Quaternion.AngleAxis(-angle, Vector3.forward) * dir.normalized;
		return newDir;
	}
	/* */

}
