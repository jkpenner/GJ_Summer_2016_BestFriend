using UnityEngine; 
using System.Collections; 

public class PlayerDive : MonoBehaviour { 

	public float diveForce = 15000f; 

	Rigidbody2D rigidBody; 
	string jumpInput; 
	bool canDive = false; 

	// Use this for initialization 
	void Start () { 
		rigidBody = gameObject.GetComponent<Rigidbody2D>(); 
		jumpInput = gameObject.GetComponent<InputMapper>().GetMappedInput("Jump"); 
	} 

	// Update is called once per frame 
	void Update () { 
		Dive(); 
	} 

	//Collision with other animals 
	void OnCollisionEnter2D(Collision2D collision){ 
		canDive = true; 
	} 

	void Dive(){ 
		if(Input.GetButtonDown(jumpInput) && Mathf.Abs(rigidBody.velocity.y) > 0.1){ 
			Debug.Log("dive"); 
			rigidBody.AddForce(new Vector2(0f, -diveForce)); 
			canDive = false; 
		} 
	} 
} 