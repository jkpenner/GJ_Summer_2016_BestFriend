using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    public PlayerId playerId;

    private void Awake() {
        // Add Spawn to Spawn Manager on Awake
        SpawnManager.AddSpawn(this);
    }

    // Use this for initialization
    void Start() {
        // Disable this spawner if playerNumber is greater the max players
        if (SettingManager.ActivePlayerCount == SettingManager.PlayerCount.TwoPlayer &&
            (playerId == PlayerId.Three || playerId == PlayerId.Four)) {
            this.gameObject.SetActive(false);
            return;
        }
    }

    // ToDo: Update playerId to be a Flag type enum
    public bool IsUsableSpawn(PlayerId playerId) {
        return this.playerId == playerId;
    }

    public void CreateNewPlayerRandom(PlayerId playerId) {
        if (GameManager.ActiveState == GameManager.State.Active ||
            GameManager.ActiveState == GameManager.State.Pause) {
            CreateNewPlayerRandomForce(playerId);
        }
    }

    public void CreateNewPlayerRandomForce(PlayerId playerId) {
        CreateNewPlayerForce(playerId, CharacterDatabase.Instance.Get(Random.Range(0, CharacterDatabase.Instance.Count)).Id);
    }

    public void CreateNewPlayer(PlayerId playerId, int characterId) {
        if (GameManager.ActiveState == GameManager.State.Active ||
            GameManager.ActiveState == GameManager.State.Pause) {
            CreateNewPlayerForce(playerId, characterId);
        }
    }

    public void CreateNewPlayerForce(PlayerId playerId, int characterId) {
        var playerInfo = PlayerManager.GetPlayerInfo(playerId);
        if (playerInfo != null && playerInfo.IsConnected) {
            var asset = CharacterDatabase.GetAsset(characterId);
            if (asset != null && playerInfo.CharacterInstance == null) {
                var newPlayer = GameObject.Instantiate(asset.Prefab, transform.position, Quaternion.identity) as GameObject;
                playerInfo.CharacterInstance = newPlayer;
                newPlayer.GetComponent<InputMapper>().SetPlayerId(playerId);
            } else {
                Debug.LogWarningFormat("[{0}]: Character Asset with id {1} is null", this.name, characterId);
            }
        }
    }
}
