using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour {

	public float dashVelocity = 40f;
	public LayerMask layerMask;
	public AudioClip dashSound;

	Rigidbody2D rigidBody;
	Animator animator;
	string dashInput;
	string dashDirectionInput;
	bool canDash = false                                ;
	float distToGround;

	// Use this for initialization
	void Start () {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		animator = gameObject.GetComponentInChildren<Animator>();
		dashInput = PlayerManager.GetPlayerInputStr(gameObject.GetComponent<InputMapper>().playerId, "X");
		distToGround = gameObject.GetComponent<CircleCollider2D>().bounds.extents.y;
	}

	// Update is called once per frame
	void FixedUpdate () {
		IsGrounded();
		Dash();
	}

	void OnCollisionEnter2D(Collision2D collision){
		animator.SetBool("isDashing",false);
	}

	bool IsGrounded()
	{
		var grounded = Physics2D.Raycast(transform.position, Vector2.down, distToGround + 0.1f, layerMask);
		if(grounded) canDash = true;
		return grounded;
	}

	void Dash(){
		float dashDirection = Input.GetAxis(PlayerManager.GetPlayerInputStr(gameObject.GetComponent<InputMapper>().playerId, "LS_Horizontal"));
		if(Input.GetButtonDown(dashInput) && dashDirection != 0 && !IsGrounded() && canDash){
			rigidBody.MoveRotation(Mathf.Atan2(0, dashDirection)*Mathf.Rad2Deg + 180);
			rigidBody.velocity = Vector2.right * dashDirection * dashVelocity;
			SoundManager.PlaySoundEffect(dashSound);
			animator.SetBool("isDashing", true);
			StartCoroutine("StopDash");
			canDash = false;
		}
	}

	IEnumerator StopDash()
	{
		yield return new WaitForSeconds(0.4f);
		rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, 12f);
		animator.SetBool("isDashing", false);
	}
}
