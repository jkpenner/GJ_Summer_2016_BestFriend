using UnityEngine; 
using System.Collections; 

public class PlayerDive : MonoBehaviour { 

	public float diveForce = 15000f; 
	public LayerMask layerMask;
	public AudioClip diveSound;

	Rigidbody2D rigidBody; 
	string diveInput;
    float distToGround;
    bool canDive = false;

	// Use this for initialization 
	void Start () { 
		rigidBody = gameObject.GetComponent<Rigidbody2D>(); 
		diveInput = PlayerManager.GetPlayerInputStr(gameObject.GetComponent<InputMapper>().playerId, "X");
		distToGround = gameObject.GetComponent<CircleCollider2D>().bounds.extents.y;
    } 

	// Update is called once per frame 
	void FixedUpdate () { 
		IsGrounded();
		Dive(); 
	}
    
    bool IsGrounded() {
		var grounded = Physics2D.Raycast(transform.position, -Vector2.up, distToGround + 0.1f, layerMask);
		if(grounded) canDive = true;
		return grounded;
    } 

	void Dive(){ 
		if(Input.GetButtonDown(diveInput) && !IsGrounded() && canDive){ 
			SoundManager.PlaySoundEffect(diveSound);
            canDive = false;
			rigidBody.AddForce(new Vector2(0f, -diveForce)); 
		} 
	} 
} 