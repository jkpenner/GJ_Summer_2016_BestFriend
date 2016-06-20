﻿using UnityEngine;


public class PlayerSpawner : MonoBehaviour {
    public bool useRandom = true;
    public GameObject[] playerObjects;
    public int playerNumber;

    // Use this for initialization
    void Start() {
        if (playerNumber > 2 && SettingManager.ActivePlayerCount == SettingManager.PlayerCount.TwoPlayer) {
            this.gameObject.SetActive(false);
            return;
        }

        useRandom = SettingManager.ActiveGameMode == SettingManager.GameMode.Random;

        ScoreManager.AddListener(ScoreManager.RoundEventType.Start, OnRoundStart);

        PlayerManager.AddListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerDisconnect, OnPlayerDisconnect);
    }

    private void OnRoundStart() {
        CreateNewPlayer(true);
    }

    private void OnPlayerDisconnect(PlayerManager.PlayerInfo player) {

    }

    private void OnPlayerConnect(PlayerManager.PlayerInfo player) {
        if (player.id == playerNumber) {
            CreateNewPlayer();
        }
    }

    public void CreateNewPlayer() {
        CreateNewPlayer(false);
    }

    public void CreateNewPlayer(bool force) {
        if (force || (GameManager.ActiveState == GameManager.State.Active ||
            GameManager.ActiveState == GameManager.State.Pause)) {
            var playerInfo = PlayerManager.GetPlayerInfo(playerNumber);
            if (playerInfo != null && playerInfo.IsConnected) {
                if (useRandom) {
                    SpawnRandom();
                } else {
                    var prefab = playerObjects[playerInfo.CharacterSelection];
                    var instance = (GameObject)Instantiate(prefab, transform.position, Quaternion.identity);
                    instance.GetComponent<InputMapper>().SetPlayerNumber(playerNumber);
                }
            }
        }
    }

    private void SpawnRandom() {
        // Pull a random character from the character database
        var prefab = playerObjects[Random.Range(0, playerObjects.Length)];
        // Instantiate the prefab from the selected asset
        GameObject newPlayer = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        newPlayer.GetComponent<InputMapper>().SetPlayerNumber(playerNumber);
    }
}
