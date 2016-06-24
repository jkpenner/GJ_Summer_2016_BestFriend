using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
	enum State{
		ACTIVE,
		DISABLED,
		SAVED

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
	public float vaccumForce = 500f;
	public AudioClip spawnSound;
	public AudioClip stickSound;

    private bool firstStick = true;
    private bool firstGrind = true;
    private float activeGrindSpeed = 0;
    private Vector2 pauseVelocityStored;
	private Light playerAura;

    State _state;
	PlayerJump playerJump;
	PlayerMove playerMove;
	PlayerDive playerDive;
	PlayerFly playerFly;
	PlayerDash playerDash;
	Rigidbody2D rigidBody;
	ParticleSystem bloodParticles;

	// Use this for initialization
	void Start () {
		playerJump = gameObject.GetComponent<PlayerJump>();
		playerMove = gameObject.GetComponent<PlayerMove>();
		playerDive = gameObject.GetComponent<PlayerDive>();
		playerFly = gameObject.GetComponent<PlayerFly>();
		playerDash = gameObject.GetComponent<PlayerDash>();
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		playerAura = gameObject.GetComponent<Light>();
		bloodParticles = gameObject.GetComponentInChildren<ParticleSystem>();
		ChangeState(State.ACTIVE);

		playerAura.color = PlayerManager.GetPlayerInfo(GetComponent<InputMapper>().playerId).Color;

        activeGrindSpeed = grindSpeed;

        // Listen to Manager Events
        GameManager.AddStateListener(GameManager.StateEventType.Enter, OnGameStateEnter);
        GameManager.AddStateListener(GameManager.StateEventType.Exit, OnGameStateExit);
        GameManager.AddRoundListner(GameManager.RoundEventType.Complete, OnRoundComplete);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);
        

		//Give initial velocity
		rigidBody.velocity = Vector2.right * (transform.position.x < 0 ? 1 : -1) * 5f;
		SoundManager.PlaySoundEffect(spawnSound);
	}

    private void OnRoundComplete() {
		//Win
		if(GameManager.ActiveState == GameManager.State.RoundWin){
			ChangeState(State.SAVED);
			rigidBody.isKinematic = false;
			bloodParticles.Stop();
			foreach(FixedJoint2D fixedJoint in gameObject.GetComponents<FixedJoint2D>()){
				Destroy(fixedJoint);
			}
		}
		//Lose
		else{
			// When the round complete kill everything
			// Updates the active grind speed for fast killing
			activeGrindSpeed = grindSpeed * 15;
            rigidBody.velocity = Vector2.down * activeGrindSpeed * Time.deltaTime;

	        // if the animal is not in the grinder
			ChangeState(State.DISABLED);
		}

        if (_state == State.ACTIVE) {
            // Spawn a partical effect
            Destroy(gameObject);
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
        GameManager.RemoveStateListener(GameManager.StateEventType.Enter, OnGameStateEnter);
        GameManager.RemoveStateListener(GameManager.StateEventType.Exit, OnGameStateExit);
        GameManager.RemoveRoundListener(GameManager.RoundEventType.Complete, OnRoundComplete);
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);
        
    }

    // Update is called once per frame
    void Update () {
		//TODO: move chicken flight to own class
		if(_state == State.DISABLED && animal == Animal.CHICKEN && GameManager.ActiveState == GameManager.State.Active){
			rigidBody.AddForce(Vector2.up * flightForce);
		}else if(_state == State.SAVED){
			rigidBody.AddForce(new Vector2(vaccumForce/2 * (transform.position.x > 0 ? -1 : 1), vaccumForce));
		}
		if(_state == State.ACTIVE && (GameManager.ActiveState == GameManager.State.RoundLost || GameManager.ActiveState == GameManager.State.RoundWin)){
			Destroy(gameObject);
		}
	}

	//Collision with other animals
	void OnCollisionEnter2D(Collision2D collision){
		if(collision.transform.tag == "Anchor" && _state != State.SAVED){
			if(_state == State.ACTIVE){
				SoundManager.PlaySoundEffect(stickSound);
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
		}else if(collider.CompareTag("Grinder") && _state != State.SAVED){
            if (_state == State.ACTIVE)
            {
				SoundManager.PlaySoundEffect(stickSound);
                ResetPlayer();
            }
			GrindPlayer();
			ChangeState(State.DISABLED);
		}

	}

	void ChangeState(State state){
		_state = state;
		if(_state == State.DISABLED || _state == State.SAVED) DisablePlayer();
	}

	//Spawn a new, controllable character
	void ResetPlayer(){
		gameObject.transform.tag = "Anchor";
        var playerInfo = PlayerManager.GetPlayerInfo(this.GetComponent<InputMapper>().playerId);
        if (playerInfo != null) {
            if (playerInfo.CharacterInstance == this.gameObject) {
                playerInfo.CharacterInstance = null;
                SpawnManager.SpawnPlayer(this.GetComponent<InputMapper>().playerId);
            }
        }
	}

	void DisablePlayer(){
		playerMove.enabled = false;
		playerJump.enabled = false;
		playerAura.enabled = false;
		if(playerDive) playerDive.enabled = false;
		if(playerFly) playerFly.enabled = false;
		if(playerDash) playerDash.enabled = false;
	}

	void GrindPlayer(){
		rigidBody.isKinematic = true;
		rigidBody.velocity = Vector2.down * activeGrindSpeed * Time.deltaTime;
		gameObject.GetComponentInChildren<Animator>().SetBool("isGrinding", true);
		bloodParticles.Play();
		transform.Find("Particle System").Rotate(-transform.eulerAngles);
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
		endHinge.frequency = 9f;
		endHinge.dampingRatio = 0.3f;

		endHinge.anchor = GetVectorOffset(gameObject, collision.gameObject, transform.eulerAngles.z) * gameObject.GetComponent<CircleCollider2D>().radius;
		endHinge.connectedAnchor = GetVectorOffset(collision.gameObject, gameObject, collision.transform.eulerAngles.z) * collision.gameObject.GetComponent<CircleCollider2D>().radius;

        if (firstStick == true) {
            firstStick = false;
            if (GameManager.GetRequireHeightForWin() < transform.position.y) {
                ScoreManager.ModifyPlayerScore(GetComponent<InputMapper>().playerId, scoreValue * 10);
                GameManager.EndRound(true);
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
