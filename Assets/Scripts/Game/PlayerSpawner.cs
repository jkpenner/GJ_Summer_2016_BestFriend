using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    public PlayerId playerId;
    public bool useRandom = true;

    private void Awake() {
        // Add Spawn to Spawn Manager on Awake
        SpawnManager.AddSpawn(this);
    }

    // Use this for initialization
    void Start() {
	// Disable this spawner if playerNumber is greater the max players
        if(SettingManager.ActivePlayerCount == SettingManager.PlayerCount.TwoPlayer && 
            (playerId == PlayerId.Three || playerId == PlayerId.Four)) {
            this.gameObject.SetActive(false);
            return;
        }

        // Set up if the spawner is random 
        useRandom = SettingManager.ActiveGameMode == SettingManager.GameMode.Random;

        // Setup event listeners
        ScoreManager.AddListener(ScoreManager.RoundEventType.Start, OnRoundStart);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
    }

    private void Disable() {
        ScoreManager.RemoveListener(ScoreManager.RoundEventType.Start, OnRoundStart);
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
    }

    // ToDo: Update playerId to be a Flag type enum
    public bool IsUsableSpawn(PlayerId playerId) {
        return this.playerId == playerId;
    }

    private void OnRoundStart() {
        CreateNewPlayerForce(playerNumber);
    }

    private void OnPlayerDisconnect(PlayerInfo player) {

    }

    private void OnPlayerConnect(PlayerInfo player) {
        if (player.Id == playerId) {
            CreateNewPlayer();
        }
    }

    public void CreateNewPlayer() {
        CreateNewPlayer(playerNumber);
    }

    public void CreateNewPlayer(PlayerId playerId) {
        if (GameManager.ActiveState == GameManager.State.Active ||
            GameManager.ActiveState == GameManager.State.Pause) {
            CreateNewPlayerForce(playerId);
        }
    }

    public void CreateNewPlayerForce(PlayerId playerId) {
        var playerInfo = PlayerManager.GetPlayerInfo(playerId);
        if (playerInfo != null && playerInfo.IsConnected) {
            if (useRandom) {
                SpawnRandom();
            } else {
                Spawn(CharacterDatabase.GetAsset(playerInfo.CharacterSelectionId).Prefab);
            }
        }
    }

    private void SpawnRandom() {
        // Pull a random character from the character database
        Spawn(CharacterDatabase.Instance.Get(Random.Range(0, CharacterDatabase.Instance.Count)).Prefab);
    }

    private void Spawn(GameObject prefab) {
        GameObject newPlayer = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        newPlayer.GetComponent<InputMapper>().SetPlayerId(playerId);
    }
}
