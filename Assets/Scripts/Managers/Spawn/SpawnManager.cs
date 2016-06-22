using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : Singleton<SpawnManager> {
    private List<PlayerSpawner> _spawnPoints;

    static public void AddSpawn(PlayerSpawner spawn) {
        if (Instance != null && spawn != null) {
            Instance._spawnPoints.Add(spawn);
        } else {
            Debug.LogWarning("[SpawnManager]: Attempting to add spawn point, but no instance found in scene");
        }
    }

    static public void SpawnPlayer(PlayerId playerId) {
        if (Instance != null) {
            List<PlayerSpawner> usableSpawns = new List<PlayerSpawner>();
            for (int i = 0; i < Instance._spawnPoints.Count; i++) {
                var spawn = Instance._spawnPoints[i];
                if (spawn.IsUsableSpawn(playerId)) {
                    usableSpawns.Add(spawn);
                }
            }

            SpawnPlayer(playerId, usableSpawns[Random.Range(0, usableSpawns.Count)]);
        } else {
            Debug.LogWarning("[SpawnManager]: Attempting to spawn player, but no instance found in scene");
        }
    }

    static public void SpawnPlayer(PlayerId playerId, PlayerSpawner spawn) {
        SpawnPlayer(playerId, spawn, false);
    }

    static public void SpawnPlayer(PlayerId playerId, PlayerSpawner spawn, bool force) {
        if(spawn != null) spawn.CreateNewPlayer(force);
    }
}
