using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {

	public GameObject[] playerObjects;
	public int playerNumber;

	// Use this for initialization
	void Start () {
		CreateNewPlayer();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CreateNewPlayer(){
		GameObject newPlayer = Instantiate(playerObjects[Random.Range(0, playerObjects.Length)], transform.position, Quaternion.identity) as GameObject;
		newPlayer.GetComponent<InputMapper>().SetPlayerNumber(playerNumber);
	}
}
