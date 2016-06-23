using UnityEngine;
using System.Collections;

public class VaccumDestroyer : MonoBehaviour {

	BoxCollider2D collider;

	// Use this for initialization
	void Start () {
		collider = gameObject.GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.ActiveState == GameManager.State.Active){
			collider.enabled = false;
		}else{
			collider.enabled = true;
		}
	}
}
