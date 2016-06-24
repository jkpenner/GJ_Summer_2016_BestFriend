using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : Singleton<SpawnManager> {
    [SerializeField]
    private bool _useRandom;

    public bool UseRandom {
        get { return _useRandom; }
        set { _useRandom = value; }
    }

    private Dictionary<PlayerId, GameObject> _playerGameObjects;

    [SerializeField]
    private List<PlayerSpawner> _spawnPoints;

    public void Awake() {
        UseRandom = SettingManager.ActiveGameMode == SettingManager.GameMode.Random;
        GameManager.AddRoundListner(GameManager.RoundEventType.Begin, OnRoundStart);
        PlayerManager.AddListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
    }

    public void Disable() {
        GameManager.RemoveRoundListener(GameManager.RoundEventType.Begin, OnRoundStart);
        PlayerManager.RemoveListener(PlayerManager.EventType.PlayerConnect, OnPlayerConnect);
    }

    private void OnPlayerConnect(PlayerInfo player) {
        SpawnPlayer(player.Id);
    }

    private void OnRoundStart() {
        var playerInfos = PlayerManager.GetAllPlayerInfo();
        foreach (var playerInfo in playerInfos) {
            if (playerInfo.IsConnected) {
                SpawnPlayer(playerInfo.Id, true);
            }
        }
    }

    static public void AddSpawn(PlayerSpawner spawn) {
        if (Instance != null && spawn != null) {
            Instance._spawnPoints.Add(spawn);
        } else {
            Debug.LogWarning("[SpawnManager]: Attempting to add spawn point, but no instance found in scene");
        }
    }

    static public void SpawnPlayer(PlayerId playerId) {
        SpawnPlayer(playerId, false);
    }

    static public void SpawnPlayer(PlayerId playerId, bool force) {
        if (Instance != null) {
            List<PlayerSpawner> usableSpawns = new List<PlayerSpawner>();
            for (int i = 0; i < Instance._spawnPoints.Count; i++) {
                var spawn = Instance._spawnPoints[i];
                if (spawn.IsUsableSpawn(playerId)) {
                    usableSpawns.Add(spawn);
                }
            }

            SpawnPlayer(playerId, usableSpawns[Random.Range(0, usableSpawns.Count)], force);
        } else {
            Debug.LogWarning("[SpawnManager]: Attempting to spawn player, but no instance found in scene");
        }
    }

    static public void SpawnPlayer(PlayerId playerId, PlayerSpawner spawn) {
        SpawnPlayer(playerId, spawn, false);
    }

    static public void SpawnPlayer(PlayerId playerId, PlayerSpawner spawn, bool force) {
        var playerInfo = PlayerManager.GetPlayerInfo(playerId);
        if (playerInfo != null && spawn != null) {
            if (Instance.UseRandom) {
                if (force) {
                    spawn.CreateNewPlayerRandomForce(playerId);
                } else {
                    spawn.CreateNewPlayerRandom(playerId);
                }
            } else {
                if (force) {
                    spawn.CreateNewPlayerForce(playerId, playerInfo.CharacterSelectionId);
                } else {
                    spawn.CreateNewPlayer(playerId, playerInfo.CharacterSelectionId);
                }
            }
        }
    }
}
