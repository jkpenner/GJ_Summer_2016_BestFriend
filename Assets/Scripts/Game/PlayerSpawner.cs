using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {
    public bool useRandom = true;
	public GameObject[] playerObjects;
	public int playerNumber;

    public UICharacterSelector characterSelector;

	// Use this for initialization
	void Start () {
		CreateNewPlayer();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CreateNewPlayer(){
        if (useRandom || (characterSelector == null && !useRandom)) {
            // Pull a random character from the character database
            var asset = CharacterDatabase.Instance.Get(Random.Range(0, CharacterDatabase.Instance.Count));
            // Instantiate the prefab from the selected asset
            GameObject newPlayer = Instantiate(asset.Prefab, transform.position, Quaternion.identity) as GameObject;
            newPlayer.GetComponent<InputMapper>().SetPlayerNumber(playerNumber);
        } else {
            // Show the character selector and spawn the selected character
            characterSelector.Display((id) => {
                var asset = CharacterDatabase.GetAsset(id);
                var instance = (GameObject)Instantiate(asset.Prefab, transform.position, Quaternion.identity);
                instance.GetComponent<InputMapper>().SetPlayerNumber(playerNumber);
            }, Camera.main.WorldToScreenPoint(transform.position));
        }
	}
}
