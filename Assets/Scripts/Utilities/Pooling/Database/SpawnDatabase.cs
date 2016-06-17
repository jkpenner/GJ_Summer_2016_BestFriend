using UnityEngine;
using System.Collections;

public class SpawnDatabase : BaseDatabase<SpawnAsset> {
    const string DatabasePath = @"Resources/Databases/";
    const string DatabaseName = @"SpawnDatabase.asset";

    static private SpawnDatabase _instance = null;
    static public SpawnDatabase Instance {
        get {
            if (_instance == null) {
                _instance = GetDatabase<SpawnDatabase>(DatabasePath, DatabaseName);
            }
            return _instance;
        }
    }

    static public SpawnAsset GetAsset(int id) {
        if (Instance != null) {
            return Instance.GetById(id);
        }
        return null;
    }
}
